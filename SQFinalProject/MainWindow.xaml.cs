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
using System.Windows.Navigation;
using System.Windows.Shapes;
using SQFinalProject.ContactMgmtBilling;

namespace SQFinalProject
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public const string configFilePath = @"..\..\config\TMS.txt";
        public List<string> TMS_Database { get; set; }
        public List<string> MarketPlace_Database { get; set; }
        Database loginDB { get; set; }
        Database MarketPlace { get; set; }

        public string userName;

        private bool isLoggedIn = false;

        public MainWindow ( string name )
        { 
            InitializeComponent();
            LoadConfig(); //<Call method that loads database connection info from config file
            if (TMS_Database!=null)
            {
                loginDB = new Database(TMS_Database[0], TMS_Database[1], TMS_Database[2], TMS_Database[3]); //< fill loginDB database object with connection info
            }
            if (MarketPlace_Database!=null)
            {
                MarketPlace = new Database(MarketPlace_Database[0], MarketPlace_Database[1], MarketPlace_Database[2], MarketPlace_Database[3]); //< fill MarketPlace database object with connection info
            }
            isLoggedIn = true; //<User is logged in
            userName = name; //copies username provided by user
            lblUsrInfo.Content = "User Name:  " + userName; //<make a string to show on the UI
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
                StreamReader configFile = new StreamReader(configFilePath);//<open reader stream
                string contents = configFile.ReadToEnd(); //< get string from config file
                configFile.Close();//<close stream
                if (contents != "")
                {
                    string[] splitByDB = contents.Split('\n');//<split string by line
                    foreach (string dbDetails in splitByDB)//<iterate through string
                    {
                        string[] details = dbDetails.Split(' '); //<split string into individual fields
                        if (details[0] == "TMS") //<If the info is for the TMS database
                        {
                            TMS_Database = new List<string>();
                            for (int i = 1; i < details.Count(); i++) //<iterate through parts of detials
                            {
                                TMS_Database.Add(details[i]);
                            }
                        }
                        else if (details[0] == "MP")//<If the info is for the MarketPlace database
                        {
                            MarketPlace_Database = new List<string>();
                            for (int i = 1; i < details.Count(); i++)//<iterate through parts of detials
                            {
                                MarketPlace_Database.Add(details[i]);
                            }
                        }
                    }
                }
            }
            else //<if file does not exist
            {
                FileStream newConfig = File.Create(configFilePath); //< create file
                newConfig.Close();//<close file
            }                      
        }



        //  METHOD:	    Window_Loaded
        /// \brief Method that is called when the main window is loaded
        /// \details <b>Details</b>
        ///     The Window_Loaded event gets called when the window is finished loading.  
        /// It brings up the Login window and handles what happens when the login window closes.
        /// 
        /// \param - <b>sender:</b>  the object that called the method
        /// \param - <b>e:</b>       the arguments that are passed when this method is called
        /// 
        /// \return - <b>Nothing</b>
        ///
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



        //  METHODS:	CloseCB_CanExecute & CloseCB_Executed
        /// \brief Says when the close command can run and what happens when it does
        /// \details <b>Details</b>
        ///     The CanExecute method says when the Close command binding can function and the Executed method says what happens
        /// when it is run.  If Close is clicked, close the window.
        /// 
        /// \param - <b>sender:</b>  the object that called the method
        /// \param - <b>e:</b>       the arguments that are passed when this method is called
        /// 
        /// \return - <b>Nothing</b>
        ///
        private void CloseCB_CanExecute ( object sender,CanExecuteRoutedEventArgs e ) {
            e.CanExecute = true;
        }

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
            AboutW aboutBox = new AboutW();
            aboutBox.Owner = this;
            aboutBox.ShowDialog();
        }

        private void GetContracts ( object sender,RoutedEventArgs e ) {
            List<string> QueryLst =  new List<string> ();
            QueryLst.Add ("*");

            Dictionary<string, string> tempDict = new Dictionary<string, string>();
            //tempDict.Add ("username", usrName);

            MarketPlace.MakeSelectCommand ( QueryLst, "cmp", tempDict );
                            
            List<string> PassReturn = MarketPlace.ExecuteCommand();
        }
    }
}
