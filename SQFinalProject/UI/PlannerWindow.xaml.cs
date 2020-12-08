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
using SQFinalProject.TripPlanning;

namespace SQFinalProject.UI {
    ///
    /// \class PlannerWindow
    ///
    /// \brief This class holds all the event handlers for for the Planner WPF window.  It is split into Orders, Summary, and Reports tabs.
    /// The error handling in this class will be handled in the form of message boxes describing the errors that happen for majour errors, and error text indicating
    /// simpler errors. The testing for this class will be mainly done manually as this is the most efficient way to access the event handlers in the way that they
    /// will be used in the final program.
    ///
    /// \author <i>Nick Byam, Deric Kruse, & Chris Lemon</i>
    ///
    public partial class PlannerWindow : Window
    {
        //! Properties
        private int OrderState { get; set; }                                        //<Stores the state that an order is in, for enabling or disabling controls
        private string userName { get; set; }                                       //<Stores the user name of the current user
        ObservableCollection<Contract> ordersCollection { get; set; }               //<Collection of contracts for databinding to the order summary list
        ObservableCollection<Contract> currOrder { get; set; }                      //<Collection of contracts for databinding to the order details table
        ObservableCollection<Carrier> currCarrier { get; set; }                     //<Collection of carriers for databinding to the carrier selection dropdown list
        ObservableCollection<List<string>> currOrderTripDet { get; set; }            //<Collection of strings containing trip details for databinding to the order trips list
        ObservableCollection<Truck> truckCollection {get;set;}                      //<Collection of trucks for databinding to the truck selection dropdown list

        ObservableCollection<Contract> planningCollection { get; set; }             //<Collection of contracts for databinding to the order selection list
        List<Truck> Trucks { get; set; }                                            //<Stores the trucks added this session
        List<Contract> Contracts { get; set; }                                      //<Stores the list of contracts from the DB
        List<Carrier> Carriers { get; set; }                                        //<Stores the list of carriers from the DB

        private int currQntRem { get; set; }                                       //<Stores the remaining quantity of the current contract
        public DateTime time { get; set; }



        public PlannerWindow ( string name ) {
            InitializeComponent();
            Trucks = new List<Truck>();
            Carriers = new List<Carrier>();

            GetContracts();
            Carriers = Controller.SetupCarriers();

            userName = name;
            OrderState = 0;
            lblUsrInfo.Content = "User Name:  " + userName;
        }


