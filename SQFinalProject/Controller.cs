using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQFinalProject.ContactMgmtBilling;

namespace SQFinalProject
{
    public static class Controller
    {
        public static Account CreateNewAccount()
        {
            Account account = new Account();
            return account;
        }

        public static Account CreateNewAccount(ContractDetails contract)
        {
            Account account = new Account(contract);
            return account;
        }

        public static Account CreateNewAccount(List<ContractDetails> contracts)
        {
            Account account = new Account(contracts);
            return account;
        }

        public static void AddTripToAccount(Account account)
        {

        }

        public static void GenerateInvoice(Account account)
        {
            // need to generate a report 
        }

        public static void GenerateReport(int weeks=0)
        {
            // generate either a 2 week or all time report
        }
    }
}
