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

        public static Account CreateNewAccount(Contract contract)
        {
            Account account = new Account(contract);
            return account;
        }

        public static Account CreateNewAccount(List<Contract> contracts)
        {
            Account account = new Account(contracts);
            return account;
        }

        public static void AddContractToAccount(Account account, Contract contract)
        {
            account.AddNewContract(contract.ID, contract);
        }

        public static Contract CreateNewContract(string name, int job, int quant, string origin, string dest, int van)
        {
            Contract contract = new Contract(name, job, quant, origin, dest, van);
            return contract;
        }
        

        public static List<string> GetAllContractsFromDB(Database mrktPlace)
        {
            List<string> fields = new List<string>();
            fields.Add("*");
            string table = "Contract";
            mrktPlace.MakeSelectCommand(fields, table, null);
            List<string> sqlReturn = new List<string>();
            sqlReturn = mrktPlace.ExecuteCommand();
            return sqlReturn;
        }

        /// \brief A method that grabs a contract from the market place with conditions
        /// \details <b>Details</b>
        /// A method that can grab contracts from the marketplace delimited by conditions
        /// \param - 
        /// \returns - 
        /// 
        /// \see
        public static List<string> SelectContract(Database mrktPlace, Dictionary<string, string> conditions)
        {
            List<string> fields = new List<string>();
            fields.Add("*");
            string table = "Contract";
            mrktPlace.MakeSelectCommand(fields, table, conditions);
            List<string> sqlReturn = mrktPlace.ExecuteCommand();
            return sqlReturn;
        }


        /// \brief A method which creates a contract specific invoice
        /// \details <b>Details</b>
        /// A method which will create an invoice on a contract containing the cost, and other particulars
        /// \param - account - <b>Account</b> - The account contains the contracts to generate invoices on
        /// \returns - <b>Nothing</b>
        /// 
        public static void GenerateInvoice(Account account)
        {
            // need to generate a report 
        }

        /// \brief A method to generate a report of company earnings and total contracts
        /// \details <b>Details</b>
        /// A method that generates either a 2 week report of contracts and earnings, or an all time report of contracts and earnings
        /// \param - weeks - <b>int</b> - The number of weeks for report generation. Must be either 2 or 0
        /// \returns - <b>Nothing</b>
        /// 
        public static void GenerateReport(int weeks=0)
        {
            // generate either a 2 week or all time report
        }
    }
}
