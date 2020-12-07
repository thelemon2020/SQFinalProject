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
            TotalQuantity = contract.Quantity;
            Rate = 0.0;
            HoursWorked = 0.0;
            HoursDriven = 0.0;
            DaysWorked = 0;
            ReeferRate = carrier.ReefCharge;
            BillTotal = 0.0;
            IsComplete = false;
            Contracts = new List<TripLine> { new TripLine(contract, TripID, qty) };
            ThisRoute = new Route();
            ThisRoute.GetCities(Origin, Destination);
            Contracts[0].Distance = ThisRoute.TotalDistance;
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
                }
                else
                {
                    Route tmpRt = new Route();
                    tmpRt.GetCities(Origin, contract.Destination);
                    contract.Distance = tmpRt.TotalDistance;
                }
                TotalQuantity += contract.Quantity;
            }

            return success;
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
            }
            // Increment additiontal days worked counter and add associated surcharge
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
    }
}
