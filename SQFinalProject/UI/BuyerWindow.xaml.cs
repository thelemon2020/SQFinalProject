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
        public string userName;                                         //<Stores the user name of the current user

        ObservableCollection<Contract> contractCollection { get; set; }
        ObservableCollection<Contract> ordersCollection { get; set; }

        public BuyerWindow ( string name )
        {
            InitializeComponent();
            Controller.LoadConfig();                                               // Parse the config file

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
            values.Add(selectedContract.Status);
            Controller.TMS.MakeInsertCommand("orders", fields, values);
            Controller.TMS.ExecuteCommand();
        }
        private bool CheckAccount(string name)
        {
            bool isIn = false;
            List<string> fields = new List<string>();
            fields.Add("*");
            Dictionary<string, string> conditions = new Dictionary<string, string>();
            conditions.Add("clientname", name);
            Controller.TMS.MakeSelectCommand(fields, "account", conditions, null);
            if (Controller.TMS.ExecuteCommand().Count > 0)
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
            Controller.TMS.MakeInsertCommand("account",fields,values);
            Controller.TMS.ExecuteCommand();
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
            Controller.TMS.MakeSelectCommand(fields, "orders", null, null);
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

        private void SendToPlanner_Click(object sender, RoutedEventArgs e)
        {
            Contract toSend = (Contract)OrderList.SelectedItem;
            toSend.Status = "IN-PROGRESS";
            Dictionary<string, string> values = new Dictionary<string, string>();
            values.Add("status", toSend.Status);
            Dictionary<string, string> conditions = new Dictionary<string, string>();
            conditions.Add("clientname", toSend.ClientName);
            Controller.TMS.MakeUpdateCommand("orders",values,conditions);
            Controller.TMS.ExecuteCommand();
            OrderList.ItemsSource = null;
            OrderList.ItemsSource = ordersCollection;
        }

        private void GenerateInvoice_Click(object sender, RoutedEventArgs e)
        {

        }

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
    }
}
