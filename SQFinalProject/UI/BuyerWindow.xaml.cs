using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SQFinalProject.ContactMgmtBilling;

namespace SQFinalProject.UI {
    ///
    /// \class BuyerWindow
    ///
    /// \brief This class holds all the event handlers for for the buyer WPF window.  It has data members for the TMS database & the Marketplace database.
    /// The error handling in this class will be handled in the form of message boxes describing the errors that happen for majour errors, and error text indicating
    /// simpler errors. The testing for this class will be mainly done manually as this is the most efficient way to access the event handlers in the way that they 
    /// will be used in the final program.
    ///
    /// \author <i>Deric Kruse</i>
    ///
    public partial class BuyerWindow : Window
    {
        //! Properties
        public const string configFilePath = @"..\..\config\TMS.txt";   //<The path to the config file
        public List<string> TMS_Database { get; set; }                  //<The the string list to store TMS DB connection info
        public List<string> MarketPlace_Database { get; set; }          //<The the string list to store Marketplace DB connection info
        Database loginDB { get; set; }                                  //<The database object for the TMS database
        Database MarketPlace { get; set; }                              //<The database object for the Marketplace database

        public string userName;                                         //<Stores the user name of the current user

        ObservableCollection<Contract> contractCollection { get; set; }
        ObservableCollection<Contract> ordersCollection { get; set; }

