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
    /// \author <i>Chris Lemon</i>
    ///
    public partial class BuyerWindow : Window
    {
        //! Properties
        public string userName;                                         //<Stores the user name of the current user
         
        ObservableCollection<Contract> customerPageCollection { get; set; }
        ObservableCollection<Contract> contractCollection { get; set; }
        ObservableCollection<Contract> ordersCollection { get; set; }

        public BuyerWindow ( string name )
        {
            InitializeComponent();                                             // Parse the config file

            userName = name;
            lblUsrInfo.Content = "User Name:  " + userName;
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
            List<string> contracts = Controller.GetAllContractsFromDB();
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
        /// \param - <b>sender:</b>  the object that called the method
        /// \param - <b>e:</b>       the arguments that are passed when this method is called
        ///
        /// \return - <b>Nothing</b>
        ///
        private void TakeContracts(object sender, RoutedEventArgs e)
        {
            Contract selectedContract = (Contract)MarketList.SelectedItem;
            contractCollection.Remove(selectedContract);
            selectedContract.Status = "NEW";
            if (!CheckAccount(selectedContract.ClientName))
            {
                AddAccount(selectedContract.ClientName);
            }
            List<string> fields = new List<string>();
            fields.Add("accountid");
            Dictionary<string, string> cond = new Dictionary<string, string>();
            cond.Add("clientname", selectedContract.ClientName);
            Controller.TMS.MakeSelectCommand(fields, "account", cond, null);
            List<string> sqlreturn = Controller.TMS.ExecuteCommand();
            selectedContract.AccountID = int.Parse(sqlreturn[0]);
            fields.Clear();

            fields.Add("clientname");
            fields.Add("jobtype");
            fields.Add("skidQuant");
            fields.Add("depotCity");
            fields.Add("destCity");
            fields.Add("vantype");
            fields.Add("status");
            fields.Add("accountid");
            List<string> values = new List<string>();
            values.Add(selectedContract.ClientName);
            values.Add(selectedContract.JobType.ToString());
            values.Add(selectedContract.Quantity.ToString());
            values.Add(selectedContract.Origin);
            values.Add(selectedContract.Destination);
            values.Add(selectedContract.VanType.ToString());
            values.Add(selectedContract.Status);
            values.Add(selectedContract.AccountID.ToString());
            Controller.TMS.MakeInsertCommand("contract", fields, values);
            Controller.TMS.ExecuteCommand();
        }

        //  METHOD:		CheckAccount
        /// \brief This method checks if a client  has an account in the TMS database
        /// \details <b>Details</b>
        ///     Creates a select query string that tries to get a single entry from the database, representing 
        ///     an account held by a specific client. 
        /// 
        /// \param - <b>name</b> - the name of the client to be checked for  
        ///
        /// \return - <b>isIn</b> - a bool that marks whether the account is present or not
        ///
        private bool CheckAccount(string name)
        {
            bool isIn = false;
            List<string> fields = new List<string>();
            fields.Add("accountid");
            fields.Add("clientName");
            Dictionary<string, string> conditions = new Dictionary<string, string>();
            conditions.Add("clientname", name);
            Controller.TMS.MakeSelectCommand(fields, "account", conditions, null);
            List<string> sqlReturn = Controller.GetAccountsFromTMS(fields, conditions);
            if ((sqlReturn != null) && (sqlReturn.Count != 0))
            {
                isIn = true;
            }
            return isIn;
        }

        //  METHOD:		AddAccount
        /// \brief This method makes a new entry in the account database
        /// \details <b>Details</b>
        ///     If the client does not have an account, the method creates an insert query to insert the information into the database
        /// 
        /// \param - <b>name</b> - the name of the client to be checked for  
        ///
        /// \return - <b>Nothing</b>
        ///
        private void AddAccount(string name)
        {
            List<string> fields = new List<string>();
            fields.Add("clientName");
            List<string> values = new List<string>();
            values.Add(name);
            Controller.TMS.MakeInsertCommand("account",fields,values);
            Controller.TMS.ExecuteCommand();
        }

        //  METHOD:		TabsCtrl_Buyer_SelectionChanged
        /// \brief This method controls the behavior of the individual tabs in the buyer page
        /// \details <b>Details</b>
        ///     Checks which tab is selected and executes code as appropriate
        /// 
        /// \param - <b>sender:</b>  the object that called the method
        /// \param - <b>e:</b>       the arguments that are passed when this method is called
        ///
        /// \return - <b>Nothing</b>
        ///
        private void TabsCtrl_Buyer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Orders.IsSelected)
            {
                GetOrders();
                OrderList.ItemsSource = ordersCollection;
            }
            else if(CustomerTab.IsSelected)
            {
                GetCustomers();
                
            }
        }

        //  METHOD:		GetCustomers
        /// \brief Gets all the customers in the customer database
        /// \details <b>Details</b>
        ///    Queries the account table for all client names 
        /// 
        /// \param - <b>None</b>
        ///
        /// \return - <b>Nothing</b>
        ///
        private void GetCustomers()
        {
            CustomerCombo.Items.Clear();
            List<string> fields = new List<string>();
            fields.Add("clientName");
            Controller.TMS.MakeSelectCommand(fields,"account",null, null);
            List<string> results = Controller.TMS.ExecuteCommand();
            foreach (string result in results)
            {
                CustomerCombo.Items.Add(result);
            }
        }

        //  METHOD:		GetOrders
        /// \brief This method displays all the orders in the database
        /// \details <b>Details</b>
        ///     Creates a select query statement that gets all rows from the orders table
        ///     and then converts each row into a Contract object which is then added to an
        ///     ObservableCollection which is used to bind with a ListBox
        ///
        /// \param - <b>None</b>
        /// 
        /// \return - <b>Nothing</b>
        ///
        private void GetOrders()
        {
            ordersCollection = new ObservableCollection<Contract>();
            List<string> fields = new List<string>();
            fields.Add("*");
            Controller.TMS.MakeSelectCommand(fields, "contract", null, null);
            List<string> results = Controller.TMS.ExecuteCommand();
            foreach (string result in results)
            {
                string[] splitResult = result.Split(',');
                StringBuilder recombine = new StringBuilder();
                recombine.AppendFormat("{0},{1},{2},{3},{4},{5}",splitResult[1],splitResult[2],splitResult[3],splitResult[4],splitResult[5],splitResult[6]);
                Contract c = new Contract(recombine.ToString());
                int temp;
                int.TryParse(splitResult[0], out temp);
                c.ID = temp;
                c.Status = splitResult[7];
                ordersCollection.Add(c);
            }
            
        }

        //  METHOD:		SendToPlanner_Click
        /// \brief Sets the status of the contract to In-Progress and updates the database
        /// \details <b>Details</b>
        ///     Gets the selected order from the ListWindow and sets it's status to In-Progress.
        ///     It then creates an update query to update the database with the new information
        /// 
        /// \param - <b>sender:</b>  the object that called the method
        /// \param - <b>e:</b>       the arguments that are passed when this method is called
        ///
        /// \return - <b>Nothing</b>
        ///
        private void SendToPlanner_Click(object sender, RoutedEventArgs e)
        {
            Contract toSend = (Contract)OrderList.SelectedItem;
            toSend.Status = "PLANNING";
            Dictionary<string, string> values = new Dictionary<string, string>();
            values.Add("status", toSend.Status);
            Dictionary<string, string> conditions = new Dictionary<string, string>();
            conditions.Add("clientname", toSend.ClientName);
            Controller.TMS.MakeUpdateCommand("contract",values,conditions);
            Controller.TMS.ExecuteCommand();
            OrderList.ItemsSource = null;
            OrderList.ItemsSource = ordersCollection;
        }

        private void GenerateInvoice_Click(object sender, RoutedEventArgs e)
        {
            Contract toSend = (Contract)OrderList.SelectedItem;
            toSend.Status = "CLOSED";
            OrderList.ItemsSource = null;
            OrderList.ItemsSource = ordersCollection;

            // Get account from database
            List<string> sqlReturn = new List<string>();
            List<string> fields = new List<string>();
            fields.Add("accountid");
            fields.Add("clientName");

            Dictionary<string, string> conditions = new Dictionary<string, string>();
            conditions.Add("clientName", toSend.ClientName);

            Controller.TMS.MakeSelectCommand(fields, "account", conditions, null);
            sqlReturn = Controller.TMS.ExecuteCommand();
            string[] tmpArray = sqlReturn[0].Split(',');

            // Create an account class using the toSend contract
            Account tmpAcc = new Account(toSend);
            tmpAcc.AccountID = int.Parse(tmpArray[0]);

            // Use the account and the contract to generate an invoice containing the all necessary IDs and details to identify it
            string invoice = Controller.GenerateInvoice(tmpAcc, toSend);

            if(invoice != null)
            {
                tmpArray = invoice.Split(',');
                List<string> values = tmpArray.ToList();

                // set up an insert for the invoice
                Controller.TMS.MakeInsertCommand("invoice", values);

                // Insert the invoice into the correct table
                Controller.TMS.ExecuteCommand();
            }
            else
            {
                toSend.Status = "COMPLETED";
            }
        }

        //  METHOD:		OrderList_SelectionChanged
        /// \brief Enables buttons depending on what is selected
        /// \details <b>Details</b>
        ///     Gets the current item selected, and enables a button based on the status of the order
        /// 
        /// \param - <b>sender:</b>  the object that called the method
        /// \param - <b>e:</b>       the arguments that are passed when this method is called
        ///
        /// \return - <b>Nothing</b>
        ///
        private void OrderList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            e.Handled = true;
            Contract c = (Contract)OrderList.SelectedItem;
            if (c != null)
            {
                if (c.Status == "NEW")
                {
                    SendPlanner.IsEnabled = true;
                    GenInvoice.IsEnabled = false;
                }
                else if (c.Status == "COMPLETED")
                {
                    SendPlanner.IsEnabled = false;
                    GenInvoice.IsEnabled = true;
                }
                else
                {
                    SendPlanner.IsEnabled = false;
                    GenInvoice.IsEnabled = false;
                }
            }
            else
            {
                SendPlanner.IsEnabled = false;
                GenInvoice.IsEnabled = false;
            }
        }

        //  METHOD:		CustomerCombo_SelectionChanged
        /// \brief Updates the customer collection depending on selection
        /// \details <b>Details</b>
        ///     Sets the CustomerList itemssource to null, gets a new list of customers and resets the itemssource to the collection
        /// 
        /// \param - <b>sender:</b>  the object that called the method
        /// \param - <b>e:</b>       the arguments that are passed when this method is called
        ///
        /// \return - <b>Nothing</b>
        ///
        private void CustomerCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            e.Handled = true;
            CustomerList.ItemsSource = null;
            getCustomerContracts();
            CustomerList.ItemsSource = customerPageCollection;
        }

        //  METHOD:		getCustomerContracts
        /// \brief Gets all the contracts associated with that client
        /// \details <b>Details</b>
        ///     Queries the contract table to get all contracts associated with that client
        /// 
        /// \param - <b>None</b>
        /// \return - <b>Nothing</b>
        ///
        private void getCustomerContracts()
        {
            double totalCost = 0;
            customerPageCollection = new ObservableCollection<Contract>();
            string cbi = (string)CustomerCombo.SelectedItem;
            List<string> fields = new List<string>();
            fields.Add("contractID");
            fields.Add("status");
            Dictionary<string, string> conditions = new Dictionary<string, string>();
            conditions.Add("clientname", cbi);
            Controller.TMS.MakeSelectCommand(fields, "contract", conditions, null);
            List<string> results = Controller.TMS.ExecuteCommand();
            foreach (string result in results)
            {
                Contract c = new Contract();
                string[] splitResult = result.Split(',');
                int temp;
                int.TryParse(splitResult[0], out temp);
                c.ID = temp;
                c.Status = splitResult[1];
                if (c.Status == "CLOSED")
                {
                    List<string> newFields = new List<string>();
                    newFields.Add("invoiceID");
                    newFields.Add("cost");
                    Dictionary<string, string> moreConditions = new Dictionary<string, string>();
                    moreConditions.Add("contractID", c.ID.ToString());
                    Controller.TMS.MakeSelectCommand(newFields, "invoice", moreConditions, null);
                    List<string> moreResults = Controller.TMS.ExecuteCommand();
                    foreach (string moreResult in moreResults)
                    {
                        string[] moreSplit = moreResult.Split(',');
                        int.TryParse(moreSplit[0], out temp);
                        c.invoiceNum = temp;
                        int.TryParse(moreSplit[1], out temp);
                        c.Cost = temp;
                        totalCost = totalCost + c.Cost;
                    }
                }
                AmountOwing.Text = totalCost.ToString();
                customerPageCollection.Add(c);
            }
        }

        //  METHOD:		CustomerList_SelectionChanged
        /// \brief Prevents a page refresh
        /// \details <b>Details</b>
        ///     Sets Handled to true to prevent a page refresh
        /// 
        /// \param - <b>sender:</b>  the object that called the method
        /// \param - <b>e:</b>       the arguments that are passed when this method is called
        ///
        /// \return - <b>Nothing</b>
        ///
        private void CustomerList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            e.Handled = true;
        }
    }
}
