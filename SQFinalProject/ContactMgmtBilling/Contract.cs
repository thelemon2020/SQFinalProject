//*********************************************
// File			 : ContractDetails.cs
// Project		 : PROG2020 - Term Project
// Programmer	 : Nick Byam, Chris Lemon, Deric Kruse, Mark Fraser
// Last Change   : 2020-11-25
// Description	 : A class to hold all of the pertinent details of a contract.
//*********************************************


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQFinalProject.ContactMgmtBilling
{
    ///
    /// \class ContractDetails
    /// 
    /// \brief A class that holds all pertinent details of a contract from the contract marketplace
    /// 
    /// \author <i>Nick Byam</i>
    /// 
    public class Contract
    {
        static private int ContractID = 0;
        //! Properties
        public double LTLUpCharge { get; set; } //!<Company rate for LTL delivery contracts 
        public double FTLUpCharge { get; set; } //!< Company rate for FTL delivery contracts 
        public double ReeferUpCharge { get; set; } //!<Company rate for Reefer van contracts 
        public int ID { get; set; } //!< Contract ID 
        public string ClientName { get; set; } //!< The name of the client
        public int JobType { get; set; } //!< The type of job, represented by an integer, 0 or 1 -> FTL or LTL */
        public int Quantity { get; set; } //!< Quantity of pallets to be delivered, only necessary for LTL deliveries */
        public string Origin { get; set; } //!< The origin city 
        public string Destination { get; set; } //!< The destination city 
        public double Distance { get; set; } //!< The total distance of the trips 
        public double Rate { get; set; } //!< The perk km or per pallet rate set by the carrier
        public int VanType { get; set; } //!< The van type, 0 or 1 -> Dry van or Reefer van 
        public bool TripComplete { get; set; } //!< A boolean which represents if the trip has been completed or not
        public double Cost { get; set; } //!<The flat cost of the trip before rates are applied
        public DateTime ETA { get; set; } //!<Estimated Time of Arrival
        public string Carrier { get; set; } //!<The name of t
        //public List<TripLine> { get; set; } 


        /// \brief A constructor for the ContractDetails class
        /// \details <b>Details</b>
        /// The constructor for the ContractDetails class, it takes an assortment of parameters which are to be gathered from the contract
        /// marketplace database. A number of parameters of the class are not set until a carrier for the contract is selected from the
        /// Carrier database.
        /// \param - name - <b>string</b> - The customer's name
        /// \param - job - <b>int</b> - The job type identified by an integer
        /// \param - quant - <b>int</b> - the quantity of pallets to deliver, 0 if the delivery is FTL
        /// \param - origin - <b>string</b> - The origin city
        /// \param - dest - <b>string</b> - The destination city
        /// \param - van - <b>int</b> - The van type identified by an integer
        /// \returns - <b>Nothing</b>
        public Contract(string name, int job, int quant, string origin, string dest, int van)
        {
            ID = ContractID;
            ClientName = name;
            JobType = job;
            Quantity = quant;
            Origin = origin;
            Destination = dest;
            VanType = van;
            TripComplete = false;
            Rate = 0.00;
            Distance = 0.00;
            Cost = 0.00;
            ContractID++;
        }


        /// \brief A method to reset the ID count of active contracts
        /// \details <b>Details</b>
        /// A method which checks how many contract classes are active so a unique ID can be assigned
        /// \param - <b>Nothing</b>
        /// \returns - <b>Nothing</b>
        public static void ResetIdCount()
        {
            ContractID = 0;
        }


        /// \brief A method to set the current ID count
        /// \details <b>Details</b>
        /// Sets the current ID count. On a new session if there are exisiting contracts in the database this can be used to make sure
        /// new contracts that are created are assigned a unique ID
        /// \param - number - <b>int</b> - the number to assign to the ContractID field
        /// \returns - <b>Nothing</b>
        public static void SetIdCount(int number)
        {
            ContractID = number;
        }


        /// \brief An overriden ToString method for ContractDetails
        /// \details <b>Details</b>
        /// This method uses a string builder to create a formatted string of pertinent information relating to the contract
        /// \param - <b>Nothing</b>
        /// \returns - contract.ToString - <b>string</b> - The StringBuilder instance converted to a string containing the string rep of the contract
        public override string ToString()
        {
            StringBuilder contract = new StringBuilder();
            string tmpJobType;
            string tmpVanType;
            if(JobType == 0) // if the job type is FTL
            {
                tmpJobType = "FTL";
            }
            else // job type is LTL
            {
                tmpJobType = "LTL";
            }

            if(VanType == 0) // Van type is Dry
            {
                tmpVanType = "DRY";
            }
            else // van type is Reefer
            {
                tmpVanType = "REF";
            }
            contract.AppendFormat("ID:{0} Name:{1} Job:{2} Quantity:{3} Origin:{4} Destination:{5} Van:{6} Delivery Complete: {7}",ID, ClientName, tmpJobType, Quantity, Origin, Destination, tmpVanType, TripComplete.ToString());
            return contract.ToString();
        }
    }
}
