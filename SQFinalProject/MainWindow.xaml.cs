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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SQFinalProject
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Database loginDB { get; set; }
        public MainWindow()
        { 
            InitializeComponent();
            loginDB = new Database("192.168.0.197", "tms_admin", "admin", "tms_login");
            List<string> insertInfo = new List<string>();
            insertInfo.Add("planner");
            insertInfo.Add("planner");
            insertInfo.Add("p");
            List<string> fields = new List<string>();
            fields.Add("username");
            fields.Add("password");
            fields.Add("role");
            loginDB.MakeInsertCommand("login", fields, insertInfo);
            loginDB.ExecuteCommand();
        }
    }
}
