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
        private const string configFilePath = "..\\..\\config\\TMS.txt";
        public List<string> TMS_Database { get; set; }
        public List<string> MarketPlace_Database { get; set; }
        Database loginDB { get; set; }
        Database MarketPlace { get; set; }
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
            List<string> select = new List<string>();
            select.Add("*");
            MarketPlace.MakeSelectCommand(select, "Contract", null);
            MarketPlace.ExecuteCommand();
        }
        
        /// \brief Loads the database connection details from an external config file
        /// \details <b>Details</b>
        /// Checks to see if the config files exists and creates it if it doesn't.  If it does, the method reads from the file 
        /// and parses it out into data that is usable to connect to one or more databases
        /// \param - <b>None</b>
        /// 
        /// \return - <b>Nothing</b>
        /// 
        private void LoadConfig()
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
    }
}
