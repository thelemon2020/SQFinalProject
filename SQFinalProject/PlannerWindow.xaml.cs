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
    /// \author <i>Deric Kruse</i>
    /// 
    public partial class PlannerWindow : Window
    {
        //! Properties
        public const string configFilePath = @"..\..\config\TMS.txt";   //!<The path to the config file
        public List<string> TMS_Database { get; set; }                  //!<The the string list to store TMS DB connection info
        public List<string> MarketPlace_Database { get; set; }          //!<The the string list to store Marketplace DB connection info
        Database loginDB { get; set; }                                  //!<The database object for the TMS database
        Database MarketPlace { get; set; }                              //!<The database object for the Marketplace database

        public string userName;                                         //!<Stores the user name of the current user

        public PlannerWindow ( string name ) {
            InitializeComponent();
            LoadConfig(); //!<Call method that loads database connection info from config file
            if (TMS_Database!=null)
            {
                loginDB = new Database(TMS_Database[0], TMS_Database[1], TMS_Database[2], TMS_Database[3]); //!< fill loginDB database object with connection info
            }
            if (MarketPlace_Database!=null)                             //!< Connect to the Marketplace database if the config file loaded successfully
            {
                MarketPlace = new Database(MarketPlace_Database[0], MarketPlace_Database[1], MarketPlace_Database[2], MarketPlace_Database[3]); //!< fill MarketPlace database object with connection info
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
            if (File.Exists(configFilePath))                        //!< If the config file exists, try to read from it
            {
                StreamReader configFile = new StreamReader(configFilePath);//!<open reader stream
                string contents = configFile.ReadToEnd(); //!< get string from config file
                configFile.Close();//!<close stream
                if (contents != "")
                {
                    string[] splitByDB = contents.Split('\n');//!<split string by line
                    foreach (string dbDetails in splitByDB)//!<iterate through string
                    {
                        string[] details = dbDetails.Split(' '); //!<split string into individual fields
                        if (details[0] == "TMS") //!<If the info is for the TMS database
                        {
                            TMS_Database = new List<string>();
                            for (int i = 1; i < details.Count(); i++) //!<iterate through parts of detials
                            {
                                TMS_Database.Add(details[i]);
                            }
                        }
                        else if (details[0] == "MP")//!<If the info is for the MarketPlace database
                        {
                            MarketPlace_Database = new List<string>();
                            for (int i = 1; i < details.Count(); i++)//!<iterate through parts of detials
                            {
                                MarketPlace_Database.Add(details[i]);
                            }
                        }
                    }
                }
            }
            else //!<if file does not exist
            {
                FileStream newConfig = File.Create(configFilePath); //!< create file
                newConfig.Close();//!<close file
            }                      
        }



        //  METHOD:	    Window_Loaded, not currently used
        // \brief Method that is called when the main window is loaded
        // \details <b>Details</b>
        //     The Window_Loaded event gets called when the window is finished loading.  
        // It brings up the Login window and handles what happens when the login window closes.
        // 
        // \param - <b>sender:</b>  the object that called the method
        // \param - <b>e:</b>       the arguments that are passed when this method is called
        // 
        // \return - <b>Nothing</b>
        //
       /* private void Window_Loaded ( object sender, EventArgs e ) {
            LoginWindow initialLogin = new LoginWindow ( loginDB );
            initialLogin.Owner = this;
            Nullable<bool> loginResult = initialLogin.ShowDialog();

            if ( loginResult.HasValue ) {
                isLoggedIn = loginResult.Value;
                
                if ( isLoggedIn ) {
                    userInfo = initialLogin.userInfo;
                }

                EnableCtrls ( isLoggedIn );
            }
        }*/



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


        
        public ObservableCollection<string> MarketRtn;

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
            List<string> QueryLst = new List<string> ();
            QueryLst.Add ("*");

            //Dictionary<string, string> tempDict = new Dictionary<string, string>();
            //tempDict.Add ("username", usrName);

            loginDB.MakeSelectCommand ( QueryLst, "login", null );
            
            List<string> tmp = loginDB.ExecuteCommand();
            //txtMain.Text = tmp.ElementAt(0) + tmp.ElementAt(1) + tmp.ElementAt(2);
                            
            MarketRtn = new ObservableCollection <string> ( loginDB.ExecuteCommand() );
        }
    }
}
