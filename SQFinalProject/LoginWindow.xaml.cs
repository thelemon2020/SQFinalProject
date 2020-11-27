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

namespace SQFinalProject {
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window {
        public const string configFilePath = @"..\..\config\TMS.txt";
        public List<string> TMS_Database { get; set; }
        public List<string> MarketPlace_Database { get; set; }
        Database loginDB { get; set; }
        Database MarketPlace { get; set; }

        private bool loggedIn = false;

        public List<string> userInfo;

        public LoginWindow () {
            InitializeComponent();

            LoadConfig();
            if (TMS_Database!=null)
            {
                loginDB = new Database(TMS_Database[0], TMS_Database[1], TMS_Database[2], TMS_Database[3]);
            }
            if (MarketPlace_Database!=null)
            {
                MarketPlace = new Database(MarketPlace_Database[0], MarketPlace_Database[1], MarketPlace_Database[2], MarketPlace_Database[3]);
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
            if (File.Exists(configFilePath))
            {
                StreamReader configFile = new StreamReader(configFilePath);
                string contents = configFile.ReadToEnd();
                configFile.Close();
                if (contents != "")
                {
                    string[] splitByDB = contents.Split('\n');
                    foreach (string dbDetails in splitByDB)
                    {
                        string[] details = dbDetails.Split(' ');
                        if (details[0] == "TMS")
                        {
                            TMS_Database = new List<string>();
                            for (int i = 1; i < details.Count(); i++)
                            {
                                TMS_Database.Add(details[i]);
                            }
                        }
                        else if (details[0] == "MP")
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
            if ( UsrName.Text.Length == 0 ) { 
                NameErr.Content = "User name cannot be blank!";
                isValid = false;
            } else {
                NameErr.Content = "";
            }
            
            if ( Password.Password.Length == 0 ) { 
                PassErr.Content = "Password cannot be blank!";
                isValid = false;
            } else { 
                PassErr.Content = "";
            }
            
            if ( isValid ) {
                string usrName = UsrName.Text.Trim ();
                string usrPass = Password.Password;
                
                // Check if the password matches
                List<string> QueryLst =  new List<string> ();
                QueryLst.Add ("password");

                Dictionary<string, string> tempDict = new Dictionary<string, string>();
                tempDict.Add ("username", usrName);

                loginDB.MakeSelectCommand ( QueryLst, "login", tempDict );
                            
                List<string> PassReturn = loginDB.ExecuteCommand();

                // Check if the name exists
                QueryLst =  new List<string> ();
                QueryLst.Add ("username");

                loginDB.MakeSelectCommand ( QueryLst, "login", tempDict );
            
                List<string> UsrReturn = loginDB.ExecuteCommand();

                if ( UsrReturn == null || PassReturn == null ) {
                    // Connection failed!!!
                    //?? isValid = false;
                    this.Close();

                } else if ( UsrReturn.Count() == 0 ) {
                    NameErr.Content = "User name doesn't exist!";
                    PassErr.Content = "";
                    isValid = false;

                } else if ( PassReturn.Count() == 0 || !usrPass.Equals ( PassReturn.ElementAt(0) ) ) {
                    
                    NameErr.Content = "";
                    PassErr.Content = "Password incorrect for user!";
                    isValid = false;

                } else {
                    PassErr.Content = "";
                    NameErr.Content = "";
                }
                
                loggedIn = isValid;

                if ( isValid ) {
                    QueryLst.Add ("password");
                    QueryLst.Add ("role");

                    loginDB.MakeSelectCommand ( QueryLst, "login", tempDict );
                            
                    userInfo = loginDB.ExecuteCommand();

                    /* !!~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~!! */
                    /* ~~~~~ ADD OTHER CLASSES WINDOWS HERE IN AN IF ~~~~~ */
                    /* !!~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~!! */
                    /*
                    if (userRole = p) {
                        load planner page ...
                    } else ... {} //*/

                    MainWindow mainWindow = new MainWindow (usrName);
                    mainWindow.Show();
                    
                    this.Close();
                }
            }        
        }
    }
}
