/* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ *
 * FILE:			AboutW.cs          																								*
 * PROJECT:			Windows & Mobile Programming - Assignment 2																		*
 * PROGRAMMER:		Deric Kruse																										*
 * FINAL VERSION:	2020/09/27																										*
 * DESCRIPTION:		File containing all of the code behind the XAML for the about window. 											*
 *					Contains the event handler for the button in the XAML.      			                        				*
 * ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ */
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
    public partial class AboutW : Window {
        public AboutW () {
            InitializeComponent();
        }

        private void OKButton_Click ( object sender,RoutedEventArgs e ) {
            DialogResult = true;
        }
    }
}
