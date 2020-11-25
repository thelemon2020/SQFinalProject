using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQFinalProject.ContactMgmtBilling
{
    public class Account
    {
        private Dictionary<int, ContractDetails> Contracts;
        private Dictionary<int, ContractDetails> UncalculatedContracts;
        public double Balance { get; set; }

        public Account()
        {
            Contracts = new Dictionary<int, ContractDetails>();
            UncalculatedContracts = new Dictionary<int, ContractDetails>();
            Balance = 0.00;
        }

        public Account(ContractDetails contract)
        {
            Contracts = new Dictionary<int, ContractDetails>();
            UncalculatedContracts = new Dictionary<int, ContractDetails>();
            AddNewContract(contract.ID, contract);
            AddBalance(contract);
        }

        public Account(List<ContractDetails> contracts)
        {
            Contracts = new Dictionary<int, ContractDetails>();
            UncalculatedContracts = new Dictionary<int, ContractDetails>();
            foreach (ContractDetails contract in contracts)
            {
                AddNewContract(contract.ID, contract);
                AddBalance(contract);
            }
        }


        public void AddNewContract(int tripID, ContractDetails contract)
        {
            Contracts.Add(tripID, contract);
        }

        public List<string> GetAllContracts()
        {
            List<string> contractList = new List<string>();

            foreach(KeyValuePair<int, ContractDetails> entry in Contracts)
            {
                contractList.Add(entry.Value.ToString());
            }

            return contractList;
        }
        

        public void AddBalance(ContractDetails contract)
        {
            if(contract.Cost > 0.0)
            {
                if (contract.VanType == 0)
                {
                    if (contract.Quantity == 0)
                    {
                        Balance += AddBalance(contract.Rate, contract.Distance);
                    }
                    else
                    {
                        Balance += AddBalance(contract.Rate, contract.Distance, contract.Quantity);
                    }
                }
                else
                {
                    if (contract.Quantity == 0)
                    {
                        double tmpBal = AddBalance(contract.Rate, contract.Distance);
                        tmpBal *= ContractDetails.ReeferUpCharge;
                        Balance += tmpBal;
                    }
                    else
                    {
                        double tmpBal = AddBalance(contract.Rate, contract.Distance, contract.Quantity);
                        tmpBal *= ContractDetails.ReeferUpCharge;
                        Balance += tmpBal;
                    }
                }
            }
            else
            {
                UncalculatedContracts.Add(contract.ID, contract);
            }
        }

        private double AddBalance(double rate, double distance)
        {
            double balance = rate * distance * ContractDetails.FTLUpCharge;
            return balance;
        }


        private double AddBalance(double rate, double distance, int quantity)
        {
            double balance = rate * distance * (double)quantity * ContractDetails.LTLUpCharge;
            return balance;
        }
    }
}
