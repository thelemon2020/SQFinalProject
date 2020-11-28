//*********************************************
// File			 : AboutW.xaml.cs
// Project		 : PROG2020 - Term Project
// Programmer	 : Nick Byam, Chris Lemon, Deric Kruse, Mark Fraser
// Last Change   : 2020-11-25
// Description	 : File containing all of the code behind the XAML for the about window.
//				Contains the event handler for the button in the XAML.
//*********************************************
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

namespace SQFinalProject.UI {
    ///
    /// \class <b>AboutW</b>
    /// 
    /// \brief This class holds the event handlers for the about window.  
    /// 
    /// \author <i>Deric Kruse</i>
    /// 
    public partial class AboutWindow : Window {
        public AboutWindow () {
            InitializeComponent();
        }



        //  METHOD:		OKButton_Click
        /// \brief Method that is called when the OK button is clicked
        /// \details <b>Details</b>
        ///     Closes The about window when the OK button is clicked.
        /// 
        /// \param - <b>sender:</b>  the object that called the method
        /// \param - <b>e:</b>       the arguments that are passed when this method is called
        /// 
        /// \return - <b>Nothing</b>
        /// 
        private void OKButton_Click ( object sender,RoutedEventArgs e ) {
            DialogResult = true;
        }
    }
}
