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
    /// \class Account
    ///
    /// \brief The Purpose of this class is to be a central area to hold exisiting customer details and their contracts.
    /// Error handling in the account class is minimal other than checking to make sure Properties are filled before attempting to use them
    /// There is nowhere in this code that would throw exceptions.
    ///
    /// \author <i>Nick Byam</i>
    ///
    public class Account
    {
        //! Properties
        private Dictionary<int, Contract> Contracts; //!< A collection of all the contracts associated with an account
        private Dictionary<int, Contract> UncalculatedContracts;//!< A collection of all contracts whose cost has not yet been calculated
        public List<string> Invoices { get; set; } //!< A collection of all the invoices related to the account
        public double Balance { get; set; } //!< The total balance owed on an account
        public string AccountName { get; set; } //!< The name of the account
        public int AccountID { get; set; } //!< Th Id number of the account
        public DateTime LastPayment { get; set; }


        /// \brief A constructor for the account class
        /// \details <b>Details</b>
        /// One constructor for the Account class that initializes the Contract dictionary and UncalculatedContracts dictionary. Sets the balance on the account to 0.
        /// 
        /// \param - <b>Nothing</b>
        /// \returns - <b>Nothing</b>
        /// 
        /// \see Account(Contract contract)
        /// \see Account(List<Contract> contracts)
        public Account()
        {
            Contracts = new Dictionary<int, Contract>();
            UncalculatedContracts = new Dictionary<int, Contract>();
            Balance = 0.00;
        }


        /// \brief A constructor for Account class which takes a Contract
        /// \details <b>Details</b>
        /// Instantiates an Account class with a contract, and adds that contract to the Contracts list. It also attempts to calculate and
        /// Add a payable balance to the account, but if the balance can't be calculated, the contract is added to the UncalculatedContracts
        /// dictionary.
        /// \param - contract - <b>ContractDetails</b> - The class containing all of the details relating to the contract from the marketplace
        /// \returns - <b>Nothing</b>
        /// 
        /// \see Account()
        /// \see Account(List<Contract> contracts)
        public Account(Contract contract)
        {
            AccountName = contract.ClientName;
            Contracts = new Dictionary<int, Contract>();
            UncalculatedContracts = new Dictionary<int, Contract>();
            AddNewContract(contract.ID, contract);
        }


        /// \brief a constructor for the account class that accepts a list of contracts
        /// \details <b>Details</b>
        /// Instantiate an Account class with a list of contracts to add to it. Adds all contracts and attempts to calculate and add a payable
        /// balance for eac contract to the account. For each contract where a balance can't be calculated, it is put in the 
        /// UncalculatedContract dictionary.
        /// \param - contracts - <b>List<ContractDetails></b> - A list of contracts to be added to the same account.
        /// \returns - <b>Nothing</b>
        /// 
        /// \see Account(Contract contract)
        /// \see Account()
        public Account(List<Contract> contracts)
        {
            AccountName = contracts[0].ClientName;
            Contracts = new Dictionary<int, Contract>();
            UncalculatedContracts = new Dictionary<int, Contract>();
            foreach (Contract contract in contracts)
            {
                AddNewContract(contract.ID, contract);
            }
        }


        /// \brief A method to add a new Contract into the Contracts dictionary
        /// \details <b>Details</b>
        /// Add a new contract into the Contracts dictionary using the Contract's ID as the Key, and the contract class as the Value
        /// \param - contractID - <b>int</b> - The contract's unique id number
        /// \param - contract - <b>ContractDetails</b> - The class containing all the details relating to the contract from the marketplace
        /// \returns - <b>bool</b> - either true if the contract was added, or false if it wasn't
        /// 
        public bool AddNewContract(int contractID, Contract contract)
        {
            if(AccountName == "" || contract.ClientName == AccountName)
            {
                AccountName = contract.ClientName;
                Contracts.Add(contractID, contract);
                AddBalance(contract);
            }
            else if(contract.ClientName != AccountName)
            {
                return false;
            }
            return true;
        }


        /// \brief A method to return all contracts on one account as a list
        /// \details <b>Details</b>
        /// A method which iterates through the Account's Contracts dictionary and adds each one to a List of strings. This utilizes the 
        /// The ContractDetails' overridden ToString method so that each string entry in the list is meaningful.
        /// \param - <b>Nothing</b>
        /// \returns - contractList <b>List<String></b> - A list containing all contracts expressed as strings
        /// 
        /// \see GetUncalcContracts()
        public List<string> GetAllContracts()
        {
            List<string> contractList = new List<string>();

            foreach(KeyValuePair<int, Contract> entry in Contracts)
            {
                contractList.Add(entry.Value.ToString());
            }

            return contractList;
        }


        /// \brief A method that returns a list of uncalculated contracts
        /// \details <b>Details</b>
        /// A Method that accesses the UncalculatedContracts Dictionary, and adds each entry to a List in its string expression
        /// \param - <b>Nothing</b>
        /// \returns - contracts - <b>List<string></b> - The list of uncalculated contracts in their string expression
        /// 
        /// \see GetAllContracts()
        public List<string> GetUncalcContracts()
        {
            List<string> contracts = new List<string>();

            foreach(KeyValuePair<int, Contract> entry in UncalculatedContracts)
            {
                contracts.Add(entry.Value.ToString());
            }

            return contracts;
        }


        /// \brief Calculates the Balance owed on an account based on contract details
        /// \details <b>Details</b>
        /// This method calculates the balance to be added to an account based on the existing calculated cost on a contract, multiplied
        /// by the set company rate on the type of trip being made.
        /// \param - contract - <b>ContractDetails</b> - The contract to be evaluated to find its cost.
        /// \returns - <b>Nothing</b>
        /// 
        public void AddBalance(Contract contract)
        {
            if(contract.Cost != 0.00)
            {
                // First apply upcharge to current cost on Contract
                if (contract.JobType == 0) //FTL
                {
                    contract.Cost *= Contract.FTLUpCharge;
                }
                else
                {
                    contract.Cost *= Contract.LTLUpCharge;
                }
                // update the Balance
                Balance += contract.Cost;
            }
            else
            {
                UncalculatedContracts.Add(contract.ID, contract);
            }
            
        }


        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0},{1},{2},{3}", AccountID, AccountName, Balance, LastPayment);
            return sb.ToString();
        }
    }
}
