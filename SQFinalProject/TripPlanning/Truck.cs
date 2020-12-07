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
        public double TotalTime { get; set; }              //<Total time it takes to deliver the truck
        public List<TripLine> Contracts { get; set; }   //<A list of contracts that the Truck will have to deliver
        public Route ThisRoute { get; set; }            //<A route object modelling the route to be taken by the truck


        /// \brief Constructor for the truck class
        /// \details <b>Details</b>
        /// The constructor for the Truck class 
        /// \param - <b>contract</b> - 
        /// \returns - <b>Nothing</b>
        /// 
        /// \see 
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
            Contracts[0].CalculateTripTime(this);
        }

        /// \brief Method to add cargo from a contract to the trip
        /// \details <b>Details</b>
        /// The constructor for the Truck class
        /// \param - contract - <b>TripLine</b> - an instantiated tripline
        /// \returns - success - <b>bool</b> - flag letting the calling method know if the contract was successfully added to the trip
        /// 
        /// \see 
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
                    for(int i = 1; i < ThisRoute.Cities.Count - 1; i++) // we don't want to include the origin city or the destination
                    {
                        if(Contracts[0].HoursPerDay[0] <= 10) // if the first day is less than 10 hours of time with load and driving
                        {                                     // Then add 2 hours stop time to the first day.
                            Contracts[0].HoursPerDay[0] += 2;
                        }
                        else // The total time for the day is already greater than 10 hours
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
                        for (int i = 1; i < ThisRoute.Cities.Count - 1; i++) // we don't want to include the origin city or the destination
                        {
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


        //
        public void SimulateDay()
        {
            HoursWorked = 0.0;
            HoursDriven = 0.0;

            if (ThisRoute.Cities[0].Name == Origin)
            {   // Load contracts and determine charges if the truck is still in the origin city
                TotalCost();
                LoadContracts();
            }
            while(kMaxTotalHours >= HoursWorked + ThisRoute.Cities[0].Time && kMaxDrivingHours > HoursDriven + ThisRoute.Cities[0].Time && !IsComplete)
            {
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
        /// A method that will increase the work time equal to the number of hours to the next city then update the contracts delivered if any
        /// \param - <b>Nothing</b>
        /// \returns - <b>Nothing</b>
        /// 
        /// \see Load(TripLine contract)
        /// \see Unload(TripLine contract)
        private void ContinueRoute()
        {
            HoursDriven += ThisRoute.Cities[0].Time;
            HoursWorked += ThisRoute.Cities[0].Time;
            ThisRoute.ArriveAtStop();
        }


        /// \brief A method that simulates loading product onto the truck
        /// \details <b>Details</b>
        /// A method to simulate loading the truck with product related to a contract, it adds 2 hours to the total work time for each 
        /// contract that needs to be loaded, then updates pallet counts
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
        /// A method that simulates unloading an order from the truck. Adds 2 hours to the work time, removes a contract from the list,
        /// and then updates the number of pallets in the truck
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

            foreach (TripLine c in Contracts) if (c.IsDelivered == false) allDelivered = false;
            if (Contracts[index].Destination == Destination && allDelivered == true)
            {
                IsComplete = true;
                //SaveToDB()? or use button idk
            }
        }
        
        public void TotalCost()
        {
            Rate = Controller.GetRate(CarrierID, TotalQuantity);
            if (Rate > 1) BillTotal = (ThisRoute.TotalDistance * Rate) * ((ReeferRate * VanType) + 1);
            else BillTotal = (ThisRoute.TotalDistance * Rate * TotalQuantity) * ((ReeferRate * VanType) + 1);
            //Round to 2 decimal places
            BillTotal = Math.Round(BillTotal, 2);
        }

        public void SaveToDB()
        {
            Controller.SaveTripToDB(this);
        }

        public int RemainingQuantity () {
            return kMaxPallets - TotalQuantity;
        }
    }
}
