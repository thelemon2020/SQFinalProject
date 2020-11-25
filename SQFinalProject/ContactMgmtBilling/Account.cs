using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQFinalProject.ContactMgmtBilling
{
    public class Account
    {
        public ContractDetails Contract { get; set; }
        public double Balance { get; set; }

        public Account(ContractDetails contract)
        {
            Contract = contract;
            Balance = 0.00;
        }
        

        public void AddBalance(double rate, double distance,int quantity=0, int vanType=0)
        {
            if(Contract.Distance > 0)
            {
                if (vanType == 0)
                {
                    if (quantity == 0)
                    {
                        Balance += AddBalance(rate, distance);
                    }
                    else
                    {
                        Balance += AddBalance(rate, distance, quantity);
                    }
                }
                else
                {
                    if (quantity == 0)
                    {
                        double tmpBal = AddBalance(rate, distance);
                        tmpBal *= ContractDetails.ReeferUpCharge;
                        Balance += tmpBal;
                    }
                    else
                    {
                        double tmpBal = AddBalance(rate, distance, quantity);
                        tmpBal *= ContractDetails.ReeferUpCharge;
                        Balance += tmpBal;
                    }
                }
            }
            else
            {
                throw new ArgumentException("Required Field Distance Has Not Been Calculated");
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

        public void GenerateInvoice()
        {

        }
    }
}
