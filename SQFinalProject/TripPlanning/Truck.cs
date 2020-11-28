using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQFinalProject.TripPlanning
{
    /// 
    /// \class <b>Truck</b>
    ///
    /// \brief This class represents the truck and driver that will be delivering the products. It keeps track of its depot city and destination
    /// city, as well as a list of contracts it has onboard, the total hours the driver has worked, and associated rates used to calculate the cost
    ///
    /// \see Trip
    /// \see Route
    /// \author <i>Nick Byam</i>
    ///
    class Truck
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
        /// \ details <b>Details</b>
        /// The constructor for the Truck class
        /// \param - <b>Nothing</b>
        /// \returns - <b>Nothing</b>
        /// 
        /// \see 
        public Truck()
        {

        }


        /// \brief A method to simulate the truck 
        /// \ details <b>Details</b>
        /// 
        /// \param - 
        /// \returns - 
        /// 
        /// \see
        public void ContinueRoute()
        {
            // increase work time, move to next city
        }


        /// \brief 
        /// \ details <b>Details</b>
        /// 
        /// \param - 
        /// \returns - 
        /// 
        /// \see
        public void Load(TripLine contract)
        {
            // update pallet count
            // Increase work time by 2 hours
            // Add a new TripLine to the List
        }


        /// \brief 
        /// \ details <b>Details</b>
        /// 
        /// \param - 
        /// \returns - 
        /// 
        /// \see
        public void Unload(TripLine contract)
        {
            // update pallet count
            // Increase work time by 2 hours
            // Remove tripline from the List
        }

    }
}
