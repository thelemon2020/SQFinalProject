using System;
using System.Collections.Generic;
using System.IO;
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
        public const string invoiceFilePath = @".\invoices\";
        public const string ConfigPath = @".\TMS.txt";   //<The path to the config file
        static public Database TMS { get; set; }                    //<The database object for the TMS database
        static public Database MarketPlace { get; set; }            //<The database object for the Marketplace database
        static public List<string> TMS_Database { get; set; }                  //<The the string list to store TMS DB connection info
        static public List<string> MarketPlace_Database { get; set; }          //<The the string list to store Marketplace DB connection info

        static public string DBBackUpPath { get; set; }

        //  METHOD:		LoadConfig
        /// \brief Loads the database connection details from an external config file
        /// \details <b>Details</b>
        /// Checks to see if the config files exists and creates it if it doesn't.  If it does, the method reads from the file
        /// and parses it out into data that is usable to connect to one or more databases
        /// \param - <b>None</b>
        ///
        /// \return - success - <b>bool</b> - true if config loaded databases correctly, false if error
        ///
        public static bool LoadConfig()
        {
            bool success = false;

            if (File.Exists(ConfigPath))                        // If the config file exists, try to read from it
            {
                StreamReader configFile = new StreamReader(ConfigPath);//open reader stream
                string contents = configFile.ReadToEnd(); // get string from config file
                configFile.Close();//close stream
                if (contents != "")
                {
                    string[] splitByDB = contents.Split('\n');//split string by line
                    foreach (string dbDetails in splitByDB)//iterate through string
                    {
                        string[] details = dbDetails.Split(' '); //split string into individual fields
                        details[1] = details[1].TrimEnd('\r');
                        if (details[0] == "TMS") //If the info is for the TMS database
                        {
                            List<string> TMS_Database = new List<string>();
                            for (int i = 1; i < details.Count(); i++) //iterate through parts of detials
                            {
                                TMS_Database.Add(details[i]);
                            }
                            TMS_Database[4] = TMS_Database[4].TrimEnd('\r');
                            TMS = new Database(TMS_Database[0], TMS_Database[1], TMS_Database[2], TMS_Database[3], TMS_Database[4]);

                            success = true;
                        }
                        else if (details[0] == "MP")//If the info is for the MarketPlace database
                        {
                            List<string> MarketPlace_Database = new List<string>();
                            for (int i = 1; i < details.Count(); i++)//iterate through parts of detials
                            {
                                MarketPlace_Database.Add(details[i]);
                            }
                            MarketPlace_Database[4] = MarketPlace_Database[4].TrimEnd('\r');
                            MarketPlace = new Database(MarketPlace_Database[0], MarketPlace_Database[1], MarketPlace_Database[2], MarketPlace_Database[3], MarketPlace_Database[4]);

                            success = true;
                        }
                        else if (details[0] == "BACKUP")
                        {
                            DBBackUpPath = details[1];
                            if (!Directory.Exists(DBBackUpPath))
                            {
                                Directory.CreateDirectory(DBBackUpPath);
                            }
                            if (!File.Exists(DBBackUpPath + "backup.sql"))
                            {
                                File.Create(DBBackUpPath + "backup.sql");
                            }
                            DBBackUpPath = DBBackUpPath + "backup.sql";
                        }
                        else if (details[0] == "LOGGER")
                        {
                            Logger.path = details[1];
                            if (!Directory.Exists(Logger.path))
                            {
                                Directory.CreateDirectory(Logger.path);
                            }
                            if (!File.Exists(Logger.path + "log.txt"))
                            {
                                File.Create(Logger.path + "log.txt");
                               
                            }
                            Logger.path = Logger.path + "log.txt";
                        }
                    }
                }
            }
            else //if file does not exist
            {
                FileStream newConfig = File.Create(ConfigPath); // create file
                newConfig.Close();//close file
            }

            return success;
        }

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
        public static Contract CreateNewContract(string details)
        {
            Contract contract = new Contract(details);
            return contract;
        }

        /// \brief A method to grab all contracts available on the contract marketplace
        /// \details <b>Details</b>
        /// A method that returns all entries in the contract marketplace as a List of strings
        /// \param - N/A
        /// \returns - sqlreturn - <b>List<string, string></b> - a list of results from the database query
        /// 
        /// \see SelectContract(Dictionary<string, string> conditions)
        public static List<string> GetAllContractsFromDB()
        {
            List<string> fields = new List<string>();
            fields.Add("*");
            string table = "Contract";
            MarketPlace.MakeSelectCommand(fields, table, null, null);
            List<string> sqlReturn = new List<string>();
            sqlReturn = MarketPlace.ExecuteCommand();
            return sqlReturn;
        }


        /// \brief A method to grab all contracts currently in the TMS database
        /// \details <b>Details</b>
        /// A method that returns all entries in the tms marketplace as a List of strings
        /// \param - N/A
        /// \returns - sqlreturn - <b>List<string, string></b> - a list of results from the database query
        /// 
        /// \see SelectContract(Dictionary<string, string> conditions)
        public static List<string> GetAllContractsFromTMS()
        {
            List<string> fields = new List<string>();
            fields.Add("*");
            string table = "Contract";
            TMS.MakeSelectCommand(fields, table, null, null);
            List<string> sqlReturn = new List<string>();
            sqlReturn = TMS.ExecuteCommand();
            return sqlReturn;
        }

        /// \brief A method to grab a city from the TMS database
        /// \details <b>Details</b>
        /// A method that returns a city from the routes table, specified by the conditions parameter
        /// \param - conditions - <b>Dictionary<string, string></b> - A Dictionary key-value pair where the key is a column name and the value is a city
        /// \returns - sqlreturn - <b>List<string, string></b> - a list of results from the database query
        /// 
        /// \see SelectContract(Dictionary<string, string> conditions)
        public static List<string> GetCityFromDB(Dictionary<string, string> conditions)
        {
            List<string> fields = new List<string>();
            fields.Add("*");
            string table = "Route";
            TMS.MakeSelectCommand(fields, table, conditions, null);
            //List<string> sqlReturn = new List<string>();
            List<string> sqlReturn = TMS.ExecuteCommand();
            return sqlReturn;
        }

        /// \brief A method that grabs a contract from the market place with conditions
        /// \details <b>Details</b>
        /// A method that can grab contracts from the marketplace delimited by conditions
        /// \param - conditions - <b>Dictionary<string, string></b> - The dictionary containing key value pairs of conditions
        /// \returns - sqlReturn - <b>List<string></b> - The results returned from the query as a list of strings
        /// 
        /// \see GetAllContractsFromDB()
        public static List<string> SelectContract(Dictionary<string, string> conditions)
        {
            List<string> fields = new List<string>();
            fields.Add("*");
            string table = "Contract";
            MarketPlace.MakeSelectCommand(fields, table, conditions, null);
            List<string> sqlReturn = MarketPlace.ExecuteCommand();
            return sqlReturn;
        }


        public static void AddContractToTMS(Contract contract)
        {
            // get the count of contracts in the db for the new contract id
            List<string> fields = new List<string>();
            fields.Add("*");
            TMS.MakeSelectCommand(fields, "contract", null, null);
            int count = 0;
            try
            {
                count = TMS.ExecuteCommand().Count;
            }
            catch(Exception e)
            {
                Logger.Log("Server Error - " + e.Message);
                count = 0;
            }

            // get the values from the contract to be added to the database table
            string[] tmpDeets = contract.ToString().Split(',');
            List<string> values = new List<string>();

            // add the values into the list to be inserted to the db
            values.Add(count.ToString());
            foreach(string s in tmpDeets)
            {
                values.Add(s);
            }
            TMS.MakeInsertCommand("contract", values);
            TMS.ExecuteCommand();
        }


        /// \brief A method that is used to add an account to the tms database
        /// \details <b>Details</b>
        /// A method which takes a single account, breaks down its values into string, and adds them into a list to be
        /// added to the Account table in the TMS database.
        /// \param - ac - <b>Account</b> - The account to be added to the Account table in the tms database
        /// \returns - <b>Nothing</b>
        /// 
        /// \see AddAccountToTMS(List<Account> accounts)
        public static void AddAccountToTMS(Account ac)
        {
            List<string> values = new List<string>();
            string acStr = ac.ToString(); // get the string representation of the account
            string[] tmp = acStr.Split(','); // break it down into individual values

            foreach (string s in tmp) // add each value to a list
            {
                values.Add(s);
            }
            TMS.MakeInsertCommand("Account", values); // make the isnert command
            TMS.ExecuteCommand(); // execute said command
        }


        /// \brief An overload of the AddAccountToTMS That takes a list of accounts
        /// \details <b>Details</b>
        /// A method that calls upon the other add account to TMS method to add multiple accounts to the database.
        /// \param - accounts - <b>List<Account></b> - A list of accounts to be added to the TMS database
        /// \returns - <b>Nothing</b>
        /// 
        /// \see AddAccountToTMS(Account ac)
        public static void AddAccountToTMS(List<Account> accounts)
        {
            foreach(Account ac in accounts) //Add all accounts in the list to the database.
            {
                AddAccountToTMS(ac);
            }
        }


        /// \brief A method that gets contracts from the database
        /// \details <b>Details</b>
        /// A method which is used to get contracts from the TMS database. By passing null to the conditions all accounts will be selected
        /// but conditions can be added to narrow the selection.
        /// \param - values - <b>List<string></b> - The column values the user specifically wants to see 
        /// \param - conditions - <b>Dictionary<string, string></b> - The dictionary of key value pairs to be used as search conditions
        /// \returns - sqlReturn - <b>List<string></b> - The results of the Selection
        /// 
        /// \see SelectContract(Dictionary<string, string> conditions)
        /// \see GetAllContractsFromDB()
        public static List<string> GetAccountsFromTMS(List<string> values, Dictionary<string, string> conditions)
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
            TMS.MakeSelectCommand(fields, table, conditions, null); // this can be not null if necessary
            List<string> sqlReturn = TMS.ExecuteCommand();
            return sqlReturn;
        }


        /// \brief A method to get carriers from the tms database
        /// \details <b>Details</b>
        /// A method which returns the carriers in the tms database, the fields shown can be specified by the user, and can be conditional
        /// \param - values - <b>List<string></b> - The column values the user specifically wants to see 
        /// \param - condtions - <b>Dictionary<string, string></b> - The key value pairs that define the conditions
        /// \returns - sqlReturn - <b>List<string></b> - A list of the results generated from the database query.
        /// 
        public static List<string> GetCarriersFromTMS(List<string> values, Dictionary<string, string> conditions)
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
            TMS.MakeSelectCommand(fields, table, conditions, null);
            List<string> sqlReturn = TMS.ExecuteCommand();
            return sqlReturn;
        }


        /// \brief 
        /// \details <b>Details</b>
        /// 
        /// \param - 
        /// \returns - 
        /// 
        /// \see
        public static List<string> GetCarrierDepotCities(List<string> values, Dictionary<string, string> conditions)
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
            TMS.MakeInnerJoinSelect(fields, tables, ids, conditions);
            List<string> sqlReturn = TMS.ExecuteCommand();
            return sqlReturn;
        }


        /// \brief 
        /// \details <b>Details</b>
        /// 
        /// \param - 
        /// \returns - 
        /// 
        /// \see
        public static List<Carrier> SetupCarriers()
        {
            List<Carrier> carriers = new List<Carrier>();
            List<string> carrierDetails = GetCarriersFromTMS(null, null); // get all the carriers in the db
            int i = 0;

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
            List<string> depotCities = GetCarrierDepotCities(fields, null); // do an inner join on the carrier table and depot table

            foreach(Carrier c in carriers)
            {
                foreach(string row in depotCities)
                {
                    string[] tmpSplit = row.Split(',');
                    if(tmpSplit[0] == c.CarrierName)
                    {
                        c.DepotCities.Add(tmpSplit[1]);
                        c.FTLA.Add(int.Parse(tmpSplit[2])); // adding the availability could likely use some work. this should ideally be
                        c.LTLA.Add(int.Parse(tmpSplit[3])); // added for every depot city rather than just to the carrier as a whole.
                    }
                    i++;
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
            int i = 0;

            foreach(Carrier carrier in carriers) //iterate through all carriers, they should already be setup
            {
                i = 0;
                foreach(string city in carrier.DepotCities) // for one carrier, check through all their depot cities to see if one matches
                {                                           // the contract origin city
                    if(contract.Origin == city) // if the contract origin city matches one of the depot cities of a carrier we check next
                    {                           // to see if they have availability of trucks matching a job type.
                        if(contract.JobType == 0) // FTL job
                        {
                            if(carrier.FTLA[i] > 0) // if the carrier has availability, add them to the list of possible carriers
                            {
                                string tmp = carrier.CarrierName + "," + city;
                                possibleCarrier.Add(tmp);
                            }
                        }
                        else // LTL job
                        {
                            if(carrier.LTLA[i] > 0) // if the carrier has an LTL truck available, add them to list of possible carriers for the job
                            {
                                string tmp = carrier.CarrierName + "," + city;
                                possibleCarrier.Add(tmp);
                            }
                        }
                    }
                    i++; // Keep an iterator for depotcities
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
            if (contract.Status != "COMPLETE")
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
                using (StreamWriter sw = new StreamWriter(invoiceFilePath + contract.ClientName + "- "+ contract.ID + ".txt"))
                {
                    sw.Write(sb.ToString());
                }
                return sb.ToString();

            }
        }

        /// \brief A method to generate a report of company earnings and total contracts
        /// \details <b>Details</b>
        /// A method that generates either a 2 week report of contracts and earnings, or an all time report of contracts and earnings
        /// \param - weeks - <b>int</b> - The number of weeks for report generation. Must be either 2 or 0
        /// \returns - <b>Nothing</b>
        /// 
        public static string GenerateReport(int weeks=0)
        {
            string start = "";
            string end = "";
            string type = "";
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
            searchPoints.Add("invoice", end);
            string table = "invoice";

            TMS.MakeBetweenSelect(fields, table, searchPoints);

            List<string> sqlReturn = TMS.ExecuteCommand();

            if(sqlReturn == null || sqlReturn.Count == 0)
            {
                return null;
            }

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
                type = "All-Time";
            }
            else
            {
                sb.AppendFormat("Period: {0} - {1}, ", start, end);
                type = "2-Week";
            }
            sb.AppendFormat("Total Contracts Delivered: {0}, Total Invoice Cost: {1}", invoiceCount, totalCost);
            string report = sb.ToString();

            fields.Clear();
            fields.Add("*");
            TMS.MakeSelectCommand(fields, "report", null, null);
            int reportCount = TMS.ExecuteCommand().Count + 1;

            fields.Clear();
            fields.Add(reportCount.ToString());
            fields.Add(type);
            fields.Add(start);
            fields.Add(end);
            fields.Add(invoiceCount.ToString());
            fields.Add(totalCost.ToString());

            TMS.MakeInsertCommand("report", fields);
            
            return report;
        }

        // Returns the highest TripID value in the TMS DB
        public static int GetLastTripID()
        {
            int retval = 0;
            List<string> fields = new List<string>();
            fields.Add("MAX(TripID)");
            string table = "truck";
            TMS.MakeSelectCommand(fields, table, null, null);
            List<string> LastTripID = TMS.ExecuteCommand();
            if (LastTripID != null) retval = int.Parse(LastTripID.First());

            return retval;
        }

        // Returns the FTL rate of a carrier if 0 or LTL rate if 1 
        public static double GetRate(int carrierID, int qty)
        {
            double rate = 0;

            List<string> fields = new List<string>();
            if (qty == 0 || qty == 26) fields.Add("FTLRate");
            else fields.Add("LTLRate");
            string table = "carrier";
            Dictionary<string, string> conditions = new Dictionary<string, string>();
            conditions.Add("carrierid", carrierID.ToString());
            TMS.MakeSelectCommand(fields, table, conditions, null);
            List<string> response = new List<string>();
            response = TMS.ExecuteCommand();
            rate = double.Parse(response.First());

            return rate;
        }

        // Saves the truck to `trip` and each tripline to `tripline` in TMS
        public static void SaveTripToDB(TripPlanning.Truck truck)
        {
            List<string> values = new List<string>();
            int tmpFlag = 0;
            if (truck.IsComplete) tmpFlag = 1;
            values.Add(truck.TripID.ToString());
            values.Add(truck.CarrierID.ToString());
            values.Add(truck.BillTotal.ToString());
            values.Add(tmpFlag.ToString());
            values.Add(truck.TotalTime.ToString());
            TMS.MakeInsertCommand("truck", values);
            TMS.ExecuteCommand();
        }


        // Saves the truck to `trip` and each tripline to `tripline` in TMS
        public static void SaveTripLineToDB(TripPlanning.TripLine line)
        {
            List<string> values = new List<string>();
            int tmpFlag = 0;

            values.Clear();
            values.Add(line.ContractID.ToString());
            values.Add(line.TripID.ToString());
            values.Add(line.Quantity.ToString());
            values.Add(line.DaysWorked.ToString());
            values.Add(line.Distance.ToString());
            if (line.IsDelivered) tmpFlag = 1;
            else tmpFlag = 0;
            values.Add(tmpFlag.ToString());
            values.Add(line.TotalTime.ToString());
            TMS.MakeInsertCommand("tripline", values);
            TMS.ExecuteCommand();
        }
    }
}