        //  METHOD:		GetContracts
        /// \brief Gets contracts from the database and creates a local list of them
        /// \details <b>Details</b>
        ///     Sets up a select command and gets the contracts from the database.  It also gets all of the trip lines associated with those contracts
        /// adds them to the list of trip lines in each contract.
        ///
        /// \param - <b>None</b>
        ///
        /// \return - <b>Nothing</b>
        ///
        private void GetContracts()
        {
            // Sets up the select command and gets the results from the database
            Contracts = new List<Contract>();
            List<string> fields = new List<string>();
            fields.Add("*");
            Controller.TMS.MakeSelectCommand(fields, "contract", null, null);
            List<string> results = Controller.TMS.ExecuteCommand();

            foreach (string result in results)
            {
                // For each result, process the string into a form that can be put into the contract constructor and  create a new contract
                string[] splitResult = result.Split(',');
                StringBuilder recombine = new StringBuilder();
                recombine.AppendFormat("{0},{1},{2},{3},{4},{5}",splitResult[1],splitResult[2],splitResult[3],splitResult[4],splitResult[5],splitResult[6]);
                Contract c = new Contract(recombine.ToString());
                int temp;
                int.TryParse(splitResult[0], out temp);
                c.ID = temp;
                c.Status = splitResult[7];

                // Set up the database query for the trip lines that are part of a contract and get them
                List<string> field = new List<string>();
                field.Add("*");

                Dictionary<string, string> conditions = new Dictionary<string, string>();
                conditions.Add("contractid", c.ID.ToString());

                Controller.TMS.MakeSelectCommand(field, "tripline", conditions,null);
                results = Controller.TMS.ExecuteCommand();

                // Add each tripline that the database returns to its contract
                foreach (string resulting in results)
                {
                    TripLine t = new TripLine(resulting, c.Destination);
                    c.Trips.Add(t);
                }
                if (c.Trips.Count == 0)
                {
                    c.TripComplete = false;
                }
                else
                {
                    c.TripComplete = true;
                    foreach (TripLine trip in c.Trips)
                    {
                        if (trip.IsDelivered == false)
                        {
                            c.TripComplete = false;
                        }
                    }
                }
                Contracts.Add(c);
            }

            // Set up the observable collections so the user interface can be updated
            ordersCollection = new ObservableCollection<Contract> ( Contracts );
            planningCollection = new ObservableCollection<Contract>();
            foreach (Contract c in ordersCollection)
            {
                if (c.Status == "PLANNING")
                {
                    planningCollection.Add(c);
                }
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



        /* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ */
        /* ~~~~~ Methods for contracts in Planning stage ~~~~~ */
        /* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ */

        //  METHOD:		TabsCtrl_Planner_SelectionChanged
        /// \brief Method that is called when tabs are changed in the planner window.
        /// \details <b>Details</b>
        ///     Sets up the required observable collections for each tab.
        ///
        /// \param - <b>sender:</b>  the object that called the method
        /// \param - <b>e:</b>       the arguments that are passed when this method is called
        ///
        /// \return - <b>Nothing</b>
        ///
        private void TabsCtrl_Planner_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            e.Handled = true;

            OrderList.ItemsSource = null;
            SummaryList.ItemsSource = null;

            GetContracts();

            if (Orders.IsSelected)
            {
                OrderList.ItemsSource = planningCollection;
            }
            else if (Summary.IsSelected)
            {
                SummaryList.ItemsSource = ordersCollection;
            }
        }



        //  METHOD:		OrderList_SelectionChanged
        /// \brief Method that is called when an order is selected from the order list.
        /// \details <b>Details</b>
        ///     This method sets up what happens when the an item from the order list is selected. First the collections
        /// that need to be reset for the user interface to be updaded are reset.  If the selected item is in the planning
        /// stage, the method sets up the tab so they can plan the shiping details of the contract.
        ///
        /// \param - <b>sender:</b>  the object that called the method
        /// \param - <b>e:</b>       the arguments that are passed when this method is called
        ///
        /// \return - <b>Nothing</b>
        ///
        private void OrderList_SelectionChanged ( object sender,SelectionChangedEventArgs e )
        {
            e.Handled = true;
            OrderState = 0;

            OrderDetails.ItemsSource = null;
            OrderTrips.ItemsSource = null;
            CarrierSelector.Items.Clear();

            TruckSelector.Items.Clear();
            TruckSelector.ItemsSource = null;

            currOrder = new ObservableCollection<Contract>();
            truckCollection = new ObservableCollection<Truck>();

            if ( OrderList.SelectedIndex != -1 )
            {
                currOrder.Add( (Contract) OrderList.SelectedItem );

                // If the selected contract is in the planning stage,  set up the carriers combo box so they can start planning
                if ( currOrder[0].Status.ToUpper().Equals ("PLANNING") ) {
                    OrderState = 1;

                    Carriers = Controller.SetupCarriers();
                    List <string> availCarriers = Controller.FindCarriersForContract( (Contract)currOrder.ElementAt(0), Carriers );

                    foreach ( string s in availCarriers )
                    {
                        CarrierSelector.Items.Add (s);
                    }

                    currQntRem = ((Contract) OrderList.SelectedItem).Quantity;

                    QntRem.Text = currQntRem.ToString();

                    // If the current contract has no trips, ensure that the list has been initialized
                    if ( currOrder[0].Trips == null ) {
                        currOrder[0].Trips = new List<TripLine>();
                    }

                    // Enable the Finalize button if it is valid, disable it otherwise
                    if ( currOrder[0].JobType == 0 && currOrder[0].Trips.Count == 0  ) {
                        btnFinalize.IsEnabled = false;
                    } else if ( currQntRem != 0 ) {
                        btnFinalize.IsEnabled = false;
                    } else {
                        btnFinalize.IsEnabled = true;
                    }
                }

                currOrderTripDet = new ObservableCollection<List<string>> ( getTripDetails( currOrder[0].Trips) );
                OrderTrips.ItemsSource = currOrderTripDet;
            }

            OrderDetails.ItemsSource = currOrder;

            EnableOrderControls (OrderState);
        }



        //  METHOD:		EnableOrderControls
        /// \brief Enables / disables the required controls on the orders tab
        /// \details <b>Details</b>
        ///     Either enables or disables controls to set up the orders tab for the current contract.
        ///
        /// \param - <b>doShow</b>  integer to store the state that the currently selected contract is in
        ///
        /// \return - <b>Nothing</b>
        ///
        private void EnableOrderControls ( int doShow ) {
            btnAddTruck.IsEnabled = false;

            if ( doShow == 1 )
            {
                CarrierSelector.IsEnabled = true;
                TruckRem.Text = "";
            }
            else
            {
                TruckSelector.IsEnabled = false;
                CarrierSelector.IsEnabled = false;
                btnAddTruck.IsEnabled = false;
                TruckRem.Text = "";
            }
        }



        //  METHOD:		CarrierSelector_SelectionChanged
        /// \brief Method that is called when a carrier is selected from the carrier selection combo box
        /// \details <b>Details</b>
        ///     If a carrier is selected, the method gets the current carrier stats from the database and adds the partially filled
        /// trucks from the truck list to the truck selection combo box.
        ///
        /// \param - <b>sender:</b>  the object that called the method
        /// \param - <b>e:</b>       the arguments that are passed when this method is called
        ///
        /// \return - <b>Nothing</b>
        ///
        private void CarrierSelector_SelectionChanged ( object sender,SelectionChangedEventArgs e )
        {
            e.Handled = true;
            CarrierDetails.ItemsSource = null;
            currCarrier = new ObservableCollection<Carrier>();
            TruckSelector.Items.Clear();
            if ( CarrierSelector.SelectedIndex != -1 && CarrierSelector.SelectedItem != null )
            {
                TruckSelector.SelectedIndex = -1;
                TruckRem.Text = "";

                TruckSelector.IsEnabled = true;

                // Gets the carrier details from the database and puts them in the carrier details box
                Dictionary<string, string> conditions = new Dictionary<string, string>();
                conditions.Add( "carrierName", ((string) CarrierSelector.SelectedItem).Split(',').ElementAt(0) );

                List <string> currStrCarrier = new List<string>( Controller.GetCarriersFromTMS( null, conditions )[0].Split(',') );

                currCarrier.Add( new Carrier (currStrCarrier) );

                // Populates the truck selection combo box with valid partially full trucks from the local list of trucks
                TruckSelector.Items.Add("New Truck");
                foreach (Truck truck in Trucks)
                {
                    if ((truck.Origin == currOrder[0].Origin) && (truck.Origin == truck.ThisRoute.Cities[0].Name) && (truck.RemainingQuantity() > 0) && (truck.CarrierID.ToString() == currStrCarrier[0]) && (truck.Direction == currOrder[0].Direction))
                    {
                        truckCollection.Add(truck);
                        TruckSelector.Items.Add(truck.TripID);
                    }
                }
            }
            else
            {
                btnAddTruck.IsEnabled = false;
            }

            CarrierDetails.ItemsSource = currCarrier;
        }



        //  METHOD:		TruckSelector_SelectionChanged
        /// \brief Method that is called when a truck is selected from the truck selection combo box
        /// \details <b>Details</b>
        ///     If a truck is selected, the method enables either the add truck button or the finalize button depending on
        /// whether the current contract has had all its pallets added to trucks.
        ///
        /// \param - <b>sender:</b>  the object that called the method
        /// \param - <b>e:</b>       the arguments that are passed when this method is called
        ///
        /// \return - <b>Nothing</b>
        ///
        private void TruckSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            e.Handled = true;
            if ( TruckSelector.SelectedIndex != -1 && TruckSelector.SelectedItem != null )
            {
                //  Enables the add truck button if the contract still needs trips added to it and activates the finalize button if it doesnt
                if ( currOrder[0].JobType == 0 && currOrder[0].Trips.Count == 0  ) {
                    btnAddTruck.IsEnabled = true;
                } else if ( currQntRem != 0 ) {
                    btnAddTruck.IsEnabled = true;
                } else if ( currQntRem == 0 ) {
                    btnFinalize.IsEnabled = true;
                }
                if (TruckSelector.SelectedIndex == 0)
                {
                    TruckRem.Text = "26";
                }
                else
                {
                    int i = TruckSelector.SelectedIndex;
                    Truck t = truckCollection[i-1];

                    TruckRem.Text = t.RemainingQuantity().ToString();
                }
            }
            else
            {
                btnAddTruck.IsEnabled = false;
            }
        }



