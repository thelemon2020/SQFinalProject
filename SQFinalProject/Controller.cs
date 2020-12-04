using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQFinalProject.ContactMgmtBilling;

namespace SQFinalProject
{
    ///
    /// \class Controller
    ///
    /// \brief A class that handles passing data around the system
    /// In this class, the errors will be handled by returning a specified error value, either zero or null, and the calling class will then 
    /// have to decide how to best handle the error.  Testing for the methods in this class will be handled in a separate unit test project.
    /// 
    /// \author <i>Nick Byam</i>
    public static class Controller
    {
        static public Database TMS { get; set; }

        //  METHOD:		CreateNewAccount
        /// \brief This method creates a new customer account with no parameters.
        /// \details <b>Details</b>
        /// This method creates a new empty customer account given no parameters
        /// \param - <b>None</b>
        /// 
        /// \return - The new Account object that was created
        /// 
        public static Account CreateNewAccount()
        {
            Account account = new Account();
            return account;
        }


        
        //  METHOD:		CreateNewAccount
        /// \brief This method creates a new customer account with a contract as a parameter.
        /// \details <b>Details</b>
        /// This method creates a new customer account given a contract as a parameter.
        /// \param - <b>contract:</b>  the Contract object to add to the new account
        /// 
        /// \return - The new Account object that was created
        /// 
        public static Account CreateNewAccount(Contract contract)
        {
            Account account = new Account(contract);
            return account;
        }


        
        //  METHOD:		CreateNewAccount
        /// \brief This method creates a new customer account with a list of contract objects as a parameter.
        /// \details <b>Details</b>
        /// This method creates a new customer account given a list of contracts currently meant for the account.
        /// \param - <b>contracts:</b>  the list of Contract objects to initialize the new account with
        /// 
        /// \return - The new Account object that was created
        /// 
        public static Account CreateNewAccount(List<Contract> contracts)
        {
            Account account = new Account(contracts);
            return account;
        }


        
        //  METHOD:		AddContractToAccount
        /// \brief This method adds a contract to a given account taking both as parameters.
        /// \details <b>Details</b>
        /// This method takes a Contract as a parameter and adds that Contract to an Account, also given by a parameter.
        /// \param - <b>account:</b>    the Account to add the Contract to
        /// \param - <b>contract:</b>   the Contract object to add to the Account
        /// 
        /// \return - void
        /// 
        public static void AddContractToAccount(Account account, Contract contract)
        {
            account.AddNewContract(contract.ID, contract);
        }


        /// \brief A method that creates a new contract
        /// \details <b>Details</b>
        /// A method that allows the controller to create a new contract, but not associate it with a class yet
        /// \param - details - <b>List<string></b> - A List of strings gathered from the Contract database
        /// \returns - contract - <b>Contract</b> - The newly created contract
        /// 
        /// \see AddContractToAccount(Account account, Contract contract)
        public static Contract CreateNewContract(List<string> details)
        {
            Contract contract = new Contract(details);
            return contract;
        }

        /// \brief A method to grab all contracts available on the contract marketplace
        /// \details <b>Details</b>
        /// A method that returns all entries in the contract marketplace as a List of strings
        /// \param - mrktPlace - <b>Database</b> - The database to connect to
        /// \returns - sqlreturn - <b>List<string, string></b> - a list of results from the database query
        /// 
        /// \see SelectContract(Database mrktPlace, Dictionary<string, string> conditions)
        public static List<string> GetAllContractsFromDB(Database mrktPlace)
        {
            List<string> fields = new List<string>();
            fields.Add("*");
            string table = "Contract";
            mrktPlace.MakeSelectCommand(fields, table, null, null);
            List<string> sqlReturn = new List<string>();
            sqlReturn = mrktPlace.ExecuteCommand();
            return sqlReturn;
        }

