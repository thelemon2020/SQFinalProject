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
        static private int LastID { get; set; } = 0;    //<Static ID value to assign new IDs incrementally
        public int TripID { get; set; }                 //<The ID number of the trip
        public int CarrierID { get; set; }              //<The ID number of the carrier
        public string Origin { get; set; }              //<The Origin city that the carrier departs from
        public string Destination { get; set; }         //<The end destination of the truck
        public int VanType { get; set; }                //<The Truck type
        public double Rate { get; set; }                //<Either the LTL rate or FTL rate depending on job type
        public double HoursWorked { get; set; }         //<The amount of hours that the driver has worked
        public double HoursDriven { get; set; }         //<The amount of hours that the driver has driven
        public int DaysWorked { get; set; }             //<The total days the trip will take to complete
        public double ReeferRate { get; set; }          //<The reefer rate of the carrier if the truck is a reefer truck
        public double BillTotal { get; set; }           //<The total price charged by the carrier for the trip
        public bool IsComplete { get; set; }            //<Flag to set trip to complete
        public List<TripLine> Contracts { get; set; }   //<A list of contracts that the Truck will have to deliver
        public Route ThisRoute { get; set; }            //<A route object modelling the route to be taken by the truck


        /// \brief Constructor for the truck class
        /// \details <b>Details</b>
        /// The constructor for the Truck class
        /// \param - <b>Database</b> - 
        /// \param - <b>Contract</b> - 
        /// \returns - <b>Nothing</b>
        /// 
        /// \see 
        public Truck(Database tms, Contract contract, Carrier carrier, int qty)
        {
            if (LastID == 0)
            {
                LastID = Controller.GetLastTripID(tms);
            }

            LastID++;
            TripID = LastID;
            CarrierID = carrier.CarrierID;
            Origin = contract.Origin;
            Destination = contract.Destination;
            VanType = contract.VanType;
            Rate = 0;
            Contracts = new List<TripLine> { new TripLine(contract, TripID, qty) };
            IsComplete = false;
        }

        //
        public void SimulateDay()
        {
            HoursWorked = 0;
            HoursDriven = 0;

            if (ThisRoute.Cities[0].Name == Origin)
            {   // Load contracts if the truck is still in the origin city
                LoadContracts();
            }
            while(kMaxTotalHours >= HoursWorked + ThisRoute.Cities[0].Time && kMaxDrivingHours < HoursDriven + ThisRoute.Cities[0].Time)
            {
                for (int i = 0; i < Contracts.Count(); i++)
                {
                    if (Contracts[i].Destination == ThisRoute.Cities[0].Name)
                    {
                        Unload(i);
                    }
                }
                ContinueRoute();
            }

            DaysWorked++;
            if (Contracts[0].IsDelivered == true)
            {
                if (Contracts.Count() > 1 && Contracts[1].IsDelivered == true)
                {
                    IsComplete = true;
                }
            }
        }
        /// \brief A method to simulate the truck continuing to the next city
        /// \details <b>Details</b>
        /// A method that will increase the work time equal to the number of hours to the next city then update the contracts delivered if any
        /// \param - <b>Nothing</b>
        /// \returns - <b>Nothing</b>
        /// 
        /// \see Load(TripLine contract)
        /// \see Unload(TripLine contract)
        public void ContinueRoute()
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
        public void LoadContracts()
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
        public void Unload(int index)
        {
            HoursWorked += 2;
            Contracts[index].IsDelivered = true;
            if (index + 1 == Contracts.Count())
            {
                IsComplete = true;
                //SaveToDB()? or use button idk
            }
        }

        public void SaveToDB()
        {

        }
    }
}
