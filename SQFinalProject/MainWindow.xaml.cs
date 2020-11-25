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

        public List<string> userInfo;

        private bool isLoggedIn = false;

        public MainWindow()
        { 
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

        private void Window_Loaded ( object sender,EventArgs e ) {
            LoginWindow initialLogin = new LoginWindow ( loginDB );
            initialLogin.Owner = this;
            Nullable<bool> loginResult = initialLogin.ShowDialog();

            if ( loginResult.HasValue ) {
                isLoggedIn = loginResult.Value;
                EnableCtrls ( isLoggedIn );
                
                if ( isLoggedIn ) {
                    userInfo = initialLogin.userInfo;
                }
            }
        }
        

        
/* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ *
 * METHOD:		CloseCB_CanExecute & CloseCB_Executed																				*
 * DESCRIPTION:	The CanExecute method says when the Close command binding can function and the Executed method says what happens    *
 *          when it is run.  If Close is clicked, close the window.                                                                 *
 * PARAMETERS:	sender :    the object that called the method              															*
 *              e :         the arguments that are passed when this method is called                                                *
 * RETURNS:		void																												*
 * ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ */
        private void CloseCB_CanExecute ( object sender,CanExecuteRoutedEventArgs e ) {
            e.CanExecute = true;
        }

        private void CloseCB_Executed ( object sender,ExecutedRoutedEventArgs e ) {
            this.Close();
        }

        private void Login_Click ( object sender,RoutedEventArgs e ) {
            LoginWindow LoginW = new LoginWindow ( loginDB );
            LoginW.Owner = this;
            Nullable<bool> loginResult = LoginW.ShowDialog();


            if ( loginResult.HasValue ) {
                isLoggedIn = loginResult.Value;
                EnableCtrls ( isLoggedIn );
                
                if ( isLoggedIn ) {
                    userInfo = LoginW.userInfo;
                }
            }
        }
        

        
/* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ *
 * METHOD:		About_Click      																		                    		*
 * DESCRIPTION:	Opens the about window when the about option is clicked.                                                            *
 * PARAMETERS:	sender :    the object that called the method              															*
 *              e :         the arguments that are passed when this method is called                                                *
 * RETURNS:		void																												*
 * ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ */
        private void About_Click ( object sender,RoutedEventArgs e ) {
            AboutW aboutBox = new AboutW();
            aboutBox.Owner = this;
            aboutBox.ShowDialog();
        }



        private void EnableCtrls ( bool isLogin ) {
                TestBTN.IsEnabled = isLogin;
        }
    }
}
