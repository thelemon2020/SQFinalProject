//*********************************************
// File			 : ContractDetails.cs
// Project		 : PROG2020 - Term Project
// Programmer	 : Nick Byam, Chris Lemon, Deric Kruse, Mark Fraser
// Last Change   : 2020-11-25
// Description	 : A class to hold all of the pertinent details of a contract.
//*********************************************


using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQFinalProject.TripPlanning;

namespace SQFinalProject.ContactMgmtBilling
{
    ///
    /// \class ContractDetails
    /// 
    /// \brief A class that holds all pertinent details of a contract from the contract marketplace
    /// In this class not many errors would arise due to the fact that there is no file writing or areas where Null arguments would be a problem
    /// Thus there is no error handling.
    /// 
    /// \author <i>Nick Byam</i>
    /// 
    public class Contract
    {
        static private int ContractID = 0;
        //! Properties
        public const double LTLUpCharge = 1.05; //!<Company rate for LTL delivery contracts 
        public const double FTLUpCharge = 1.08; //!< Company rate for FTL delivery contracts 
        public double ReeferUpCharge { get; set; } //!<Company rate for Reefer van contracts 
        public int invoiceNum { get; set; }
        public int ID { get; set; } //!< Contract ID 
        public int AccountID { get; set; } //!< The Related account ID of which the contract belongs to
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
        public Carrier Carrier { get; set; } //!<The name of the carrier delivering the contract
        public List<TripLine> Trips { get; set; } //!< A list of the trips required to deliver the full order
        public string Status { get; set; }
        public ObservableCollection<string> Statuses { get; set; }


        /// \brief A constructor for the ContractDetails class
        /// \details <b>Details</b>
        /// The constructor for the ContractDetails class, it takes an assortment of parameters which are to be gathered from the contract
        /// marketplace database. A number of parameters of the class are not set until a carrier for the contract is selected from the
        /// Carrier database.
        /// \param - details - <b>List<string></b> - A list of contract details retrieved from the contract marketplace database
        /// \returns - <b>Nothing</b>
        public Contract(string details)
        {
            string[] splitDetails = details.Split(',');

            ClientName = splitDetails[0];
            JobType = int.Parse(splitDetails[1]);
            Quantity = int.Parse(splitDetails[2]);
            Origin = splitDetails[3];
            Destination = splitDetails[4];
            VanType = int.Parse(splitDetails[5]);
            
        }

        public Contract()
        {

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
            contract.AppendFormat("{0},{1},{2},{3},{4},{5},{6},{7}",ID, ClientName, tmpJobType, Quantity, Origin, Destination, tmpVanType, TripComplete.ToString());
            return contract.ToString();
        }
    }
}
