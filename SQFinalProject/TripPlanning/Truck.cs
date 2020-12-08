using SQFinalProject.ContactMgmtBilling;
using SQFinalProject.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQFinalProject.TripPlanning
{
    /// 
    /// \class Truck
    ///
    /// \brief This class represents the truck and driver that will be delivering the products. It keeps track of its depot city and destination
    /// city, as well as a list of contracts it has onboard, the total hours the driver has worked, and associated rates used to calculate the cost
    ///
    /// \see Trip
    /// \see Route
    /// \author <i>Nick Byam, Mark Fraser</i>
    ///
    public class Truck
    {
        //! Properties
        private const int kMaxPallets = 26;             //<The max number of pallets allowed on one truck
        private const int kMaxTotalHours = 12;          //<The max number of hours drivers can spend working in total per day
        private const int kMaxDrivingHours = 8;         //<The max number of hours drivers can spend driving per day
        private const int kLoadTime = 2;                //<The time it takes to load or unload cargo
        static private int LastID { get; set; } = 0;    //<Static ID value to assign new IDs incrementally
        public int TripID { get; set; }                 //<The ID number of the trip
        public int CarrierID { get; set; }              //<The ID number of the carrier
        public string Origin { get; set; }              //<The Origin city that the carrier departs from
        public string Destination { get; set; }         //<The end destination of the truck
        public int VanType { get; set; }                //<The Truck type, 0 = dry van, 1 = reefer
        public int TotalQuantity { get; set; }          //<Quantity to be updated as cargo is added
        public double Rate { get; set; }                //<Either the LTL rate or FTL rate depending on quantity
        public double HoursWorked { get; set; }         //<The amount of hours that the driver has worked
        public double HoursDriven { get; set; }         //<The amount of hours that the driver has driven
        public int DaysWorked { get; set; }             //<The total days past the first the trip will take to complete
        public double ReeferRate { get; set; }          //<The reefer rate of the carrier if the truck is a reefer truck
        public double BillTotal { get; set; }           //<The total price charged by the carrier for the trip
        public bool IsComplete { get; set; }            //<Flag to set trip to complete
        public double TotalTime { get; set; }           //<Total time it takes to deliver the truck
        public List<TripLine> Contracts { get; set; }   //<A list of contracts that the Truck will have to deliver
        public Route ThisRoute { get; set; }            //<A route object modelling the route to be taken by the truck
        public string Direction { get; set; }
        public bool Corrected { get; set; }


        /// \brief Constructor for the truck class
        /// \details <b>Details</b>
        /// The constructor for the Truck class 
        /// \param - <b>contract</b> - Contract class containing details for the order
        /// \param - <b>carrier</b> - Carrier class containing details for the carrier
        /// \param - <b>qty</b> - int containing the number of pallets from the contract to be fulfilled by this truck
        /// \returns - <b>Nothing</b>
        /// 
        public Truck(Contract contract, Carrier carrier, int qty)
        {
            if (LastID == 0)
            {
                LastID = Controller.GetLastTripID();
            }

            LastID++;
            TripID = LastID;
            CarrierID = carrier.CarrierID;
            Origin = contract.Origin;
            Destination = contract.Destination;
            VanType = contract.VanType;
            TotalQuantity = qty;
            Rate = 0.0;
            HoursWorked = 0.0;
            HoursDriven = 0.0;
            DaysWorked = 0;
            ReeferRate = carrier.ReefCharge;
            BillTotal = 0.0;
            IsComplete = false;
            TotalTime = 0;
            Contracts = new List<TripLine> { new TripLine(contract, TripID, qty) };
            ThisRoute = new Route();
            ThisRoute.GetCities(Origin, Destination);
            Contracts[0].Distance = ThisRoute.TotalDistance;
            Direction = contract.Direction;
            Corrected = false;

            Contracts[0].CalculateTripTime(this);
        }


        /// \brief Method to add cargo from a contract to the trip
        /// \details <b>Details</b>
        /// The constructor for the Truck class
        /// \param - <b>contract</b> - an instantiated tripline to be added
        /// \returns - <b>success</b> - flag letting the calling method know if the contract was successfully added to the trip
        /// 
        public bool AddContract(TripLine contract)
        {
            bool success = false;

            if (contract.Quantity + TotalQuantity <= kMaxPallets && TotalQuantity != 0)
            {
                Contracts.Add(contract);
                if (!ThisRoute.AddCity(contract.Destination))    // False if route was modified
                {
                    Destination = contract.Destination;
                    contract.Distance = ThisRoute.TotalDistance;
                    contract.CalculateTripTime(this);
                }
                else
                {
                    Route tmpRt = new Route();
                    tmpRt.GetCities(Origin, contract.Destination);
                    contract.Distance = tmpRt.TotalDistance;
                    contract.CalculateTripTime(this);
                }
                TotalQuantity += contract.Quantity;
            }

            return success;
        }


        /// \brief Method to add cargo from a contract to the trip
        /// \details <b>Details</b>
        /// The constructor for the Truck class
        /// \param - N/A
        /// \returns - N/A
        /// 
        public void CorrectContractTime()
        {
            if(Contracts.Count == 1 && TotalQuantity == 0) // This is an FTL Trip, so we don't need to do anything.
            {
                return;
            }
            else // This is an LTL trip with potentially multiple contracts
            {
                if(Contracts.Count == 1 && TotalQuantity <= kMaxPallets) // LTL but only one contract, Add 2 hours for each city we got through
                {
                    if(ThisRoute.Cities.Count == 2) // the trip is only going one town over, don't add any extra time
                    {
                        return;
                    }

                    for(int i = 1; i < ThisRoute.Cities.Count - 1; i++) // we don't want to include the origin city or the destination
                    {
                        if(Contracts[0].Destination == ThisRoute.Cities[i].Name) // if we're at the destination then break the loop
                        {
                            break;
                        }

                        if(Contracts[0].HoursPerDay[0] <= 10) // if the first day is less than 10 hours of time with load and driving
                        {                                     // Then add 2 hours stop time to the first day.
                            Contracts[0].HoursPerDay[0] += 2;
                        }
                        else // The tot time for the day is already greater than 10 hours
                        {
                            Contracts[0].HoursPerDay[1] += 2; // add the extra two hours to the follow up day

                            if(Contracts[0].HoursPerDay[1] > kMaxTotalHours) // if adding the extra 2 hours stop time pushed the second day over
                            {                                                // over the limit we start adding time to a third day.
                                Contracts[0].HoursPerDay[1] -= 2; // make it so that the second day is not over the limit anymore
                                Contracts[0].HoursPerDay[2] += 2; // add the two hours to the third day.
                            }
                        }
                    }
                    
                }
                else // There are two or more trips
                {
                    // iterate through each trip adding 2 hours for each city stopped in
                    foreach(TripLine tl in Contracts)
                    {
                        if (tl.Destination == ThisRoute.Cities[1].Name) // the trip is only going to the next city, don't add any extra
                        {                                               // time for it.
                            continue;
                        }

                        for (int i = 1; i < ThisRoute.Cities.Count - 1; i++) // we don't want to include the origin city or the destination
                        {
                            if(tl.Destination == ThisRoute.Cities[i].Name) // if the trip line is at the current city then break;
                            {
                                break;
                            }

                            if(ThisRoute.Cities[i].Name != tl.Destination) // if we're not at the trip lines destination add 2 hours stop time
                            {                                              // Do this because we already added 2 hours unload time
                                                                           // and we don't want the shorter ltl trip to add 2 hours stop time
                                                                           // at its destination.

                                if (tl.HoursPerDay[0] <= 10) // if the first day is less than 10 hours of time with load and driving
                                {                                     // Then add 2 hours stop time to the first day.
                                    tl.HoursPerDay[0] += 2;
                                }
                                else // The total time for the day is already greater than 10 hours
                                {
                                    tl.HoursPerDay[1] += 2; // add the extra two hours to the follow up day

                                    if (tl.HoursPerDay[1] > kMaxTotalHours) // if adding the extra 2 hours stop time pushed the second
                                    {                                                // day over the limit we start adding time to a third day.
                                        tl.HoursPerDay[1] -= 2; // make it so that the second day is not over the limit anymore
                                        tl.HoursPerDay[2] += 2; // add the two hours to the third day.
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }


        /// \brief Method to simulate a workday for the truck
        /// \details <b>Details</b>
        /// The constructor for the Truck class
        /// \param - N/A
        /// \returns - N/A
        /// 
        public void SimulateDay()
        {
            HoursWorked = 0.0;
            HoursDriven = 0.0;

            if (ThisRoute.Cities[0].Name == Origin)
            {   
                // Load contracts and determine charges(cost to hire truck) if the truck is still in the origin city
                TotalCost();
                LoadContracts();
            }
            // Keep moving as long as "work limits" for the day are not reached
            while(kMaxTotalHours >= HoursWorked + ThisRoute.Cities[0].Time && kMaxDrivingHours > HoursDriven + ThisRoute.Cities[0].Time && !IsComplete)
            {
                // Loop through each contract too see if it should be unloaded in the current city
                for (int i = 0; i < Contracts.Count(); i++)
                {
                    if (Contracts[i].Destination == ThisRoute.Cities[0].Name && kMaxTotalHours > HoursWorked + 2)
                    {
                        Unload(i);
                    }
                }
                if (!IsComplete) ContinueRoute();
                else break;
            }
            // Increment additiontal days worked counter and add associated surcharge
            foreach (TripLine c in Contracts)
            {
                if (!c.IsDelivered) c.TotalTime += HoursWorked;
            }
            TotalTime += HoursWorked;
            DaysWorked++;
            BillTotal += 150;
        }

        /// \brief A method to simulate the truck continuing to the next city
        /// \details <b>Details</b>
        /// A method that will increase the work time equal to the number of hours to the next city then update the route object to move to the next city
        /// \param - <b>Nothing</b>
        /// \returns - <b>Nothing</b>
        /// 
        private void ContinueRoute()
        {
            // Increase hours worked/driven by the time to the next city
            HoursDriven += ThisRoute.Cities[0].Time;
            HoursWorked += ThisRoute.Cities[0].Time;
            ThisRoute.ArriveAtStop(); // Removes one city from the Cities list
        }


        /// \brief A method that simulates loading product onto the truck
        /// \details <b>Details</b>
        /// A method to simulate loading the truck with product related to a contract, it adds 2 hours to the total work time for each 
        /// contract that needs to be loaded
        /// \param - contract <b>TripLine</b> - The trip details of the contract
        /// \returns - <b>Nothing</b>
        /// 
        /// \see Unload(TripLine contract)
        private void LoadContracts()
        {
            for (int i = 0; i < Contracts.Count(); i++)
            {
                HoursWorked += 2;
            }
        }


        /// \brief A method that simulates unloading the truck
        /// \details <b>Details</b>
        /// A method that simulates unloading an order from the truck. Adds 2 hours to the work time, then updates
        /// final delivery information in the tripline and checks if all contracts are delivered in which case
        /// the truck's IsComplete flag is set
        /// \param - index - <b>int</b> - The index of the trip line (contract) being unloaded
        /// \returns - <b>Nothing</b>
        /// 
        /// \see Load(TripLine contract)
        private void Unload(int index)
        {
            bool allDelivered = true;
            HoursWorked += 2;
            Contracts[index].DaysWorked = DaysWorked;
            Contracts[index].TotalTime += (float)HoursWorked;
            Contracts[index].IsDelivered = true;
            // Check the delivery flag on each contract on the truck
            foreach (TripLine c in Contracts) if (c.IsDelivered == false) allDelivered = false;
            if (Contracts[index].Destination == Destination && allDelivered == true)
            {
                IsComplete = true;
            }
        }


        /// \brief A method that calculates the total cost for tms to hire the truck
        /// \details <b>Details</b>
        /// The rate is retrieved from the database based on how many pallets are on the truck (FTL or LTL Rate)
        /// then the rate/reefer charge(if applicable) are applied and 
        /// the truck's IsComplete flag is set
        /// \param - index - <b>int</b> - The index of the trip line (contract) being unloaded
        /// \returns - <b>Nothing</b>
        /// 
        public void TotalCost()
        {
            Rate = Controller.GetRate(CarrierID, TotalQuantity);
            if (Rate > 1) BillTotal = (ThisRoute.TotalDistance * Rate) * ((ReeferRate * VanType) + 1);
            else BillTotal = (ThisRoute.TotalDistance * Rate * TotalQuantity) * ((ReeferRate * VanType) + 1);
            //Round to 2 decimal places
            BillTotal = Math.Round(BillTotal, 2);
        }


        /// \brief A method for saving trucks to the TMS DB
        /// \details <b>Details</b>
        /// This method simply calls a controller method which performs the logic of saving truck data to the db
        /// \param - <b>nothing</b>
        /// \returns - <b>Nothing</b>
        /// 
        /// \see Truck
        public void SaveToDB()
        {
            Controller.SaveTripToDB(this);
        }


        /// \brief A method that returns the amount of space remaining on a truck
        /// \details <b>Details</b>
        /// TotalQuantity is simply subtracted from the maxumim total no. of pallets and the value is returned
        /// \param - <b>Nothing</b>
        /// \returns - <b>Nothing</b>
        /// 
        public int RemainingQuantity () {
            return kMaxPallets - TotalQuantity;
        }
    }
}
