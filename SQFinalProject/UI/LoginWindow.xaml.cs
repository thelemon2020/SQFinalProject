using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace SQFinalProject.UI {
    ///
    /// \class LoginWindow
    ///
    /// \brief This class holds all the event handlers for for the WPF login window.  It has data members for the TMS database & the Marketplace database.
    /// The error handling in this class will be handled in the form of message boxes describing the errors that happen for majour errors, and error text indicating
    /// simpler errors. The testing for this class will be mainly done manually as this is the most efficient way to access the event handlers in the way that they
    /// will be used in the final program.
    ///
    /// \author <i>Deric Kruse</i>
    ///
    public partial class LoginWindow : Window {
        //! Properties
        public const string configFilePath = @"..\..\config\TMS.txt";   //<The path to the config file
        public List<string> TMS_Database { get; set; }                  //<The the string list to store TMS DB connection info
        public List<string> MarketPlace_Database { get; set; }          //<The the string list to store Marketplace DB connection info for the config parser
        Database loginDB { get; set; }                                  //<The database object for the TMS database

        public List<string> userInfo;                                   //!<String list to store the info on the user that is logging in

        public LoginWindow () {
            InitializeComponent();

            LoadConfig();                                               // Parse the config file
            if (TMS_Database!=null)                                     // Connect to the TMS database if the config file loaded successfully
            {
                loginDB = new Database(TMS_Database[0], TMS_Database[1], TMS_Database[2], TMS_Database[3], TMS_Database[4]);
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
                        else if (details[0] == "LOGGER")
                        {
                            Logger.path = details[1];
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



        //  METHOD:		Login_Click
        /// \brief Method that dictates what happens when the login button is clicked.
        /// \details <b>Details</b>
        ///     This method first checks if either the user name or password is empty, displaying appropriage messages.  If they aren't, it queries the database
        /// to see if the user name exists and if the password matches.  If both of these are true, it loads the appropriate main page for the users role and closes
        /// this window.
        ///
        /// \param - <b>sender:</b>  the object that called the method
        /// \param - <b>e:</b>       the arguments that are passed when this method is called
        ///
        /// \return - <b>Nothing</b>
        ///
        private void Login_Click ( object sender,RoutedEventArgs e ) {
            bool isValid = true;
            if ( UsrName.Text.Length == 0 ) {                   // Check if the username is blank and display an error if it is
                NameErr.Content = "User name cannot be blank!";
                isValid = false;
            } else {
                NameErr.Content = "";
            }

            if ( Password.Password.Length == 0 ) {              // Check if the username is blank and display an error if it is
                PassErr.Content = "Password cannot be blank!";
                isValid = false;
            } else {
                PassErr.Content = "";
            }

            if ( isValid ) {                                   // If the two text boxes are not empty ...
                string usrName = UsrName.Text.Trim ();
                string usrPass = Password.Password;

                List<string> QueryLst = new List<string> ();   // Set up the database query and check if the user name exists in the database
                QueryLst.Add ("username");

                Dictionary<string, string> tempDict = new Dictionary<string, string>();
                tempDict.Add ("username", usrName);

                loginDB.MakeSelectCommand ( QueryLst, "login", tempDict,null);

                List<string> UsrReturn = loginDB.ExecuteCommand();

                QueryLst = new List<string> ();                // Then check if the password matches
                QueryLst.Add ("password");

                loginDB.MakeSelectCommand ( QueryLst, "login", tempDict, null);

                List<string> PassReturn = loginDB.ExecuteCommand();

                if ( UsrReturn == null || PassReturn == null ) {
                    // Connection failed!!!
                    this.Close();

                } else if ( UsrReturn.Count() == 0 ) {          // If the User name is not found, print an error message ...
                    NameErr.Content = "User name doesn't exist!";
                    PassErr.Content = "";
                    isValid = false;

                } else if (( PassReturn.Count() == 0) || (!usrPass.Equals ( PassReturn.ElementAt(0) ))) {
                                                                // and if the password doesn't match, print an error message
                    NameErr.Content = "";
                    PassErr.Content = "Password incorrect for user!";
                    isValid = false;

                } else {                                        // Otherwise clear all the errors
                    PassErr.Content = "";
                    NameErr.Content = "";
                }

                if ( isValid ) {                                // If no errors were found ...
                    QueryLst = new List<string> ();             // Set up the database query and get the user's role from the database
                    QueryLst.Add ("role");

                    loginDB.MakeSelectCommand ( QueryLst, "login", tempDict, null);

                    userInfo = loginDB.ExecuteCommand();

                    if ( userInfo.ElementAt(0).ToUpper().Equals( "A" ) ) {          // If the user is an admin, load the admin main page
                        AdminWindow mainWindow = new AdminWindow (usrName);
                        mainWindow.Show();
                    } else if ( userInfo.ElementAt(0).ToUpper().Equals( "B" ) ) {   // If the user is a buyer, load the buyer main page
                        BuyerWindow mainWindow = new BuyerWindow (usrName);
                        mainWindow.Show();
                    } else if ( userInfo.ElementAt(0).ToUpper().Equals( "P" ) ) {   // If the user is a planner, load the buyer main page
                        PlannerWindow mainWindow = new PlannerWindow (usrName);
                        mainWindow.Show();
                    } else {
                        BuyerWindow mainWindow = new BuyerWindow (usrName);
                        mainWindow.Show();
                    }

                    this.Close();                               // and finally close the window
                }
            }
        }
    }
}
