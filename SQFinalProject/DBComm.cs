using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace SQFinalProject
{
    public class DBComm
    {
        public string connectionString { get; set; }
        public MySqlConnection currentConnection { get; set; }
        public MySqlCommand SQLCommand { get; set; }
        public string userCommand { get; set; }
        public List<string> SQLReturn { get; set; }

        public DBComm(string dbIP, string userName, string password, string command, string table)
        {
            connectionString = "server=" + dbIP + ";uid=" + userName + ";pwd=" + password + ";database=" + table;
            userCommand = command;
        }

        public List<string> ExecuteCommand()
        {
            currentConnection = DatabaseInteraction.connectToDatabase(connectionString);
            SQLCommand = new MySqlCommand(userCommand, currentConnection);
            SQLReturn = DatabaseInteraction.CommandDatabase(SQLCommand);
            DatabaseInteraction.CloseConnection(currentConnection);
            return SQLReturn;
        }
    }
}