        //  METHOD:		AddTruck_Click
        /// \brief Method that is called when the add truck button is clicked
        /// \details <b>Details</b>
        ///     This method contains the logic for adding trucks / trips to the contract.  If the contract is an
        /// FTL contract it adds a truck and enables the finalize button.  If a new truck was selected for an LTL
        /// contract, it creates the truck and fills it up appropriately.  If a partially filled truck has been selected,
        /// fill it up appropriately.  Finally the function sets up the trip details box.
        ///
        /// \param - <b>sender:</b>  the object that called the method
        /// \param - <b>e:</b>       the arguments that are passed when this method is called
        ///
        /// \return - <b>Nothing</b>
        ///
        private void AddTruck_Click ( object sender, RoutedEventArgs e )
        {
            int truckLoad = 0;
            OrderTrips.ItemsSource = null;

            // If the contract is FTL, we just create a truck and fill it as its the only thing in the load
            if ( currOrder[0].JobType == 0 ) {

                btnAddTruck.IsEnabled = false;
                btnFinalize.IsEnabled = true;

                truckLoad = 26;

                Truck newTruck = new Truck ( currOrder[0], currCarrier[0], truckLoad );
                TruckRem.Text = "";
                Trucks.Add(newTruck);

                currOrder[0].Trips.Add ( newTruck.Contracts.Last() );

            // If a new truck was selected, create a new truck containing the tripline and add as much of the contract as possible
            } else if ( TruckSelector.SelectedIndex == 0 ) {

                if ( currQntRem <= 26 ) {
                    truckLoad += currQntRem;
                    btnAddTruck.IsEnabled = false;

                    currQntRem = 0;

                    btnFinalize.IsEnabled = true;
                } else {
                    truckLoad = 26;
                    currQntRem -= truckLoad;
                }

                QntRem.Text = currQntRem.ToString();

                Truck newTruck = new Truck ( currOrder[0], currCarrier[0], truckLoad );
                TruckRem.Text = newTruck.RemainingQuantity().ToString();
                Trucks.Add(newTruck);

                currOrder[0].Trips.Add ( newTruck.Contracts.Last() );
                currOrder[0].Quantity = currQntRem;

            // If a partially filled truck was selected, put as much of the contract in it as possible and add the tripline to the truck
            } else {
                int i = TruckSelector.SelectedIndex;
                Truck t = truckCollection[i-1];

                if ( currQntRem <= t.RemainingQuantity() ) {

                    truckLoad += currQntRem;
                    btnAddTruck.IsEnabled = false;

                    currQntRem = 0;

                    btnFinalize.IsEnabled = true;
                } else {
                    truckLoad += t.RemainingQuantity();
                    currQntRem -= t.RemainingQuantity();
                }

                TripLine tmpTL = new TripLine(currOrder[0], t.TripID, truckLoad);

                t.AddContract( tmpTL );
                currOrder[0].Trips.Add ( t.Contracts.Last() );
                currOrder[0].Quantity = currQntRem;

                QntRem.Text = currQntRem.ToString();
                TruckRem.Text = t.RemainingQuantity().ToString();

                // Add the triplines into the Right contract in the Contract list
                foreach(Contract c in Contracts)
                {
                    if(c.ID == currOrder[0].ID) // Select the contract with the same ID as the currOrder add the trip line to it
                    {                           // and update the remaining quantity on the contract.
                        c.Trips.Add(tmpTL);
                        c.RemainingQuantity = currQntRem;
                    }
                }
            }

            // Update the observable collections so the user interface can display the changes to the trip details
            currOrderTripDet = new ObservableCollection<List<string>> ( getTripDetails( currOrder[0].Trips) );
            OrderTrips.ItemsSource = currOrderTripDet;
        }



