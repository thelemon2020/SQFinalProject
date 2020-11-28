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
        //  METHOD:		CreateNewAccount
        /// \brief This method creates a new customer account with no parameters.
        /// \details <b>Details</b>
        /// This method creates a new empty constructor given no parameters
        /// \param - <b>None</b>
        /// 
        /// \return - The new Account object that was created
        /// 
        public static Account CreateNewAccount()
        {
            Account account = new Account();
            return account;
        }



        //  METHOD:		LoadConfig
        /// \brief Loads the database connection details from an external config file
        /// \details <b>Details</b>
        /// Checks to see if the config files exists and creates it if it doesn't.  If it does, the method reads from the file 
        /// and parses it out into data that is usable to connect to one or more databases
        /// \param - <b>None</b>
        /// 
        /// \return - <b>Nothing</b>
        /// 
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


        public static List<string> SelectContract(Database mrktPlace, Dictionary<string, string> conditions)
        {
            List<string> fields = new List<string>();
            fields.Add("*");
            string table = "Contract";
            mrktPlace.MakeSelectCommand(fields, table, conditions);
            List<string> sqlReturn = mrktPlace.ExecuteCommand();
            return sqlReturn;
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