        /// \brief A method to grab a city from the TMS database
        /// \details <b>Details</b>
        /// A method that returns a city from the routes table, specified by the conditions parameter
        /// \param - tms - <b>Database</b> - The database to connect to
        /// \param - conditions - <b>Dictionary<string, string></b> - A Dictionary key-value pair where the key is a column name and the value is a city
        /// \returns - sqlreturn - <b>List<string, string></b> - a list of results from the database query
        /// 
        /// \see SelectContract(Database mrktPlace, Dictionary<string, string> conditions)
        public static List<string> GetCityFromDB(Database tms, Dictionary<string, string> conditions)
        {
            List<string> fields = new List<string>();
            fields.Add("*");
            string table = "Route";
            tms.MakeSelectCommand(fields, table, conditions, null);
            //List<string> sqlReturn = new List<string>();
            List<string> sqlReturn = tms.ExecuteCommand();
            return sqlReturn;
        }

        /// \brief A method that grabs a contract from the market place with conditions
        /// \details <b>Details</b>
        /// A method that can grab contracts from the marketplace delimited by conditions
        /// \param - mrktPlace - <b>Database</b> - the database to connect to
        /// \param - conditions - <b>Dictionary<string, string></b> - The dictionary containing key value pairs of conditions
        /// \returns - sqlReturn - <b>List<string></b> - The results returned from the query as a list of strings
        /// 
        /// \see GetAllContractsFromDB(Database mrktPlace)
        public static List<string> SelectContract(Database mrktPlace, Dictionary<string, string> conditions)
        {
            List<string> fields = new List<string>();
            fields.Add("*");
            string table = "Contract";
            mrktPlace.MakeSelectCommand(fields, table, conditions, null);
            List<string> sqlReturn = mrktPlace.ExecuteCommand();
            return sqlReturn;
        }


        /// \brief A method that is used to add an account to the tms database
        /// \details <b>Details</b>
        /// A method which takes a single account, breaks down its values into string, and adds them into a list to be
        /// added to the Account table in the TMS database.
        /// \param - tmsDB - <b>Database</b> - The TMS database object used to access the tms database and build a command to insert and account
        /// \param - ac - <b>Account</b> - The account to be added to the Account table in the tms database
        /// \returns - <b>Nothing</b>
        /// 
        /// \see AddAccountToTMS(Database tmsDB, List<Account> accounts)
        public static void AddAccountToTMS(Database tmsDB, Account ac)
        {
            List<string> values = new List<string>();
            string acStr = ac.ToString(); // get the string representation of the account
            string[] tmp = acStr.Split(','); // break it down into individual values

            foreach (string s in tmp) // add each value to a list
            {
                values.Add(s);
            }
            tmsDB.MakeInsertCommand("Account", values); // make the isnert command
            tmsDB.ExecuteCommand(); // execute said command
        }


        /// \brief An overload of the AddAccountToTMS That takes a list of accounts
        /// \details <b>Details</b>
        /// A method that calls upon the other add account to TMS method to add multiple accounts to the database.
        /// \param - tmsDB - <b>Database</b> - The database object that grants access to the TMS database
        /// \param - accounts - <b>List<Account></b> - A list of accounts to be added to the TMS database
        /// \returns - <b>Nothing</b>
        /// 
        /// \see AddAccountToTMS(Database tmsDB, Account ac)
        public static void AddAccountToTMS(Database tmsDB, List<Account> accounts)
        {
            foreach(Account ac in accounts) //Add all accounts in the list to the database.
            {
                AddAccountToTMS(tmsDB, ac);
            }
        }


        /// \brief A method that gets contracts from the database
        /// \details <b>Details</b>
        /// A method which is used to get contracts from the TMS database. By passing null to the conditions all accounts will be selected
        /// but conditions can be added to narrow the selection.
        /// \param - tmsDB - <b>Database</b> - The database object used to connect to the tms database
        /// \param - values - <b>List<string></b> - The column values the user specifically wants to see 
        /// \param - conditions - <b>Dictionary<string, string></b> - The dictionary of key value pairs to be used as search conditions
        /// \returns - sqlReturn - <b>List<string></b> - The results of the Selection
        /// 
        /// \see SelectContract(Database mrktPlace, Dictionary<string, string> conditions)
        /// \see GetAllContractsFromDB(Database mrktPlace)
        public static List<string> GetAccountsFromTMS(Database tmsDB, List<string> values, Dictionary<string, string> conditions)
        {
            List<string> fields = new List<string>();
            if (values != null)
            {
                fields = values;
            }
            else
            {
                fields.Add("*");
            }
            string table = "Account";
            tmsDB.MakeSelectCommand(fields, table, conditions, null); // this can be not null if necessary
            List<string> sqlReturn = tmsDB.ExecuteCommand();
            return sqlReturn;
        }


