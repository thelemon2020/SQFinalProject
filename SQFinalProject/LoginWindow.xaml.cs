using System;
using System.Collections.Generic;
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
        private Database LoginDB;
        private bool loggedIn = false;

        public List<string> userInfo;

        public LoginWindow ( Database loginDB ) {
            InitializeComponent();
            LoginDB = loginDB;
        }

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

                LoginDB.MakeSelectCommand ( QueryLst, "login", tempDict );
                            
                List<List<string>> PassReturn = LoginDB.ExecuteCommand();

                // Check if the name exists
                QueryLst =  new List<string> ();
                QueryLst.Add ("username");

                LoginDB.MakeSelectCommand ( QueryLst, "login", tempDict );
            
                List<List<string>> UsrReturn = LoginDB.ExecuteCommand();

                if ( UsrReturn == null || PassReturn == null ) {
                    // Connection failed!!!
                    //?? isValid = false;
                } else if ( UsrReturn.Count() == 0 ) {
                    NameErr.Content = "User name doesn't exist!";
                    PassErr.Content = "";
                    isValid = false;

                } else if ( PassReturn.Count() == 0 || !usrPass.Equals ( PassReturn.ElementAt(0).ElementAt(0) ) ) {
                    
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

                    LoginDB.MakeSelectCommand ( QueryLst, "login", tempDict );
                            
                    userInfo = LoginDB.ExecuteCommand().ElementAt(0);
                    
                    DialogResult = true;
                }
            }        
        }

        private void Login_Closing ( object sender,System.ComponentModel.CancelEventArgs e ) {
            
             DialogResult = loggedIn;
        }



        /* ~~~~~~~~~~~~~~~~~~~~~~~~~~ */
        /* ~~~~~ Helper Methods ~~~~~ */
        /* ~~~~~~~~~~~~~~~~~~~~~~~~~~ *
        public bool UsrExists ( string usrName ) {
            bool retVal = false;

            List<string> nameLst =  new List<string> ();
            nameLst.Add ("username");
            Dictionary<string, string> tempDict = new Dictionary<string, string>();
            tempDict.Add ("username", usrName);

            LoginDB.MakeSelectCommand ( nameLst, "login", tempDict );
            
            List<List<string>> UsrReturn = LoginDB.ExecuteCommand();

            if ( UsrReturn == null ) {
                // Connection failed!!!
            } else if ( UsrReturn != null && UsrReturn.Count() >0 ) {
                retVal = true;
            }

            return retVal;
        } //*/


        /*
        public bool PassMatch ( string usrName, string pass ) {
            bool retVal = false;

            // 1. Set up stuff
            List<string> passLst =  new List<string> ();
            passLst.Add ("password");
            Dictionary<string, string> tempDict = new Dictionary<string, string>();
            tempDict.Add ("username", usrName);

            // 2. Make select command            
            LoginDB.MakeSelectCommand ( passLst, "login", tempDict );
            
            List<List<string>> PassReturn = LoginDB.ExecuteCommand();

            // 3. ??? username, password, role
            if ( PassReturn == null ) {
                // Connection failed!!!
            } else if ( PassReturn.Count() > 0 ) {
                string usrPass = PassReturn.ElementAt(0).ElementAt(0) ;

                retVal = usrPass.Equals(pass);
            }

            // 4. Profit!

            return retVal;
        } //*/
    }
}