        //  METHOD:		btnFinalize_Click
        /// \brief Method that is called when the finalize button is clicked
        /// \details <b>Details</b>
        ///     Saves all the trip lines in the current contract to the database.  Also search the truck list for any trucks
        /// that need to be added to the database and adds them.  Then changes the state of the current contract to in progress
        /// sending it to the next tab.
        ///
        /// \param - <b>sender:</b>  the object that called the method
        /// \param - <b>e:</b>       the arguments that are passed when this method is called
        ///
        /// \return - <b>Nothing</b>
        ///
        private void btnFinalize_Click ( object sender,RoutedEventArgs e ) {

            foreach ( TripLine t in currOrder[0].Trips ) {
                Controller.SaveTripLineToDB ( t );
            }

            foreach ( Truck t in Trucks) {
                if ( t.Contracts.First().ContractID == currOrder[0].ID ) {
                    Controller.SaveTripToDB ( t );
                }
            }

            currOrder[0].Status = "IN-PROGRESS";
            Dictionary<string, string> values = new Dictionary<string, string>();
            values.Add("status", currOrder[0].Status);
            Dictionary<string, string> conditions = new Dictionary<string, string>();
            conditions.Add("contractID", currOrder[0].ID.ToString());
            Controller.TMS.MakeUpdateCommand("contract",values,conditions);
            Controller.TMS.ExecuteCommand();

            GetContracts();

            btnFinalize.IsEnabled = false;
        }



