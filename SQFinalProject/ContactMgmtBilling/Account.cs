//*********************************************
// File			 : Account.cs
// Project		 : PROG2020 - Term Project
// Programmer	 : Nick Byam, Chris Lemon, Deric Kruse, Mark Fraser
// Last Change   : 2020-11-25
// Description	 : A class used to hold account details for existing or new customers, includes a list of their existing contracts, and
//				 : their total balance owed.
//*********************************************


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQFinalProject.ContactMgmtBilling
{
    /// 
    /// \class <b>Account</b>
    ///
    /// \brief The Purpose of this class is to be a central area to hold exisiting customer details and their contracts.
    ///
    /// \author <i>Nick Byam</i>
    ///
    public class Account
    {
        //! Properties
        private Dictionary<int, ContractDetails> Contracts; //< A collection of all the contracts associated with an account
        private Dictionary<int, ContractDetails> UncalculatedContracts;//< A collection of all contracts whose cost has not yet been calculated
        public double Balance { get; set; } //< The total balance owed on an account
        public string AccountName { get; set; }
        public int AccountID { get; set; }


        /// \brief A constructor for the account class
        /// \ details <b>Details</b>
        /// One constructor for the Account class that initializes the Contract dictionary and UncalculatedContracts dictionary. Sets the balance on the account to 0.
        /// \param - <b>Nothing</b>
        /// \returns - <b>Nothing</b>
        public Account()
        {
            Contracts = new Dictionary<int, ContractDetails>();
            UncalculatedContracts = new Dictionary<int, ContractDetails>();
            Balance = 0.00;
        }


        /// \brief A constructor for Account class which takes a Contract
        /// \ details <b>Details</b>
        /// Instantiates an Account class with a contract, and adds that contract to the Contracts list. It also attempts to calculate and
        /// Add a payable balance to the account, but if the balance can't be calculated, the contract is added to the UncalculatedContracts
        /// dictionary.
        /// \param - contract - <b>ContractDetails</b> - The class containing all of the details relating to the contract from the marketplace
        /// \returns - <b>Nothing</b>
        public Account(ContractDetails contract)
        {
            AccountName = contract.ClientName;
            Contracts = new Dictionary<int, ContractDetails>();
            UncalculatedContracts = new Dictionary<int, ContractDetails>();
            AddNewContract(contract.ID, contract);
        }


        /// \brief a constructor for the account class that accepts a list of contracts
        /// \ details <b>Details</b>
        /// Instantiate an Account class with a list of contracts to add to it. Adds all contracts and attempts to calculate and add a payable
        /// balance for eac contract to the account. For each contract where a balance can't be calculated, it is put in the 
        /// UncalculatedContract dictionary.
        /// \param - contracts - <b>List<ContractDetails></b> - A list of contracts to be added to the same account.
        /// \returns - <b>Nothing</b>
        public Account(List<ContractDetails> contracts)
        {
            AccountName = contracts[0].ClientName;
            Contracts = new Dictionary<int, ContractDetails>();
            UncalculatedContracts = new Dictionary<int, ContractDetails>();
            foreach (ContractDetails contract in contracts)
            {
                AddNewContract(contract.ID, contract);
            }
        }


        /// \brief A method to add a new Contract into the Contracts dictionary
        /// \ details <b>Details</b>
        /// Add a new contract into the Contracts dictionary using the Contract's ID as the Key, and the contract class as the Value
        /// \param - contractID - <b>int</b> - The contract's unique id number
        /// \param - contract - <b>ContractDetails</b> - The class containing all the details relating to the contract from the marketplace
        /// \returns - <b>Nothing</b>
        public void AddNewContract(int contractID, ContractDetails contract)
        {
            Contracts.Add(contractID, contract);
            if(this.AccountName == "")
            {
                AccountName = contract.ClientName;
            }
            AddBalance(contract);
        }


        /// \brief A method to return all contracts on one account as a list
        /// \ details <b>Details</b>
        /// A method which iterates through the Account's Contracts dictionary and adds each one to a List of strings. This utilizes the 
        /// The ContractDetails' overridden ToString method so that each string entry in the list is meaningful.
        /// \param - <b>Nothing</b>
        /// \returns - contractList <b>List<String></b> - A list containing all contracts expressed as strings
        public List<string> GetAllContracts()
        {
            List<string> contractList = new List<string>();

            foreach(KeyValuePair<int, ContractDetails> entry in Contracts)
            {
                contractList.Add(entry.Value.ToString());
            }

            return contractList;
        }


        /// \brief 
        /// \ details <b>Details</b>
        /// 
        /// \param - 
        /// \returns - 
        public List<string> GetUncalcContracts()
        {
            List<string> contracts = new List<string>();

            foreach(KeyValuePair<int, ContractDetails> entry in UncalculatedContracts)
            {
                contracts.Add(entry.Value.ToString());
            }

            return contracts;
        }


        /// \brief Calculates the Balance owed on an account based on contract details
        /// \ details <b>Details</b>
        /// This method calculates the balance to be added to an account based on factors of a contract including distance, rate, truck type
        /// and quantity of pallets. If the balance cannot be calculated, the contract is added into another list for contracts whose balance
        /// has not yet been calculated.
        /// \param - contract - <b>ContractDetails</b> - The contract to be evaluated to find its cost.
        /// \returns - <b>Nothing</b>
        public void AddBalance(ContractDetails contract)
        {
            if(contract.Cost > 0.0) // make sure a basic cost has been calculated for the contract first
            {
                if (contract.VanType == 0) // the van type is a Dry Van, no upcharge associated
                {
                    if (contract.Quantity == 0) // the trip is FTL
                    {
                        Balance += AddBalance(contract.Rate, contract.Distance);
                    }
                    else //the trip is LTL
                    {
                        Balance += AddBalance(contract.Rate, contract.Distance, contract.Quantity);
                    }
                }
                else // The van type is a Reefer Van there is an associate upcharge
                {
                    if (contract.Quantity == 0) // FTL in a Reefer van
                    {
                        double tmpBal = AddBalance(contract.Rate, contract.Distance);
                        tmpBal *= ContractDetails.ReeferUpCharge;
                        Balance += tmpBal;
                    }
                    else // LTL in a Reefer van
                    {
                        double tmpBal = AddBalance(contract.Rate, contract.Distance, contract.Quantity);
                        tmpBal *= ContractDetails.ReeferUpCharge;
                        Balance += tmpBal;
                    }
                }
            }
            else // flat cost not assessed yet, add the contract to the UncalculatedContracts dictionary to be assessed at a later time.
            {
                UncalculatedContracts.Add(contract.ID, contract);
            }
        }


        /// \brief Calculates and returns a balance
        /// \ details <b>Details</b>
        /// An overridden method that calculates balance due for a contract based on the FTL rate and distance of the trip.
        /// \param - rate - <b>double</b> - The rate set by a company offering a FTL delivery service
        /// \param - distance - <b>double</b> - The distance of the trip
        /// \returns - balance <b>double</b> - The balance calculated from the rate and distance
        private double AddBalance(double rate, double distance)
        {
            double balance = rate * distance * ContractDetails.FTLUpCharge;
            return balance;
        }


        /// \brief Calculates and returns a balance
        /// \ details <b>Details</b>
        /// An overridden method that calculates the balance due for a contract based on the LTL rate, the distance of the trip, and the
        /// quantity of the pallets delivered.
        /// \param - rate - <b>double</b> - The rate set by a company offering a LTL delivery service
        /// \param - distance - <b>double</b> - The distance of the trip
        /// \param - quantity - <b>int</b> - The amount of pallets to be delivered
        /// \returns - 
        private double AddBalance(double rate, double distance, int quantity)
        {
            double balance = rate * distance * (double)quantity * ContractDetails.LTLUpCharge;
            return balance;
        }
    }
}
