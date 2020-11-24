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
        public bool loggedIn = false;
        public LoginWindow () {
            InitializeComponent();
        }

        private void Login_Click ( object sender,RoutedEventArgs e ) {
            if ( UsrName.Text.Length == 0 ) { 
                // change text in label to error message
            } else if ( Password.Text.Length == 0 ) { 
                // change text in label to error message
            } else {
                // get user name and check it against db
                if ( true ) { // if user name !exists
                } else if ( true ) { // if password !matches user name
                } else { 
                    loggedIn = true;
                    DialogResult = true;
                }
            }
            // temparary
            loggedIn = true;
            DialogResult = true;
        }

        private void Login_Closing ( object sender,System.ComponentModel.CancelEventArgs e ) {
            
             DialogResult = loggedIn;
        }
    }
}