        //  METHOD:		getTripDetails
        /// \brief Method that gets all the needed details for the trip detail box
        /// \details <b>Details</b>
        ///     Gets the name of the carrier that owns the truck that the trip line is on and adds that to a list with
        /// the truck id and the quantity of the contract on that truck and adds this list to a list.
        ///
        /// \param - <b>sender:</b>  the object that called the method
        /// \param - <b>e:</b>       the arguments that are passed when this method is called
        ///
        /// \return - <b>List<List<string>></b> holding the details of all the trips in a contract
        ///
        private List<List<string>> getTripDetails ( List <TripLine> trips ) {
            List<List<string>> details = new List<List<string>>();
            List<string> item;

            foreach ( TripLine t in trips ) {
                // for each trip line in a list of trips get the required information
                item = new List<string>();
                string carrierName = " Truck not found ";
                string carrierID = "";

                bool found = false;

                // Get the carrier id from the truck that the tripline is on
                for ( int i = 0; i < Trucks.Count && !found; i++ ) {
                    if ( Trucks[i].TripID == t.TripID ) {
                        carrierID = Trucks[i].CarrierID.ToString();
                        found = true;
                    }
                }

                found = false;
                // Get the carrier name from the carrier matching the carrier id of the truck
                for ( int i = 0; i < Carriers.Count && !found; i++ ) {
                    if ( Carriers[i].CarrierID.ToString().Equals ( carrierID ) ) {
                        carrierName = Carriers[i].CarrierName;
                        found = true;
                    }
                }

                // add everything to the lists and return the result
                item.Add(t.TripID.ToString());
                item.Add(carrierName);
                item.Add(t.Quantity.ToString());

                details.Add ( item );
            }

            return details;
        }



