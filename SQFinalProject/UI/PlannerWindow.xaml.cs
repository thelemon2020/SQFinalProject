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
        ObservableCollection<List<string>> currOrderTripDet { get; set; }            //<Collection of tripLines for databinding to the order trips list
        ObservableCollection<Truck> truckCollection {get;set;}                      //<Collection of trucks for databinding to the truck selection dropdown list

        ObservableCollection<Contract> planningCollection { get; set; }             //<Collection of contracts for databinding to the order selection list
        List<Truck> Trucks { get; set; }
        List<Contract> Contracts { get; set; }
        List<Carrier> Carriers { get; set; }

        private int currQntRem { get; set; }



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


        //  METHOD:		LoadRates
        /// \brief Gets Rates Information from DB
        /// \details <b>Details</b>
        ///     Creates a SELECT query string to grab all data from the rates table and then loads them into approrpriate textbox
        ///
        /// \param - <b>None</b>
        /// 
        /// \return - <b>Nothing</b>
        ///
        private void GetContracts()
        {
            Contracts = new List<Contract>();
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
                List<string> field = new List<string>();
                field.Add("*");
                Dictionary<string, string> conditions = new Dictionary<string, string>();
                conditions.Add("contractid", c.ID.ToString());
                Controller.TMS.MakeSelectCommand(field, "tripline", conditions,null);
                results = Controller.TMS.ExecuteCommand();
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

                    if ( currOrder[0].Trips == null ) {
                        currOrder[0].Trips = new List<TripLine>();
                    }

                    if ( currOrder[0].JobType == 0 && currOrder[0].Trips.Count == 0  ) {
                        btnFinalize.IsEnabled = false;
                    } else if ( currQntRem != 0 ) {
                        btnFinalize.IsEnabled = false;
                    } else {
                        btnFinalize.IsEnabled = true;
                    }

                    currOrderTripDet = new ObservableCollection<List<string>> ( getTripDetails( currOrder[0].Trips) );
                    OrderTrips.ItemsSource = currOrderTripDet;
                }
            }

            OrderDetails.ItemsSource = currOrder;

            EnableOrderControls (OrderState);
        }

        //  METHOD:		LoadRates
        /// \brief Gets Rates Information from DB
        /// \details <b>Details</b>
        ///     Creates a SELECT query string to grab all data from the rates table and then loads them into approrpriate textbox
        ///
        /// \param - <b>None</b>
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
               
                Dictionary<string, string> conditions = new Dictionary<string, string>();
                conditions.Add( "carrierName", ((string) CarrierSelector.SelectedItem).Split(',').ElementAt(0) );

                List <string> currStrCarrier = new List<string>( Controller.GetCarriersFromTMS( null, conditions )[0].Split(',') );

                currCarrier.Add( new Carrier (currStrCarrier) );
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

        private void TruckSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            e.Handled = true;
            if ( TruckSelector.SelectedIndex != -1 && TruckSelector.SelectedItem != null )
            {
  
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
        private void AddTruck_Click ( object sender, RoutedEventArgs e ) 
        {
            int truckLoad = 0;
            OrderTrips.ItemsSource = null;

            if ( currOrder[0].JobType == 0 ) {

                btnAddTruck.IsEnabled = false;
                btnFinalize.IsEnabled = true;

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

                t.AddContract( new TripLine (currOrder[0], t.TripID, truckLoad) );
                currOrder[0].Trips.Add ( t.Contracts.Last() );
                currOrder[0].Quantity = currQntRem;

                QntRem.Text = currQntRem.ToString();
                TruckRem.Text = t.RemainingQuantity().ToString();
            }

            currOrderTripDet = new ObservableCollection<List<string>> ( getTripDetails( currOrder[0].Trips) );
            OrderTrips.ItemsSource = currOrderTripDet;
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


        private List<List<string>> getTripDetails ( List <TripLine> trips ) {
            List<List<string>> details = new List<List<string>>();
            List<string> item;
            
            List<string> QueryLst = new List<string> ();   // Set up the database query and check if the user name exists in the database
            QueryLst.Add ("carrierID");
            Dictionary<string, string> tempDict;
            List<string> retList;

            foreach ( TripLine t in trips ) {
                item = new List<string>();
                string carrierName = " Truck not found ";
                string carrierID = "";
                
                bool found = false;

                tempDict = new Dictionary<string, string>();
                tempDict.Add ("tripID", t.TripID.ToString());

                Controller.TMS.MakeSelectCommand ( QueryLst, "truck", tempDict,null);

                retList = Controller.TMS.ExecuteCommand();

                if ( retList.Count() != 0 ) {
                    carrierID = retList.ElementAt(0);
                } else {
                    for ( int i = 0; i < Trucks.Count && !found; i++ ) {
                        if ( Trucks[i].TripID == t.TripID ) {
                            carrierID = Trucks[i].CarrierID.ToString();
                            found = true;
                        }
                    }
                }
                
                found = false;
                for ( int i = 0; i < Carriers.Count && !found; i++ ) {
                    if ( Carriers[i].CarrierID.ToString().Equals ( carrierID ) ) {
                        carrierName = Carriers[i].CarrierName;
                        found = true;
                    }
                }

                item.Add(t.TripID.ToString());
                item.Add(carrierName);
                item.Add(t.Quantity.ToString());

                details.Add ( item );
            }

            return details;
        }

        private void Nullify_SelectionChanged ( object sender,SelectionChangedEventArgs e ) {
            e.Handled = true;
        }



        /* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ */
        /* ~~~~~ Methods for contracts in In-Progress stage ~~~~~ */
        /* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ */

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
        private void btnCompleteContract_Click ( object sender,RoutedEventArgs e ) {

            if (SummaryList.SelectedIndex > -1)
            {
                int i = SummaryList.SelectedIndex;


            }
            currOrder[0].Status = "COMPLETE";
            Dictionary<string, string> values = new Dictionary<string, string>();
            values.Add("status", currOrder[0].Status);
            Dictionary<string, string> conditions = new Dictionary<string, string>();
            conditions.Add("contractID", currOrder[0].ID.ToString());
            Controller.TMS.MakeUpdateCommand("contract",values,conditions);
            Controller.TMS.ExecuteCommand();

            GetContracts();
        }


        /* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ */
        /* ~~~~~ Methods for Advancing Time ~~~~~ */
        /* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ */

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

        private void Get2wReports()
        {
            ReportBlock.Text = string.Empty;

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

            foreach(string s in sqlReturn)
            {
                string[] split = s.Split(',');
                StringBuilder sb = new StringBuilder();

                sb.Append("TMS Internal Report:\n\n");
                sb.AppendFormat("Period: {0} - {1}\n", split[2], split[3]);
                sb.AppendFormat("Total Contracts Delivered: {0}, Total Invoice Cost: {1}\n\n", split[4], split[5]);

                ReportBlock.Text += sb.ToString();
            }
        }

        private void GetAtReports()
        {
            ReportBlock.Text = string.Empty;

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

            foreach (string s in sqlReturn)
            {
                string[] split = s.Split(',');
                StringBuilder sb = new StringBuilder();

                sb.Append("TMS Internal Report:\n\n");
                sb.AppendFormat("Period: {0}\n", split[1]);
                sb.AppendFormat("Total Contracts Delivered: {0}, Total Invoice Cost: {1}\n\n", split[4], split[5]);

                ReportBlock.Text += sb.ToString();
            }
        }


        private void AdvTimeBtn_Click(object sender, RoutedEventArgs e)
        {
            // Advance each truck by one day
            foreach(Truck t in Trucks)
            {
                if(!t.IsComplete) // only advance time on trucks that are not yet complete
                {
                    // first correct the time of all triplines on a truck 
                    t.CorrectContractTime();

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
            }
        }

        private void SummaryList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            e.Handled = true;
            Contract c = (Contract)SummaryList.SelectedItem;
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
