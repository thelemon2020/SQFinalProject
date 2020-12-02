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

namespace SQFinalProject.UI
{
    /// <summary>
    /// Interaction logic for AddCarrier.xaml
    /// </summary>
    public partial class AddCarrier : Window
    {
        public bool canCreate { get; set; }
        public string name { get; set; }
        public string FTLRate { get; set; }
        public string LTLRate { get; set; }
        public string reefer { get; set; }
        public AddCarrier()
        {
            InitializeComponent();
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            canCreate = true;
            name = cName.Text;
            FTLRate = FTL.Text;
            LTLRate = LTL.Text;
            reefer = Reef.Text;
            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            canCreate = false;
            this.Close();
        }

        private void CheckCreate(object sender, TextChangedEventArgs e)
        {
            if ((cName.Text != "") && (FTL.Text != "") && (LTL.Text != "") && (Reef.Text != ""))
            {
                Create.IsEnabled = true;
            }
            else
            {
                Create.IsEnabled = false;
            }
        }
    }
}
