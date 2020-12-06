﻿using System;
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
    /// \brief This class holds all the event handlers for for the Planner WPF window.  It has data members for the TMS database & the Marketplace database.
    /// The error handling in this class will be handled in the form of message boxes describing the errors that happen for majour errors, and error text indicating
    /// simpler errors. The testing for this class will be mainly done manually as this is the most efficient way to access the event handlers in the way that they
    /// will be used in the final program.
    ///
    /// \author <i>Deric Kruse</i>
    ///
    public partial class PlannerWindow : Window
    {
        //! Properties
        private bool orderSelected { get; set; }
        private string userName { get; set; }                                         //<Stores the user name of the current user
        ObservableCollection<Contract> ordersCollection { get; set; }
        ObservableCollection<Contract> currOrder { get; set; }
        ObservableCollection<Carrier>  currCarrier { get; set; }

        public PlannerWindow ( string name ) {
            InitializeComponent();

            userName = name;
            orderSelected = false;
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


        /////////////////////////////////////////////////////////////////
        

        private void TabsCtrl_Planner_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            e.Handled = true;

            GetOrders();

            if (Orders.IsSelected)
            {
                OrderList.ItemsSource = ordersCollection;
            }
            else if (Summary.IsSelected)
            {
                SummaryList.ItemsSource = ordersCollection;
            }
        }

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

        private void OrderList_SelectionChanged ( object sender,SelectionChangedEventArgs e ) 
        {
            e.Handled = true;

            OrderDetails.ItemsSource = null;
            CarrierSelector.Items.Clear() ;

            currOrder = new ObservableCollection<Contract>();

            if ( OrderList.SelectedIndex != -1 )
            {
                currOrder.Add( (Contract) OrderList.SelectedItem );
                orderSelected = true;

                List <Carrier> carriersLst = Controller.SetupCarriers();
                List <string> availCarriers = Controller.FindCarriersForContract( (Contract)currOrder.ElementAt(0), carriersLst );

                foreach ( string s in availCarriers ) {
                    CarrierSelector.Items.Add (s);
                }
            }
            else
            {
                orderSelected = false;
            }

            OrderDetails.ItemsSource = currOrder;

            ShowOrderControls (orderSelected);
        }

        private void ShowOrderControls ( bool doShow ) {

            if ( doShow ) 
            {
                CarrierSelLBL.Visibility = Visibility.Visible;
                CarrierSelector.Visibility = Visibility.Visible;
                CarrierDetails.Visibility = Visibility.Visible;
            }
            else 
            {
                CarrierSelLBL.Visibility = Visibility.Collapsed;
                CarrierSelector.Visibility = Visibility.Collapsed;
                CarrierDetails.Visibility = Visibility.Collapsed;
            }
        }

        private void CarrierSelector_SelectionChanged ( object sender,SelectionChangedEventArgs e ) 
        {    
            e.Handled = true;
            CarrierDetails.ItemsSource = null;
            currCarrier = new ObservableCollection<Carrier>();

            if ( OrderList.SelectedIndex != -1 )
            {
                Dictionary<string, string> conditions = new Dictionary<string, string>();
                conditions.Add( "carrierName", ((string) CarrierSelector.SelectedItem).Split(',').ElementAt(0) );

                List <string> currStrCarrier = new List<string>( Controller.GetCarriersFromTMS( null, conditions )[0].Split(',') );

                currCarrier.Add( new Carrier (currStrCarrier) );
                //orderSelected = true;

                /*List <Carrier> carriersLst = Controller.SetupCarriers();
                List <string> availCarriers = Controller.FindCarriersForContract( (Contract)currOrder.ElementAt(0), carriersLst );

                foreach ( string s in availCarriers ) {
                    CarrierSelector.Items.Add (s);
                }//*/
            }
            else
            {
                //orderSelected = false;
            }

            CarrierDetails.ItemsSource = currCarrier;

            ShowOrderControls (orderSelected);
        }
    }
}