        public BuyerWindow ( string name )
        {
            InitializeComponent();
            LoadConfig();                                               // Parse the config file
            if (TMS_Database!=null)                                     // Connect to the TMS database if the config file loaded successfully
            {
                loginDB = new Database(TMS_Database[0], TMS_Database[1], TMS_Database[2], TMS_Database[3], TMS_Database[4]);
            }
            if (MarketPlace_Database!=null)                             // Connect to the Marketplace database if the config file loaded successfully
            {
                MarketPlace = new Database(MarketPlace_Database[0], MarketPlace_Database[1], MarketPlace_Database[2], MarketPlace_Database[3], MarketPlace_Database[4]);
            }

            userName = name;
            lblUsrInfo.Content = "User Name:  " + userName;
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
        public void LoadConfig()
        {
            if (File.Exists(configFilePath))                        // If the config file exists, try to read from it
            {
                StreamReader configFile = new StreamReader(configFilePath);
                string contents = configFile.ReadToEnd();
                configFile.Close();
                if (contents != "")
                {
                    string[] splitByDB = contents.Split('\n');      // Grab each line so it can be dealt with individually
                    foreach (string dbDetails in splitByDB)
                    {
                        string[] details = dbDetails.Split(' ');    // Pull the individual fields from the line of the config file
                        if (details[0] == "TMS")                    // If the line pertains to the TMS database, assign the values to the TMS string list
                        {
                            TMS_Database = new List<string>();
                            for (int i = 1; i < details.Count(); i++)
                            {
                                TMS_Database.Add(details[i]);
                            }
                        }
                        else if (details[0] == "MP")                // If the line pertains to the Marketplace database, assign the values to the Marketplace string list
                        {
                            MarketPlace_Database = new List<string>();
                            for (int i = 1; i < details.Count(); i++)
                            {
                                MarketPlace_Database.Add(details[i]);
                            }
                        }
                    }
                }
            }
            else
            {
                FileStream newConfig = File.Create(configFilePath);
                newConfig.Close();
            }
        }



        //  METHOD:	CloseCB_CanExecute
        /// \brief Says when the close command can run, which is always
        /// \details <b>Details</b>
        ///     The CanExecute method says when the Close command binding can function, which is always.
        ///
        /// \param - <b>sender:</b>  the object that called the method
        /// \param - <b>e:</b>       the arguments that are passed when this method is called
        ///
        /// \return - <b>Nothing</b>
        ///
        private void CloseCB_CanExecute ( object sender,CanExecuteRoutedEventArgs e ) {
            e.CanExecute = true;
        }



        //  METHOD:	CloseCB_Executed
        /// \brief Dictates what happens when the close command runs
        /// \details <b>Details</b>
        ///     The Executed method says what happens the Close Command is run.  If Close is clicked, close the window.
        ///
        /// \param - <b>sender:</b>  the object that called the method
        /// \param - <b>e:</b>       the arguments that are passed when this method is called
        ///
        /// \return - <b>Nothing</b>
        ///
        private void CloseCB_Executed ( object sender,ExecutedRoutedEventArgs e ) {
            this.Close();
        }



        //  METHOD:		Logout_Click
        /// \brief Method that is called when the logout item on the menu is clicked
        /// \details <b>Details</b>
        ///     Closes this window and opens up the login window when the option is clicked on the menu.
        ///
        /// \param - <b>sender:</b>  the object that called the method
        /// \param - <b>e:</b>       the arguments that are passed when this method is called
        ///
        /// \return - <b>Nothing</b>
        private void Logout_Click ( object sender,RoutedEventArgs e ) {
            LoginWindow login = new LoginWindow ();
            login.Show();

            this.Close();
        }



        //  METHOD:		About_Click
        /// \brief Method that is called when the About item on the menu is clicked
        /// \details <b>Details</b>
        ///     Opens up the about window when the option is clicked on the menu.
        ///
        /// \param - <b>sender:</b>  the object that called the method
        /// \param - <b>e:</b>       the arguments that are passed when this method is called
        ///
        /// \return - <b>Nothing</b>
        ///
        private void About_Click ( object sender,RoutedEventArgs e ) {
            AboutWindow aboutBox = new AboutWindow();
            aboutBox.Owner = this;
            aboutBox.ShowDialog();
        }


        //  METHOD:		GetContracts
        /// \brief This method gets the currently available contracts from the marketplace and displays them to the screen.
        /// \details <b>Details</b>
        ///     Creates and sends a query to the Marketplace database then populates a container which updates the information in
        /// the list box on the screen.
        ///
        /// \param - <b>sender:</b>  the object that called the method
        /// \param - <b>e:</b>       the arguments that are passed when this method is called
        ///
        /// \return - <b>Nothing</b>
        ///
        private void GetContracts ( object sender,RoutedEventArgs e ) {
            contractCollection = new ObservableCollection<Contract>();
            List<string> QueryLst = new List<string> ();
            QueryLst.Add("*");
            MarketPlace.MakeSelectCommand ( QueryLst, "Contract", null, null);
            List<string> contracts = MarketPlace.ExecuteCommand();
            foreach (string contract in contracts)
            {
                Contract c = new Contract(contract);
                contractCollection.Add(c);
            }
            MarketList.ItemsSource = contractCollection;
        }

        //  METHOD:		TakeContracts
        /// \brief This method gets the currently selected contract from the list, removes it from the list and adds it to the TMS database
        /// \details <b>Details</b>
        ///     Creates an insert query string to insert the new contract into our database.  
        ///     Also calls methods that check if we have the client in our system and adds them in if not.
        /// 
        ///
        /// \param - <b>sender:</b>  the object that called the method
        /// \param - <b>e:</b>       the arguments that are passed when this method is called
        ///
        /// \return - <b>Nothing</b>
        ///
        private void TakeContracts(object sender, RoutedEventArgs e)
        {
            Contract selectedContract = (Contract)MarketList.SelectedItem;
            contractCollection.Remove(selectedContract);
            selectedContract.status = "NEW";
            if (!CheckAccount(selectedContract.ClientName))
            {
                AddAccount(selectedContract.ClientName);
            }
            List<string> fields = new List<string>();
            fields.Add("clientname");
            fields.Add("jobtype");
            fields.Add("skidQuant");
            fields.Add("depotCity");
            fields.Add("destCity");
            fields.Add("vantype");
            fields.Add("status");
            List<string> values = new List<string>();
            values.Add(selectedContract.ClientName);
            values.Add(selectedContract.JobType.ToString());
            values.Add(selectedContract.Quantity.ToString());
            values.Add(selectedContract.Origin);
            values.Add(selectedContract.Destination);
            values.Add(selectedContract.VanType.ToString());
            values.Add(selectedContract.status);
            loginDB.MakeInsertCommand("orders", fields, values);
            loginDB.ExecuteCommand();
        }
        private bool CheckAccount(string name)
        {
            bool isIn = false;
            List<string> fields = new List<string>();
            fields.Add("*");
            Dictionary<string, string> conditions = new Dictionary<string, string>();
            conditions.Add("clientname", name);
            loginDB.MakeSelectCommand(fields, "account", conditions, null);
            if (loginDB.ExecuteCommand().Count > 0)
            {
                isIn = true;
            }
            return isIn;
        }
        private void AddAccount(string name)
        {
            List<string> fields = new List<string>();
            fields.Add("clientName");
            List<string> values = new List<string>();
            values.Add(name);
            loginDB.MakeInsertCommand("account",fields,values);
            loginDB.ExecuteCommand();
        }

        private void TabsCtrl_Buyer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Orders.IsSelected)
            {
                GetOrders();
                OrderList.ItemsSource = ordersCollection;
            }
        }

        private void GetOrders()
        {
            ordersCollection = new ObservableCollection<Contract>();
            List<string> fields = new List<string>();
            fields.Add("*");
            loginDB.MakeSelectCommand(fields, "orders", null, null);
            List<string> results = loginDB.ExecuteCommand();
            foreach (string result in results)
            {
                string[] splitResult = result.Split(',');
                StringBuilder recombine = new StringBuilder();
                recombine.AppendFormat("{0},{1},{2},{3},{4},{5}",splitResult[1],splitResult[2],splitResult[3],splitResult[4],splitResult[5],splitResult[6]);
                Contract c = new Contract(recombine.ToString());
                int temp;
                int.TryParse(splitResult[0], out temp);
                c.ID = temp;
                c.status = splitResult[7];
                ordersCollection.Add(c);
            }
            
        }
    }

}
