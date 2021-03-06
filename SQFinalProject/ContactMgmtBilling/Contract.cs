﻿//*********************************************
// File			 : Contract.cs
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
        public string invoiceNum { get; set; }
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
        public int RemainingQuantity { get; set; } //!< The remaining quantity of pallets for an LTL contract if the total quantity is > 26
        public string Direction { get; set; }
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
            Trips = new List<TripLine>();
            ClientName = splitDetails[0];
            JobType = int.Parse(splitDetails[1]);
            Quantity = int.Parse(splitDetails[2]);
            Origin = splitDetails[3];
            Destination = splitDetails[4];
            VanType = int.Parse(splitDetails[5]);
            Status = "NEW";
            GetEastOrWest();
        }


        /// \brief A constructor for the Contract class
        /// \details <b>Details</b>
        /// A basic constructor for the Contract class
        /// \param - <b>Nothing</b> 
        /// \returns - <b>Nothing</b>
        /// 
        /// \see Contract(string details)
        public Contract()
        {
        }


        /// \brief A method that checks to see if a contract has been fulfilled
        /// \details <b>Details</b>
        /// A method that checks all the trip lines associated with a single contract to see if they've all been delivered.
        /// It also relies on the contract having been fully divided into triplines.
        /// \param - <b>Nothing</b>
        /// \returns - complete - <b>bool</b> - Whether or not the contract was fulfilled
        /// 
        public bool IsContractComplete()
        {
            bool complete = true;
            
            foreach(TripLine tl in Trips) // all the trip lines on a contract mustbe completed
            {
                if(!tl.IsDelivered)
                {
                    complete = false;
                }
            }

            if(RemainingQuantity != 0) // and all skids of an order must be put into to trip lines and delivered
            {
                complete = false;
            }

            return complete;
        }


        /// \brief A method that calculates the total cost on a contract
        /// \details <b>Details</b>
        /// A method that adds up all the triplines on the contract to get the total cost of the contract
        /// \param - truck - <b>Truck</b> - The truck the triplines were delivered on, this is used for the rates of the carrier who owns the truck
        /// \returns - <b>Nothing</b>
        /// 
        public void CalculateCost(Truck truck)
        {
            double totalCost = 0.0;
            foreach(TripLine t in Trips)
            {
                totalCost = t.CalculateCost(this, truck);
            }
            Cost = totalCost;
        }


        /// \brief Determines if a contract is eastbound or westbound
        /// \details <b>Details</b>
        /// Gets the route IDs for the origin and destination city and subtracts one from the other.  If it's negative 
        /// the route is east bound, it's positive, west bound
        /// 
        /// \param - details - <b>List<string></b> - A list of contract details retrieved from the contract marketplace database
        /// \returns - <b>Nothing</b>
        private void GetEastOrWest()
        {
            List<string> field = new List<string>();
            field.Add("routeID");
            Dictionary<string, string> conditions = new Dictionary<string, string>();
            conditions.Add("destCity", Origin);
            Controller.TMS.MakeSelectCommand(field,"route",conditions, null);
            List<string> results = Controller.TMS.ExecuteCommand();
            int originID;
            int.TryParse(results[0], out originID);
            field = new List<string>();
            field.Add("routeID");
            conditions = new Dictionary<string, string>();
            conditions.Add("destCity", Destination);
            Controller.TMS.MakeSelectCommand(field, "route", conditions, null);
            results = Controller.TMS.ExecuteCommand();
            int destID;
            int.TryParse(results[0], out destID);
            int isEast = (originID - destID);
            if (isEast < 0)
            {
                Direction = "East";
            }
            else
            {
                Direction = "West";
            }
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
            contract.AppendFormat("{0},{1},{2},{3},{4},{5},{6},{7}",ID, ClientName, tmpJobType, Quantity, Origin, Destination, tmpVanType, Status);
            return contract.ToString();
        }
    }
}
