using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQFinalProject.TripPlanning
{
    /// 
    /// \class <b>TripLine</b>
    ///
    /// \brief This class joins together a <b>Truck</b> with a <b>Contract</b>.  No real error handling is needed as a <b>Truck</b> and a <b>Contract</b> must exist
    /// in a usable state to get a <b>TripLine</b>
    ///
    /// \see Truck
    /// \see Contract
    /// \author <i>Chris Lemon</i>
    ///
    class TripLine
    {
        public int ContractID { get; set; }
        public int TripID { get; set; }
        public int Quantity { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public int EstTime { get; set; }
        public int EstKM { get; set; }

        /// \brief Constructor for the TripLine class
        /// \details <b>Details</b>
        /// Sets some general properties based on arguments passed in from another method 
        /// 
        /// \param - connectionString - <b>string</b> - a string holding all the information neccessary to connect to a database
        /// 
        /// \return - connection - <b>MySqlConnection</b> - The object that represents the connection to a database
        /// 
        public TripLine(int contract, int trip, int qty)
        {
            ContractID = contract;
            TripID = trip;
            Quantity = qty;
            EstTime = 0;
        }
    }
}
