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
        public const string configFilePath = @"..\..\config\TMS.txt";   //<The path to the config file
        public List<string> TMS_Database { get; set; }                  //<The the string list to store TMS DB connection info
        public List<string> MarketPlace_Database { get; set; }          //<The the string list to store Marketplace DB connection info
        Database loginDB { get; set; }                                  //<The database object for the TMS database
        Database MarketPlace { get; set; }                              //<The database object for the Marketplace database

        public string userName;                                         //<Stores the user name of the current user

        public AdminWindow(string name)
        {
            InitializeComponent();
            LoadConfig();                                               // Parse the config file
            CreateDatabases();
            userName = name;
            lblUsrInfo.Content = "User Name:  " + userName;
        }

        private void CreateDatabases()
        {
            if (TMS_Database != null)                                     // Connect to the TMS database if the config file loaded successfully
            {
                loginDB = new Database(TMS_Database[0], TMS_Database[1], TMS_Database[2], TMS_Database[3], TMS_Database[4]);
            }
            if (MarketPlace_Database != null)                             // Connect to the Marketplace database if the config file loaded successfully
            {
                MarketPlace = new Database(MarketPlace_Database[0], MarketPlace_Database[1], MarketPlace_Database[2], MarketPlace_Database[3], MarketPlace_Database[4]);
}
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

        private void TMSChange_Click(object sender, RoutedEventArgs e)
        {
            IPAddress temp;
            int temp1;
            if (IPAddress.TryParse(TMS_IP.Text, out temp) && (int.TryParse(TMS_Port.Text, out temp1)))
            {
                string newWrite = "TMS " + TMS_IP.Text + " " + TMS_Port.Text + " " + TMS_User.Text + " " + TMS_Password.Password.ToString() + " " + loginDB.schema + "\n" 
                    + "MP " + MarketPlace_Database[0] + " " + MarketPlace_Database[1] + " " + MarketPlace_Database[2] + " " + MarketPlace_Database[3] + " " + MarketPlace_Database[4] + "\n" + "LOGGER " + Logger.path; 
                StreamWriter sw = new StreamWriter(configFilePath,false);
                sw.Write(newWrite);
                sw.Close();
                TMS_Database = null;
                MarketPlace_Database = null;
                loginDB = null;
                MarketPlace = null;
                LoadConfig();
                CreateDatabases();
            }
            else
            {
                System.Windows.MessageBox.Show("Connection Information Not Formatted properly", "Bad Input", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
        }

        private void MPChange_Click(object sender, RoutedEventArgs e)
        {
            IPAddress temp;
            int temp1;
            if (IPAddress.TryParse(TMS_IP.Text, out temp) && (int.TryParse(TMS_Port.Text, out temp1)))
            {
                string newWrite = "MP " + MP_IP.Text + " " + MP_Port.Text + " " + MP_User.Text + " " + MP_Password.Password.ToString() + " " + loginDB.schema + "\n"
                    + "MP " + TMS_Database[0] + " " + TMS_Database[1] + " " + TMS_Database[2] + " " + TMS_Database[3] + " " + TMS_Database[4] + "\n" + "LOGGER " + Logger.path;
                StreamWriter sw = new StreamWriter(configFilePath, false);
                sw.Write(newWrite);
                sw.Close();
                TMS_Database = null;
                MarketPlace_Database = null;
                loginDB = null;
                MarketPlace = null;
                LoadConfig();
                CreateDatabases();
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Connection Information Not Formatted properly", "Bad Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

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
            }
        }

        private void LoadCarriers()
        {
            ObservableCollection<Carrier> carrierData = GetCarrierData();
            CarrierData.DataContext = carrierData;
        }

        private ObservableCollection<Carrier> GetCarrierData()
        {
            ObservableCollection<Carrier> carrierCollection = new ObservableCollection<Carrier>();
            List<string> fields = new List<string>();
            fields.Add("*");
            loginDB.MakeSelectCommand(fields, "carrier", null);
            fields = loginDB.ExecuteCommand();
            foreach (string field in fields)
            {
                string[] columns = field.Split(',');
                int carrierID;
                int.TryParse(columns[0], out carrierID);
                double FTL;
                double LTL;
                double reef;
                double.TryParse(columns[2], out FTL);
                double.TryParse(columns[3], out LTL);
                double.TryParse(columns[4], out reef);
                Carrier c = new Carrier(carrierID,columns[1],FTL, LTL, reef);
                carrierCollection.Add(c);
            }
            return carrierCollection;
        }
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
        private void FillFields()
        {
            TMS_IP.Text = loginDB.ip;
            TMS_Port.Text = loginDB.port;
            TMS_User.Text = loginDB.user;
            TMS_Password.Password = loginDB.pass;
            MP_IP.Text = MarketPlace.ip;
            MP_Port.Text = MarketPlace.port;
            MP_User.Text = MarketPlace.user;
            MP_Password.Password = MarketPlace.pass;
        }

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
                using (StreamReader sr = new StreamReader(configFilePath))
                {
                    configContents = sr.ReadToEnd();
                }
                string toWrite = configContents.Replace(temp, Logger.path);
                using (StreamWriter sw = new StreamWriter(configFilePath, false))
                {
                    sw.Write(toWrite);
                }
                LoadLogger();
            }
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            AddCarrier ac = new AddCarrier();
            ac.ShowDialog();
            if (ac.canCreate == true)
            {
                //Carrier c = new Carrier();
            }
        }
    }
}