        /// \brief A method to get carriers from the tms database
        /// \details <b>Details</b>
        /// A method which returns the carriers in the tms database, the fields shown can be specified by the user, and can be conditional
        /// \param - tmsDB - <b>Database</b> - The database object which grants access to the tms database
        /// \param - values - <b>List<string></b> - The column values the user specifically wants to see 
        /// \param - condtions - <b>Dictionary<string, string></b> - The key value pairs that define the conditions
        /// \returns - sqlReturn - <b>List<string></b> - A list of the results generated from the database query.
        /// 
        public static List<string> GetCarriersFromTMS(Database tmsDB, List<string> values, Dictionary<string, string> conditions)
        {
            List<string> fields = new List<string>();
            if(values != null)
            {
                fields = values;
            }
            else
            {
                fields.Add("*");
            }
            string table = "carrier";
            tmsDB.MakeSelectCommand(fields, table, conditions, null);
            List<string> sqlReturn = tmsDB.ExecuteCommand();
            return sqlReturn;
        }


        /// \brief 
        /// \details <b>Details</b>
        /// 
        /// \param - 
        /// \returns - 
        /// 
        /// \see
        public static List<string> GetCarrierDepotCities(Database tmsDB, List<string> values, Dictionary<string, string> conditions)
        {
            List<string> fields = new List<string>();
            List<string> tables = new List<string>();
            List<string> ids = new List<string>();
            if(values != null)
            {
                fields = values;
            }
            else
            {
                fields.Add("*");
            }
            tables.Add("carrier");
            tables.Add("depot");
            ids.Add("carrierid");
            tmsDB.MakeInnerJoinSelect(fields, tables, ids, conditions);
            List<string> sqlReturn = tmsDB.ExecuteCommand();
            return sqlReturn;
        }


        /// \brief 
        /// \details <b>Details</b>
        /// 
        /// \param - 
        /// \returns - 
        /// 
        /// \see
        public static List<Carrier> SetupCarriers(Database tmsDB)
        {
            List<Carrier> carriers = new List<Carrier>();
            List<string> carrierDetails = GetCarriersFromTMS(tmsDB, null, null); // get all the carriers in the db

            foreach(string row in carrierDetails) // instantiate all the the carrier classes with details from the db
            {
                string[] tmpSplit = row.Split(',');
                List<string> tmpList = tmpSplit.ToList();
                carriers.Add(new Carrier(tmpList));
            }

            List<string> fields = new List<string>();
            fields.Add("carrierName");
            fields.Add("depotCity");
            fields.Add("FTLA");
            fields.Add("LTLA");
            List<string> depotCities = GetCarrierDepotCities(tmsDB, fields, null); // do an inner join on the carrier table and depot table

            foreach(Carrier c in carriers)
            {
                foreach(string row in depotCities)
                {
                    string[] tmpSplit = row.Split(',');
                    if(tmpSplit[0] == c.CarrierName)
                    {
                        c.DepotCities.Add(tmpSplit[1]);
                        c.FTLA = int.Parse(tmpSplit[2]); // adding the availability could likely use some work. this should ideally be
                        c.LTLA = int.Parse(tmpSplit[3]); // added for every depot city rather than just to the carrier as a whole.
                    }
                }
            }

            return carriers;
        }