        //  METHOD:		Nullify_SelectionChanged
        /// \brief Method that is called when a selectable item is selected that has no actions when selected
        /// \details <b>Details</b>
        ///     This method was created solely to stop the selection of one item from clearing the selection of other items.
        /// If an item does not need to have actions when it is selected, like a list box containing datails, is selected, this
        /// method stops it from clearing all other selected items.
        ///
        /// \param - <b>sender:</b>  the object that called the method
        /// \param - <b>e:</b>       the arguments that are passed when this method is called
        ///
        /// \return - <b>Nothing</b>
        ///
        private void Nullify_SelectionChanged ( object sender,SelectionChangedEventArgs e ) {
            e.Handled = true;
        }



        /* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ */
        /* ~~~~~ Methods for contracts in In-Progress stage ~~~~~ */
        /* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ */

        //  METHOD:		btnCompleteContract_Click
        /// \brief Method that is called when the complete contract button is clicked
        ///     Sets the status of the currently selected order to complete if all of its trip lines have
        /// been delivered.  This allows the buyer to finish of the contract.
        ///
        /// \param - <b>sender:</b>  the object that called the method
        /// \param - <b>e:</b>       the arguments that are passed when this method is called
        ///
        /// \return - <b>Nothing</b>
        ///
        private void btnCompleteContract_Click ( object sender,RoutedEventArgs e ) {
            if (SummaryList.SelectedIndex > -1)
            {
                Contract c = (Contract)SummaryList.SelectedItem;
                c.Status = "COMPLETE";
                Dictionary<string, string> values = new Dictionary<string, string>();
                values.Add("status", c.Status);
                Dictionary<string, string> conditions = new Dictionary<string, string>();
                conditions.Add("contractID", c.ID.ToString());
                Controller.TMS.MakeUpdateCommand("contract", values, conditions);
                Controller.TMS.ExecuteCommand();
                SummaryList.ItemsSource = null;
                SummaryList.ItemsSource = ordersCollection;
                GetContracts();

            }

        }



        /* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ */
        /* ~~~~~ Methods for Advancing Time ~~~~~ */
        /* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ */

        //  METHOD:		GenRep_Click
        /// \brief Method that is called when the Generate Report button is clicked
        /// \details <b>Details</b>
        ///     This method sets up the title of the reports tab depending on the type of report being generated
        /// and then generates a report for the specified time.  Then calls a helper method which puts the report
        /// file onto the page.
        ///
        /// \param - <b>sender:</b>  the object that called the method
        /// \param - <b>e:</b>       the arguments that are passed when this method is called
        ///
        /// \return - <b>Nothing</b>
        ///
        private void GenRep_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            int weeks = SummaryTimeFrame.SelectedIndex;
            string report = "";

            switch(weeks)
            {
                case 0:
                    weeks = 2;
                    ReportsTtl.Content = "TMS Reports - Past 2 Weeks";
                    report = Controller.GenerateReport(weeks);
                    Get2wReports();
                    break;

                case 1:
                    ReportsTtl.Content = "TMS Reports - All Time";
                    report = Controller.GenerateReport();
                    GetAtReports();
                    break;

                default:
                    ReportsTtl.Content = "TMS Reports";
                    break;
            }
        }



