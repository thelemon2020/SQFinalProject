using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SQFinalProject.ContactMgmtBilling;
using SQFinalProject.TripPlanning;

namespace SQFinalProject.UI {
    ///
    /// \class MainWindow
    /// 
    /// \brief This class holds all the event handlers for for the main WPF window.  It has data members for the TMS database & the Marketplace database.
    /// The error handling in this class will be handled in the form of message boxes describing the errors that happen for majour errors, and error text indicating
    /// simpler errors. The testing for this class will be mainly done manually as this is the most efficient way to access the event handlers in the way that they 
    /// will be used in the final program.
    /// 
    /// \author <i>Deric Kruse, Chris Lemon</i>
    /// 
    public partial class AdminWindow : Window {
        //! Properties
        public string DBBackUp { get; set; }
        private ObservableCollection<Carrier> carrierCollection { get;set;}
        private ObservableCollection<RouteCity> cityCollection { get; set; }
        public string userName;                                         //<Stores the user name of the current user

        public AdminWindow(string name)
        {
            InitializeComponent();
            Controller.LoadConfig();                                               // Load databases from config file
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
        private void CloseCB_CanExecute(object sender, CanExecuteRoutedEventArgs e) {
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
        private void CloseCB_Executed(object sender, ExecutedRoutedEventArgs e) {
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
        private void Logout_Click(object sender, RoutedEventArgs e) {
            LoginWindow login = new LoginWindow();
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
        private void About_Click(object sender, RoutedEventArgs e) {
            AboutWindow aboutBox = new AboutWindow();
            aboutBox.Owner = this;
            aboutBox.ShowDialog();
        }


        //  METHOD:		TMSChange_Click
        /// \brief Changes the TMS Database settings stored in the config file
        /// \details <b>Details</b>
        ///     Creates a new string that contains the updated information and overwrites the old text file
        /// 
        /// \param - <b>sender:</b>  the object that called the method
        /// \param - <b>e:</b>       the arguments that are passed when this method is called
        /// 
        /// \return - <b>Nothing</b>
        ///
        private void TMSChange_Click(object sender, RoutedEventArgs e)
        {
            IPAddress temp;
            int temp1;
            if (IPAddress.TryParse(TMS_IP.Text, out temp) && (int.TryParse(TMS_Port.Text, out temp1)))
            {
                string newWrite = "TMS " + TMS_IP.Text + " " + TMS_Port.Text + " " + TMS_User.Text + " " + TMS_Password.Password.ToString() + " " + Controller.TMS.schema + "\n" 
                    + "MP " + Controller.MarketPlace_Database[0] + " " + Controller.MarketPlace_Database[1] + " " + Controller.MarketPlace_Database[2] + " " + Controller.MarketPlace_Database[3] + " " + Controller.MarketPlace_Database[4] + "\n" + "LOGGER " + Logger.path + "\n" + "BACKUP " + DBBackUp; 
                StreamWriter sw = new StreamWriter(Controller.ConfigPath,false);
                sw.Write(newWrite);
                sw.Close();
                Controller.TMS_Database = null;
                Controller.MarketPlace_Database = null;
                Controller.LoadConfig();
            }
            else
            {
                System.Windows.MessageBox.Show("Connection Information Not Formatted properly", "Bad Input", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
        }


        //  METHOD:		MPChange_Click
        /// \brief Changes the Market Place Database settings stored in the config file
        /// \details <b>Details</b>
        ///     Creates a new string that contains the updated information and overwrites the old text file
        /// 
        /// \param - <b>sender:</b>  the object that called the method
        /// \param - <b>e:</b>       the arguments that are passed when this method is called
        /// 
        /// \return - <b>Nothing</b>
        ///
        private void MPChange_Click(object sender, RoutedEventArgs e)
        {
            IPAddress temp;
            int temp1;

            if (IPAddress.TryParse(TMS_IP.Text, out temp) && (int.TryParse(TMS_Port.Text, out temp1)))
            {
                string newWrite = "MP " + MP_IP.Text + " " + MP_Port.Text + " " + MP_User.Text + " " + MP_Password.Password.ToString() + " " + Controller.TMS.schema + "\n"
                    + "MP " + Controller.TMS_Database[0] + " " + Controller.TMS_Database[1] + " " + Controller.TMS_Database[2] + " " + Controller.TMS_Database[3] + " " + Controller.TMS_Database[4] + "\n" + "LOGGER " + Logger.path + "\n" + "BACKUP " + DBBackUp;
                StreamWriter sw = new StreamWriter(Controller.ConfigPath, false);
                sw.Write(newWrite);
                sw.Close();
                Controller.TMS_Database = null;
                Controller.MarketPlace_Database = null;
                Controller.TMS = null;
                Controller.MarketPlace = null;
                Controller.LoadConfig();
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Connection Information Not Formatted properly", "Bad Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //  METHOD:		TabsCtrl_SelectionChange
        /// \brief Governs the behavior of the Admin window
        /// \details <b>Details</b>
        ///     Checks which tab is selected and loads the required data to fill out that t
        /// 
        /// \param - <b>None</b>
        /// 
        /// \return - <b>Nothing</b>
        ///
        private void TabsCtrl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Config.IsSelected)
            {
                FillFields();
            }
            else if(Log.IsSelected)
            {
                LoadLogger();
            }
            else if (CarrierTab.IsSelected)
            {
                LoadCarriers();
                UpdateCarrierComboBox();
            }
            else if (RouteTab.IsSelected)
            {
                LoadCities();
                UpdateRouteComboBox();
            }
            else if (RateTab.IsSelected)
            {
                LoadRates();
            }
            else if (BackUpTab.IsSelected)
            {
                BackUpPath.Text = DBBackUp;
            }
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
        private void LoadRates()
        {
            List<string> field = new List<string>();
            field.Add("*");
            Controller.TMS.MakeSelectCommand(field, "rates", null, null);
            List<string> returns = Controller.TMS.ExecuteCommand();
            string[] rates = returns[0].Split(',');
            FTLRate.Text = rates[0].ToString();
            LTLRate.Text = rates[1].ToString();
        }

        //  METHOD:		LoadCities
        /// \brief Gets City Information from DB and loads it into table
        /// \details <b>Details</b>
        ///     Calls a method that loads the cities data from DB and then sets the datacontext of the datagrid to that of the city collection
        ///
        /// \param - <b>None</b>
        /// 
        /// \return - <b>Nothing</b>
        ///
        private void LoadCities()
        {
            cityCollection = GetCityData();
            RouteData.DataContext = cityCollection;
        }

        //  METHOD:		LoadCities
        /// \brief Gets City Information from DB and loads it into an Observable collection
        /// \details <b>Details</b>
        ///     Generates a SELECT query to get all routecity entries from the route database.  Adds to observable collection
        ///
        /// \param - <b>None</b>
        /// 
        /// \return - cityCollection -<b>ObservableCollection<RouteCity></b> - a collection to be used to fill a data grid
        ///
        private ObservableCollection<RouteCity> GetCityData()
        {
            cityCollection = new ObservableCollection<RouteCity>();
            List<string> fields = new List<string>();
            fields.Add("*");
            Dictionary<string, string> order = new Dictionary<string, string>();
            order.Add("routeID", "ASC");
            Controller.TMS.MakeSelectCommand(fields, "route", null, order);
            fields = Controller.TMS.ExecuteCommand();
            foreach (string field in fields)
            {
                string[] columns = field.Split(',');
                int cityID;
                int.TryParse(columns[0], out cityID);
                int kmToEast;
                int kmToWest;
                double hToEast;
                double hToWest;
                int.TryParse(columns[2], out kmToEast);
                int.TryParse(columns[3], out kmToWest);
                double.TryParse(columns[4], out hToEast);
                double.TryParse(columns[5], out hToWest);
                RouteCity c = new RouteCity(cityID, columns[1], kmToEast, kmToWest, hToEast, hToWest,columns[6], columns[7]);
                c.newlyCreated = false;
                cityCollection.Add(c);
            }
            return cityCollection;
        }

        //  METHOD:		LoadCarriers
        /// \brief Gets Carrier Information from DB and loads it into table
        /// \details <b>Details</b>
        ///     Calls a method that loads the carrier data from DB and then sets the datacontext of the datagrid to that of the carrier collection
        ///
        /// \param - <b>None</b>
        /// 
        /// \return - <b>Nothing</b>
        ///
        private void LoadCarriers()
        {
            carrierCollection = GetCarrierData();
            CarrierData.DataContext = carrierCollection;
        }

        //  METHOD:		GetCarrierData
        /// \brief Gets City Information from DB and loads it into an observable collection
        /// \details <b>Details</b>
        ///     Generates a SELECT query to get all carrier entries from the carrier database.  Adds to observable collection
        ///
        /// \param - <b>None</b>
        /// 
        /// \return - carrierCollection -<b>ObservableCollection<RouteCity></b> - a collection to be used to fill a data grid
        ///
        private ObservableCollection<Carrier> GetCarrierData()
        {
            carrierCollection = new ObservableCollection<Carrier>();
            List<Carrier> tmpCarriers = Controller.SetupCarriers();
            foreach(Carrier c in tmpCarriers)
            {
                carrierCollection.Add(c);
            }
            return carrierCollection;
        }

        //  METHOD:		LoadLogger
        /// \brief Opens logger file and displays it on the screen
        /// \details <b>Details</b>
        ///     Calls a logger method that returns the string of whatever is in the log file
        ///
        /// \param - <b>None</b>
        /// 
        /// \return - <b>Nothing</b>
        ///
        private void LoadLogger()
        {
            FilePath.Text = Logger.path;
            string logFile = Logger.ReadLog();
            if (logFile == null)
            {
                Log_Viewer.Content = "Log File Failed To Open";
            }
            else
            {
                Log_Viewer.Content = logFile;
            }
        }
        //  METHOD:		FillFields
        /// \brief Sets up the default fields in the config panel
        /// \details <b>Details</b>
        ///     Copies from database properties to fill in default field values
        ///
        /// \param - <b>None</b>
        /// 
        /// \return - <b>Nothing</b>
        ///
        private void FillFields()
        {
            TMS_IP.Text = Controller.TMS.ip;
            TMS_Port.Text = Controller.TMS.port;
            TMS_User.Text = Controller.TMS.user;
            TMS_Password.Password = Controller.TMS.pass;
            MP_IP.Text = Controller.MarketPlace.ip;
            MP_Port.Text = Controller.MarketPlace.port;
            MP_User.Text = Controller.MarketPlace.user;
            MP_Password.Password = Controller.MarketPlace.pass;
        }

        //  METHOD:		ChangePath_Click
        /// \brief Changes the Logger file path
        /// \details <b>Details</b>
        ///     Opens an OpenFIleDialog window to allow the user to change the location of the logger file
        /// 
        /// \param - <b>sender:</b>  the object that called the method
        /// \param - <b>e:</b>       the arguments that are passed when this method is called
        /// 
        /// \return - <b>Nothing</b>
        ///
        private void ChangePath_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();
            of.InitialDirectory = Logger.path;
            of.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            of.FilterIndex = 2;
            if (of.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string temp = Logger.path;   
                Logger.path = of.FileName;
                string configContents;
                using (StreamReader sr = new StreamReader(Controller.ConfigPath))
                {
                    configContents = sr.ReadToEnd();
                }
                string toWrite = configContents.Replace(temp, Logger.path);
                using (StreamWriter sw = new StreamWriter(Controller.ConfigPath, false))
                {
                    sw.Write(toWrite);
                }
                LoadLogger();
            }
        }

        //  METHOD:		Create_Click
        /// \brief Adds a new row to the datagrid
        /// \details <b>Details</b>
        ///     Creates a new Carrier object and adds it to the carrier collection and updates the combo box
        ///
        /// \param - <b>sender:</b>  the object that called the method
        /// \param - <b>e:</b>       the arguments that are passed when this method is called
        /// 
        /// \return - <b>Nothing</b>
        ///
        private void Create_Click(object sender, RoutedEventArgs e)
        {
            Carrier c = new Carrier();
            c.newlyCreated = true;
            carrierCollection.Add(c);
            UpdateCarrierComboBox();
        }

        //  METHOD:		UpdateRouteComboBox
        /// \brief Makes sure the combo box is up to date
        /// \details <b>Details</b>
        ///     Clears the combo box and adds all ids in the city collection to the combo box
        ///
        /// \param - <b>None</b>
        /// 
        /// \return - <b>Nothing</b>
        ///
        private void UpdateRouteComboBox()
        {
            RouteCityList.Items.Clear();
            foreach (RouteCity c in cityCollection)
            {
                RouteCityList.Items.Add(c.routeID);
            }
        }

        //  METHOD:		UpdateCarrierComboBox
        /// \brief Makes sure the combo box is up to date
        /// \details <b>Details</b>
        ///     Clears the combo box and adds all ids in the carrier collection to the combo box
        ///
        /// \param - <b>None</b>
        /// 
        /// \return - <b>Nothing</b>
        ///
        private void UpdateCarrierComboBox()
        {
            DeleteCarrierList.Items.Clear();
            foreach (Carrier c in carrierCollection)
            {
                DeleteCarrierList.Items.Add(c.CarrierID);
            }
        }

        //  METHOD:		Delete_Click
        /// \brief Removes an entry in the data grid
        /// \details <b>Details</b>
        ///     Gets the index of the selected item from the combo box and removes it from the collection
        ///
        /// \param - <b>sender:</b>  the object that called the method
        /// \param - <b>e:</b>       the arguments that are passed when this method is called
        /// 
        /// \return - <b>Nothing</b>
        ///
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            int whichDelete = (int)DeleteCarrierList.SelectedIndex;
            carrierCollection.RemoveAt(whichDelete);
            UpdateCarrierComboBox();
        }

        //  METHOD:		DeleteCarrierList_SelectionChange
        /// \brief Checks that the delete button can be pressed
        /// \details <b>Details</b>
        ///     Checks that an actual value has been selected and if it has, enable the delete button
        ///
        /// \param - <b>sender:</b>  the object that called the method
        /// \param - <b>e:</b>       the arguments that are passed when this method is called
        /// 
        /// \return - <b>Nothing</b>
        ///
        private void DeleteCarrierList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            e.Handled = true;
            if (DeleteCarrierList.SelectedIndex.ToString() !="")
            {
                Delete.IsEnabled = true;
            }
            else
            {
                Delete.IsEnabled = false;
            }
        }

        //  METHOD:		Update_Click
        /// \brief Sends all data in the datagrid back to the database
        /// \details <b>Details</b>
        ///     Creates a series of inserts, update and delete query statements to send to the server to update the database
        ///
        /// \param - <b>sender:</b>  the object that called the method
        /// \param - <b>e:</b>       the arguments that are passed when this method is called
        /// 
        /// \return - <b>Nothing</b>
        ///
        private void Update_Click(object sender, RoutedEventArgs e)
        {          
            List<string> fields = new List<string>();
            fields.Add("carrierid");
            Controller.TMS.MakeSelectCommand(fields, "carrier", null, null);
            List<string> results = Controller.TMS.ExecuteCommand();
            foreach (string result in results)
            {
                bool toDelete = true;
                int temp;
                int.TryParse(result, out temp);
                foreach (Carrier c in carrierCollection)
                {
                    if (temp == c.CarrierID)
                    {
                        Dictionary<string, string> updateValues = new Dictionary<string, string>();
                        updateValues.Add("carrierID", c.CarrierID.ToString());
                        if (c.CarrierName.Contains("'"))
                        {
                            c.CarrierName = c.CarrierName.Replace("'", "''");
                        }
                        updateValues.Add("carriername", c.CarrierName);
                        updateValues.Add("ftlrate", c.FTLRate.ToString());
                        updateValues.Add("ltlrate", c.LTLRate.ToString());
                        updateValues.Add("reefcharge", c.ReefCharge.ToString());
                        Dictionary<string, string> conditions = new Dictionary<string, string>();
                        conditions.Add("carrierID", temp.ToString());
                        Controller.TMS.MakeUpdateCommand("carrier", updateValues, conditions);
                        Controller.TMS.ExecuteCommand();
                        toDelete = false;
                        break;
                    }
                }
                if (toDelete == true)
                {
                    Dictionary<string, string> conditions = new Dictionary<string, string>();
                    conditions.Add("carrierid", temp.ToString());
                    Controller.TMS.MakeDeleteCommand("carrier", conditions);
                    Controller.TMS.ExecuteCommand();
                }                                     
            }
            foreach (Carrier newC in carrierCollection)
            {
                if (newC.newlyCreated == true)
                {
                    List<string> values = new List<string>();
                    values.Add(newC.CarrierID.ToString());
                    if (newC.CarrierName.Contains("'"))
                    {
                        newC.CarrierName = newC.CarrierName.Replace("'", "''");
                    }
                    values.Add(newC.CarrierName);
                    values.Add(newC.FTLRate.ToString());
                    values.Add(newC.LTLRate.ToString());
                    values.Add(newC.ReefCharge.ToString());
                    Controller.TMS.MakeInsertCommand("carrier", values);
                    Controller.TMS.ExecuteCommand();
                }
            }
        }

        //  METHOD:		RouteDelete_Click
        /// \brief Removes an entry in the data grid
        /// \details <b>Details</b>
        ///     Gets the index of the selected item from the combo box and removes it from the collection
        ///
        /// \param - <b>sender:</b>  the object that called the method
        /// \param - <b>e:</b>       the arguments that are passed when this method is called
        /// 
        /// \return - <b>Nothing</b>
        //
        private void RouteDelete_Click(object sender, RoutedEventArgs e)
        {
            int whichDelete = (int)RouteCityList.SelectedIndex;
            cityCollection.RemoveAt(whichDelete);
            UpdateRouteComboBox();
        }

        //  METHOD:		CarrierData_AddingNewItem
        /// \brief If an item is added to the grid, update the combobox
        /// \details <b>Details</b>
        ///     Calls the UpdateCarrierComboBox() method if a new item is added
        ///
        /// \param - <b>sender:</b>  the object that called the method
        /// \param - <b>e:</b>       the arguments that are passed when this method is called
        /// 
        /// \return - <b>Nothing</b>
        //
        private void CarrierData_AddingNewItem(object sender, AddingNewItemEventArgs e)
        {
            UpdateCarrierComboBox();
        }

        //  METHOD:		DeleteCityList_SelectionChange
        /// \brief Checks that the delete button can be pressed
        /// \details <b>Details</b>
        ///     Checks that an actual value has been selected and if it has, enable the delete button
        ///
        /// \param - <b>sender:</b>  the object that called the method
        /// \param - <b>e:</b>       the arguments that are passed when this method is called
        /// 
        /// \return - <b>Nothing</b>
        ///
        private void DeleteCityList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            e.Handled = true;
            if (RouteCityList.SelectedIndex.ToString() != "")
            {
                RouteDelete.IsEnabled = true;
            }
            else
            {
                RouteDelete.IsEnabled = false;
            }
        }

        //  METHOD:		CreateRoute_Click
        /// \brief Adds a new row to the datagrid
        /// \details <b>Details</b>
        ///     Creates a new RouteCity object and adds it to the carrier collection and updates the combo box
        ///
        /// \param - <b>sender:</b>  the object that called the method
        /// \param - <b>e:</b>       the arguments that are passed when this method is called
        /// 
        /// \return - <b>Nothing</b>
        ///
        private void CreateRoute_Click(object sender, RoutedEventArgs e)
        {
            RouteCity c = new RouteCity();
            c.newlyCreated = true;
            cityCollection.Add(c);
            UpdateRouteComboBox();
        }

        //  METHOD:		RouteUpdate_Click
        /// \brief Sends all data in the datagrid back to the database
        /// \details <b>Details</b>
        ///     Creates a series of inserts, update and delete query statements to send to the server to update the database
        ///
        /// \param - <b>sender:</b>  the object that called the method
        /// \param - <b>e:</b>       the arguments that are passed when this method is called
        /// 
        /// \return - <b>Nothing</b>
        ///
        private void RouteUpdate_Click(object sender, RoutedEventArgs e)
        {
            List<string> fields = new List<string>();
            fields.Add("routeid");
            Controller.TMS.MakeSelectCommand(fields, "route", null, null);
            List<string> results = Controller.TMS.ExecuteCommand();
            foreach (string result in results)
            {
                bool toDelete = true;
                int temp;
                int.TryParse(result, out temp);
                foreach (RouteCity c in cityCollection)
                {
                    if (temp == c.routeID)
                    {
                        Dictionary<string, string> updateValues = new Dictionary<string, string>();
                        updateValues.Add("routeID", c.routeID.ToString());
                        updateValues.Add("destCity", c.cityName);
                        updateValues.Add("kmToEast", c.kmToEast.ToString());
                        updateValues.Add("kmToWest", c.kmToWest.ToString());
                        updateValues.Add("hToEast", c.hToEast.ToString());
                        updateValues.Add("hToWest", c.hToWest.ToString());
                        updateValues.Add("east", c.east);
                        updateValues.Add("west", c.west);
                        Dictionary<string, string> conditions = new Dictionary<string, string>();
                        conditions.Add("carrierID", temp.ToString());
                        Controller.TMS.MakeUpdateCommand("carrier", updateValues, conditions);
                        Controller.TMS.ExecuteCommand();
                        toDelete = false;
                        break;
                    }
                }
                if (toDelete == true)
                {
                    Dictionary<string, string> conditions = new Dictionary<string, string>();
                    conditions.Add("routeid", temp.ToString());
                    Controller.TMS.MakeDeleteCommand("route", conditions);
                    Controller.TMS.ExecuteCommand();
                }
            }
            foreach (RouteCity newC in cityCollection)
            {
                if (newC.newlyCreated == true)
                {
                    List<string> values = new List<string>();
                    values.Add(newC.routeID.ToString());
                    values.Add(newC.cityName);
                    values.Add(newC.kmToEast.ToString());
                    values.Add(newC.kmToWest.ToString());
                    values.Add(newC.hToEast.ToString());
                    values.Add(newC.hToWest.ToString());
                    values.Add(newC.east);
                    values.Add(newC.west);
                    Controller.TMS.MakeInsertCommand("route", values);
                    Controller.TMS.ExecuteCommand();
                }
            }
        }

        //  METHOD:		RouteData_AddingNewItem
        /// \brief If an item is added to the grid, update the combobox
        /// \details <b>Details</b>
        ///     Calls the UpdateRouteComboBox() method if a new item is added
        ///
        /// \param - <b>sender:</b>  the object that called the method
        /// \param - <b>e:</b>       the arguments that are passed when this method is called
        /// 
        /// \return - <b>Nothing</b>
        ///
        private void RouteData_AddingNewItem(object sender, AddingNewItemEventArgs e)
        {
            UpdateRouteComboBox();
        }

        //  METHOD:		RateApply_Click
        /// \brief Updates the rate db table with new values
        /// \details <b>Details</b>
        ///     Deletes the old row in the rates table and inserts a new one
        ///
        /// \param - <b>sender:</b>  the object that called the method
        /// \param - <b>e:</b>       the arguments that are passed when this method is called
        /// 
        /// \return - <b>Nothing</b>
        ///
        private void RateAppy_Click(object sender, RoutedEventArgs e)
        {
            Controller.TMS.MakeDeleteCommand("rates", null);
            Controller.TMS.ExecuteCommand();
            List<string> toInsert = new List<string>();
            toInsert.Add(FTLRate.Text);
            toInsert.Add(LTLRate.Text);
            Controller.TMS.MakeInsertCommand("rates", toInsert);
            Controller.TMS.ExecuteCommand();
        }

        //  METHOD:		ChangeUpdatePath_Click
        /// \brief Changes the path that the database file is stored at
        /// \details <b>Details</b>
        ///     Opens a file dialog box that allows the user to choose a new path to save and open a backup file
        ///
        /// \param - <b>sender:</b>  the object that called the method
        /// \param - <b>e:</b>       the arguments that are passed when this method is called
        /// 
        /// \return - <b>Nothing</b>
        ///
        private void ChangeUpdatePath_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();
            of.InitialDirectory = DBBackUp;
            of.Filter = "sql files (*.sql)|*.sql|All files (*.*)|*.*";
            of.FilterIndex = 2;
            if (of.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string temp = DBBackUp;
                DBBackUp = of.FileName; 
                BackUpPath.Text = DBBackUp;
                string configContents;
                using (StreamReader sr = new StreamReader(Controller.ConfigPath))
                {
                    configContents = sr.ReadToEnd();
                }
                string toWrite = configContents.Replace(temp, DBBackUp);
                using (StreamWriter sw = new StreamWriter(Controller.ConfigPath, false))
                {
                    sw.Write(toWrite);
                }
            }
        }

        //  METHOD:		BackUp_Click
        /// \brief Backs up the database
        /// \details <b>Details</b>
        ///     Calls a database method that backs up the whole database
        ///
        /// \param - <b>sender:</b>  the object that called the method
        /// \param - <b>e:</b>       the arguments that are passed when this method is called
        /// 
        /// \return - <b>Nothing</b>
        ///
        private void BackUp_Click(object sender, RoutedEventArgs e)
        {
            Controller.TMS.BackItUp(DBBackUp);
        }

        //  METHOD:		Restore_Click
        /// \brief Restores the database
        /// \details <b>Details</b>
        ///     Calls a database method that restores the database from a backup file
        ///
        /// \param - <b>sender:</b>  the object that called the method
        /// \param - <b>e:</b>       the arguments that are passed when this method is called
        /// 
        /// \return - <b>Nothing</b>
        ///
        private void Restore_Click(object sender, RoutedEventArgs e)
        {
            Controller.TMS.Restore(DBBackUp);
        }
    }
}
