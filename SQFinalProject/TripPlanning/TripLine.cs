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
    public class TripLine
    {
        public int ContractID { get; set; }     //!< the contract associated with the trip line
        public int TripID { get; set; }         //!<the truck associated with the trip line
        public int Quantity { get; set; }       //!< quantity of order
        public string Destination { get; set; } //!< destination city
        public int DaysWorked { get; set; }     //!< days(past the first) taken to complete
        public int Distance { get; set; }       //!< distance to complete
        public bool IsDelivered { get; set; }   //!< Flag to mark the payload delivered

        /// \brief Constructor for the TripLine class
        /// \details <b>Details</b>
        /// Sets some general properties based on arguments passed in from another method 
        /// 
        /// \param - contract - <b>Contract</b> - the contract to be loaded onto a <b>Truck</b>
        /// \param - truck - <b>Truck</b> - the <b>Truck</b> to load the contract on to
        /// \param - qty -<b>int</b> - number of pallets to be fulfilled on this trip
        /// 
        /// \return N/A
        /// 
        public TripLine(Contract contract, int tripID, int qty)
        {
            ContractID = contract.ID;
            TripID = tripID;
            Quantity = qty;
            Destination = contract.Destination;
            DaysWorked = 0;
            Distance = 0;
            IsDelivered = false;
        }

        /// \brief Constructor for the TripLine class
        /// \details <b>Details</b>
        /// Creates a tripline method from TMS database for the contract class to process after completion
        /// 
        /// \param - details - <b>List<string></b> - the query return of a tripline from the tms db
        /// 
        /// \return N/A
        /// 
        public TripLine(string details, string destination)
        {
            string[] splitDetails = details.Split();
            ContractID = int.Parse(splitDetails[0]);
            TripID = int.Parse(splitDetails[1]);
            Quantity = int.Parse(splitDetails[2]);
            Destination = destination;
            DaysWorked = int.Parse(splitDetails[3]);
            Distance = int.Parse(splitDetails[4]);
            IsDelivered = bool.Parse(splitDetails[5]);
        }
    }
}