        //  METHOD:		Get2wReports
        /// \brief This method gets a 2 week report from the database and displays it to the screen
        /// \details <b>Details</b>
        ///     This method first receives 2 week reports from the database then parses them and reorders them
        /// for display in the output box.
        ///
        /// \param - <b>None</b>
        ///
        /// \return - <b>Nothing</b>
        ///
        private void Get2wReports()
        {
            ReportBlock.Text = string.Empty;

            // Gets the reports from the database
            List<string> sqlReturn = new List<string>();
            List<string> fields = new List<string>();
            Dictionary<string, string> conditions = new Dictionary<string, string>();

            fields.Add("*");
            conditions.Add("type", "2-week");
            Controller.TMS.MakeSelectCommand(fields, "report", conditions, null);

            sqlReturn = Controller.TMS.ExecuteCommand();

            if(sqlReturn == null || sqlReturn.Count == 0)
            {
                ReportBlock.Text = "No Reports to Display.\n";
                return;
            }

            // Parses and displays the reports if any were found
            foreach(string s in sqlReturn)
            {
                string[] split = s.Split(',');
                StringBuilder sb = new StringBuilder();
                double tmpCost = Math.Round(double.Parse(split[5]), 2);

                sb.Append("TMS Internal Report:\n\n");
                sb.AppendFormat("Period: {0} - {1}\n", split[2], split[3]);
                sb.AppendFormat("Total Contracts Delivered: {0}, Total Invoice Cost: {1}\n\n", split[4], tmpCost.ToString()); ;

                ReportBlock.Text += sb.ToString();
            }
        }



        //  METHOD:		GetAtReports
        /// \brief This method gets all time reports from the database and displays them to the screen
        /// \details <b>Details</b>
        ///     This method first receives reports of the everything that the program has done in its lifetime from the database
        /// then parses them and reorders them for display in the output box.
        ///
        /// \param - <b>None</b>
        ///
        /// \return - <b>Nothing</b>
        ///
        private void GetAtReports()
        {
            ReportBlock.Text = string.Empty;

            // Gets the reports from the database
            List<string> sqlReturn = new List<string>();
            List<string> fields = new List<string>();
            Dictionary<string, string> conditions = new Dictionary<string, string>();

            fields.Add("*");
            conditions.Add("type", "All-Time");
            Controller.TMS.MakeSelectCommand(fields, "report", conditions, null);

            sqlReturn = Controller.TMS.ExecuteCommand();

            if (sqlReturn == null || sqlReturn.Count == 0)
            {
                ReportBlock.Text = "No Reports to Display.\n";
                return;
            }

            // Parses and displays the reports if any were found
            foreach (string s in sqlReturn)
            {
                string[] split = s.Split(',');
                StringBuilder sb = new StringBuilder();
                double tmpCost = Math.Round(double.Parse(split[5]), 2);

                sb.Append("TMS Internal Report:\n\n");
                sb.AppendFormat("Period: {0}\n", split[1]);
                sb.AppendFormat("Total Contracts Delivered: {0}, Total Invoice Cost: {1}\n\n", split[4], tmpCost.ToString());

                ReportBlock.Text += sb.ToString();
            }
        }



