using SQFinalProject.ContactMgmtBilling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQFinalProject.TripPlanning
{
    /// 
    /// \class TripLine
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
        /// \param - contract - <b>Contract</b> - the contract to be loaded onto a <b>Truck</b>
        /// \param - truck - <b>Truck</b> - the <b>Truck</b> to load the contract on to
        /// 
        /// \return - connection - <b>MySqlConnection</b> - The object that represents the connection to a database
        /// 
        public TripLine(Contract contract, Truck truck)
        {
            ContractID = contract.ID;
            TripID = truck.TripID;
            Quantity = contract.Quantity;
            EstTime = 0;
        }
    }
}