        /// \brief 
        /// \details <b>Details</b>
        /// 
        /// \param - 
        /// \returns - 
        /// 
        /// \see
        public static List<string> FindCarriersForContract(Contract contract, List<Carrier> carriers)
        {
            List<string> possibleCarrier = new List<string>();

            foreach(Carrier carrier in carriers) //iterate through all carriers, they should already be setup
            {
                foreach(string city in carrier.DepotCities) // for one carrier, check through all their depot cities to see if one matches
                {                                           // the contract origin city
                    if(contract.Origin == city) // if the contract origin city matches one of the depot cities of a carrier we check next
                    {                           // to see if they have availability of trucks matching a job type.
                        if(contract.JobType == 0) // FTL job
                        {
                            if(carrier.FTLA > 0) // if the carrier has availability, add them to the list of possible carriers
                            {
                                string tmp = carrier.CarrierName + "," + city;
                                possibleCarrier.Add(tmp);
                            }
                        }
                        else // LTL job
                        {
                            if(carrier.LTLA > 0) // if the carrier has an LTL truck available, add them to list of possible carriers for the job
                            {
                                string tmp = carrier.CarrierName + "," + city;
                                possibleCarrier.Add(tmp);
                            }
                        }
                    }
                }
            }

            return possibleCarrier;

        }


        /// \brief A method which creates a contract specific invoice
        /// \details <b>Details</b>
        /// A method which will create an invoice on a contract containing the cost, and other particulars
        /// \param - account - <b>Account</b> - The account contains the contracts to generate invoices on
        /// \returns - <b>Nothing</b>
        /// 
        public static string GenerateInvoice(Account account, Contract contract)
        {
            if (!contract.TripComplete)
            {
                return null;
            }
            else
            {
                string date = DateTime.Now.ToString();
                string invoiceID = account.AccountID.ToString() + contract.ID.ToString();
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("{0},{1},{2},{3},{4},{5},{6},{7}", invoiceID, contract.ID, account.AccountID, date, contract.Cost, contract.JobType, contract.Quantity, contract.VanType);

                account.Invoices.Add(sb.ToString());
                return sb.ToString();
            }
        }

        /// \brief A method to generate a report of company earnings and total contracts
        /// \details <b>Details</b>
        /// A method that generates either a 2 week report of contracts and earnings, or an all time report of contracts and earnings
        /// \param - weeks - <b>int</b> - The number of weeks for report generation. Must be either 2 or 0
        /// \returns - <b>Nothing</b>
        /// 
        public static string GenerateReport(Database tmsDB, int weeks=0)
        {
            string start = "";
            string end = "";
            if(weeks == 0) // this will be an "all-time" reports
            {
                start = DateTime.MinValue.ToString();
                end = DateTime.MaxValue.ToString();
            }
            else
            {
                end = DateTime.Now.AddDays(1.0).ToString();
                start = DateTime.Now.Subtract(TimeSpan.FromDays(14)).ToString();
            }
            List<string> fields = new List<string>();
            fields.Add("cost");
            Dictionary<string, string> searchPoints = new Dictionary<string, string>();
            searchPoints.Add("invoiceDate", start);
            searchPoints.Add("invoiceDate", end);
            string table = "invoice";

            tmsDB.MakeBetweenSelect(fields, table, searchPoints);

            List<string> sqlReturn = tmsDB.ExecuteCommand();

            double totalCost = 0.00;
            int invoiceCount = sqlReturn.Count;

            foreach(string row in sqlReturn)
            {
                totalCost += double.Parse(row);
            }
            StringBuilder sb = new StringBuilder();
            sb.Append("TMS Internal Report:\n\n");
            if (weeks == 0)
            {
                sb.Append("Period: All-Time, ");
            }
            else
            {
                sb.AppendFormat("Period: {0} - {1}, ", start, end);
            }
            sb.AppendFormat("Total Contracts Delivered: {0}, Total Invoice Cost: {1}", invoiceCount, totalCost);

            return sb.ToString();
        }

        public static int GetLastTripID(Database tmsDB)
        {
            List<string> fields = new List<string>();
            fields.Add("MAX(TripID)");
            string table = "Trip";
            tmsDB.MakeSelectCommand(fields, table, null, null);
            List<string> LastTripID = tmsDB.ExecuteCommand();

            return int.Parse(LastTripID.First());
        }
    }
}
