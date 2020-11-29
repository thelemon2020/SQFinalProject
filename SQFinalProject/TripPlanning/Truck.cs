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
    /// \author <i>Nick Byam</i>
    ///
    public class Truck
    {
        //! Properties
        private const int kMaxPallets = 26; //<The max number of pallets allowed on one truck
        public int TripID { get; set; } //<The Id number of the trip
        public int CarrierID { get; set; } //<The ID number of the carrier
        public string Origin { get; set; } //<The Origin city that the carrier departs from
        public string Destination { get; set; } //<The end destination of the truck
        public string VanType { get; set; } //<The Truck type
        public double Rate { get; set; } //<Either the LTL rate or FTL rate depending on job type
        public double HoursWorked { get; set; } //<The amount of hours that the driver has worked
        public int DaysWorked { get; set; } //<The total days the trip will take to complete
        public double ReeferRate { get; set; } //<The reefer rate of the carrier if the truck is a reefer truck
        public List<TripLine> Contracts { get; set; } //<A list of contracts that the Truck will have to deliver


        /// \brief Constructor for the truck class
        /// \details <b>Details</b>
        /// The constructor for the Truck class
        /// \param - <b>Nothing</b>
        /// \returns - <b>Nothing</b>
        /// 
        /// \see 
        public Truck()
        {

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
            // increase work time, move to next city
        }


        /// \brief A method that simulates loading product onto the truck
        /// \details <b>Details</b>
        /// A method to simulate loading the truck with product related to a contract, it adds 2 hours to the total work time
        /// adds a new contract to the list, then updates pallet counts
        /// \param - contract <b>TripLine</b> - The trip details of the contract
        /// \returns - <b>Nothing</b>
        /// 
        /// \see Unload(TripLine contract)
        public void Load(TripLine contract)
        {
            // update pallet count
            // Increase work time by 2 hours
            // Add a new TripLine to the List
        }


        /// \brief A method that simulates unloading the truck
        /// \details <b>Details</b>
        /// A method that simulates unloading an order from the truck. Adds 2 hours to the work time, removes a contract from the list,
        /// and then updates the number of pallets in the truck
        /// \param - contract - <b>TripLine</b> - The trip details of the contract
        /// \returns - <b>Nothing</b>
        /// 
        /// \see Load(TripLine contract)
        public void Unload(TripLine contract)
        {
            // update pallet count
            // Increase work time by 2 hours
            // Remove tripline from the List
        }

    }
}