        //  METHOD:		AdvTimeBtn_Click
        /// \brief Method that is called when the advance time button is clicked, advancing the time by 1 day.
        /// \details <b>Details</b>
        ///     This method simulates the passage of time by virtually moving all currently ready trucks allong their trip,
        /// adding time for stops at each city.  It also calculates the price of the trip as the trucks travel.
        ///
        /// \param - <b>sender:</b>  the object that called the method
        /// \param - <b>e:</b>       the arguments that are passed when this method is called
        ///
        /// \return - <b>Nothing</b>
        ///
        private void AdvTimeBtn_Click(object sender, RoutedEventArgs e)
        {
            time.AddDays(1.0); // add a day to the global time variable
            // Advance each truck by one day
            foreach(Truck t in Trucks)
            {
                if(!t.IsComplete) // only advance time on trucks that are not yet complete
                {
                    // first correct the time of all triplines on a truck
                    if(t.Corrected == false)
                    {
                        t.CorrectContractTime();
                        t.Corrected = true;
                    }

                    // Now go through each tripline and take away the first array element of the hours per day array
                    foreach (TripLine tl in t.Contracts)
                    {
                        if(!tl.IsDelivered) // only advance time for undelivered contracts
                        {
                            if (tl.HoursPerDay[0] == 0.00) // day one was already advanced
                            {
                                if (tl.HoursPerDay[1] == 0.00) // day two was already advanced
                                {
                                    tl.HoursPerDay[2] = 0.00; // set the third to 0.
                                }
                                else // set the second day to 0
                                {
                                    tl.HoursPerDay[1] = 0.00;
                                }
                            }
                            else // take away the first day
                            {
                                tl.HoursPerDay[0] = 0.00;
                            }

                            // Check through all days in the hours per day and make sure they're all 0 for the tripline to be
                            // considered done
                            int daysDone = 0;
                            for (int i = 0; i < tl.HoursPerDay.Length; i++)
                            {
                                if (tl.HoursPerDay[i] == 0.00)
                                {
                                    daysDone++;
                                }
                            }

                            // if all the elements in the array are 0.00 the tripline is complete
                            if (daysDone == tl.HoursPerDay.Length)
                            {
                                tl.IsDelivered = true;
                                foreach(Contract c in Contracts)
                                {
                                    if(tl.ContractID == c.ID)
                                    {
                                        foreach(TripLine trip in c.Trips)
                                        {
                                            if(tl.TripID == trip.TripID)
                                            {
                                                trip.IsDelivered = true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                }
                //Check through all the triplines again to see if they're all delivered
                bool truckIsComplete = true;
                foreach(TripLine tl in t.Contracts) // if even just one tripline isn't delivered the truck won't be set to complete
                {
                    if(!tl.IsDelivered)
                    {
                        truckIsComplete = false;
                    }
                }

                if(truckIsComplete)
                {
                    t.IsComplete = true;
                }

                foreach (Contract c in Contracts)
                {
                    IsContractComplete(c);

                    foreach(TripLine tl in c.Trips)
                    {
                        if (t.TripID == tl.TripID && c.TripComplete == true)
                        {
                            c.CalculateCost(t);
                        }
                    }
                }
            }
            ordersCollection.Clear();

            foreach(Contract c in Contracts)
            {
                ordersCollection.Add(c);
            }

            SummaryList.ItemsSource = null;
            SummaryList.ItemsSource = ordersCollection;
        }



        //  METHOD:		IsContractComplete
        /// \brief Method that is called to check if a contract is ready to complete.
        /// \details <b>Details</b>
        ///     This method checks each trip attatched to the contract to see if they are all complete and if so
        /// returns true.
        ///
        /// \param - <b>contract:</b>  the contract to check for completion
        ///
        /// \return - <b>boolean</b> saying whether the contract is complete
        ///
        private bool IsContractComplete(Contract contract)
        {
            bool complete = contract.IsContractComplete();
            if (complete)
            {
                contract.TripComplete = true;
            }
            return complete;
        }



        //  METHOD:		SummaryList_SelectionChanged
        /// \brief Method that is called when an order is selected on the summary tab.
        /// \details <b>Details</b>
        ///     This method thecks if the selected order is ready for completion and, if so, activates the complete button.
        ///
        /// \param - <b>sender:</b>  the object that called the method
        /// \param - <b>e:</b>       the arguments that are passed when this method is called
        ///
        /// \return - <b>Nothing</b>
        ///
        private void SummaryList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            e.Handled = true;
            Contract c = (Contract)SummaryList.SelectedItem;
            if (c!= null)
            {
                if ((c.Status == "IN-PROGRESS") && (c.TripComplete == true))
                {
                    CompleteContract.IsEnabled = true;
                }
                else
                {
                    CompleteContract.IsEnabled = false;
                }
            }

        }
    }
}
