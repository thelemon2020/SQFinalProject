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
    /// \class <b>MainWindow</b>
    /// 
    /// \brief This class holds all the event handlers for for the main WPF window.  It has data members for the TMS database & the Marketplace database.
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

        public AdminWindow ( string name )
        { 
            InitializeComponent();
            LoadConfig();                                               //< Parse the config file
            if (TMS_Database!=null)                                     //< Connect to the TMS database if the config file loaded successfully
            {
                loginDB = new Database(TMS_Database[0], TMS_Database[1], TMS_Database[2], TMS_Database[3]);
            }
            if (MarketPlace_Database!=null)                             //< Connect to the Marketplace database if the config file loaded successfully
            {
                MarketPlace = new Database(MarketPlace_Database[0], MarketPlace_Database[1], MarketPlace_Database[2], MarketPlace_Database[3]);
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
            if (File.Exists(configFilePath))                        //< If the config file exists, try to read from it
            {
                StreamReader configFile = new StreamReader(configFilePath);
                string contents = configFile.ReadToEnd();
                configFile.Close();
                if (contents != "")
                {
                    string[] splitByDB = contents.Split('\n');      //< Grab each line so it can be dealt with individually
                    foreach (string dbDetails in splitByDB)
                    {
                        string[] details = dbDetails.Split(' ');    //< Pull the individual fields from the line of the config file
                        if (details[0] == "TMS")                    //< If the line pertains to the TMS database, assign the values to the TMS string list
                        {
                            TMS_Database = new List<string>();
                            for (int i = 1; i < details.Count(); i++)
                            {
                                TMS_Database.Add(details[i]);
                            }
                        }
                        else if (details[0] == "MP")                //< If the line pertains to the Marketplace database, assign the values to the Marketplace string list
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
    }
}
