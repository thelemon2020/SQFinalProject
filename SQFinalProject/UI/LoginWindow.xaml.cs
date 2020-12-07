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
    /// \author <i>Deric Kruse, & Chris Lemon</i>
    ///
    public partial class LoginWindow : Window {
        //! Properties

        public List<string> userInfo;                                   //!<String list to store the info on the user that is logging in

        public LoginWindow () {
            InitializeComponent();

            Controller.LoadConfig();                                    // Parse the config file and initialize the databases for the program
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

                Controller.TMS.MakeSelectCommand ( QueryLst, "login", tempDict,null);

                List<string> UsrReturn = Controller.TMS.ExecuteCommand();

                QueryLst = new List<string> ();                // Then check if the password matches
                QueryLst.Add ("password");

                Controller.TMS.MakeSelectCommand ( QueryLst, "login", tempDict, null);

                List<string> PassReturn = Controller.TMS.ExecuteCommand();

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

                    Controller.TMS.MakeSelectCommand ( QueryLst, "login", tempDict, null);

                    userInfo = Controller.TMS.ExecuteCommand();

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
