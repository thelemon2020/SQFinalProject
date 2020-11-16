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
    public class Database
    {
        public string connectionString { get; set; }

        public string ip { get; set; }
        public string user { get; set; }
        public string pass { get; set; }
        public string schema { get; set; }
        public MySqlConnection currentConnection { get; set; }
        public MySqlCommand SQLCommand { get; set; }
        public string userCommand { get; set; }
        public List<string> SQLReturn { get; set; }

        public Database(string dbIP, string userName, string password, string table)
        {
            ip = dbIP;
            user = userName;
            pass = password;
            schema = table;
            connectionString = "server=" + ip + ";uid=" + user + ";pwd=" + pass + ";database=" + schema;
        }

        public List<string> ExecuteCommand()
        {
            currentConnection = DatabaseInteraction.connectToDatabase(connectionString);
            SQLCommand = new MySqlCommand(userCommand, currentConnection);
            SQLReturn = DatabaseInteraction.CommandDatabase(SQLCommand);
            DatabaseInteraction.CloseConnection(currentConnection);
            return SQLReturn;
        }

        virtual public void MakeCommand(string table, string columns)
        {
            userCommand = "SELECT " + columns + " FROM " + table + ";";
        }

        public void MakeCommand(string columns, string table, string conditionalField, string searchParam)
        {
            userCommand = "SELECT " + columns + " FROM " + table + " WHERE " + conditionalField + " = " + searchParam + ";";
        }
        public void MakeCommand()
        {
            userCommand = "SELECT * from tms_login.login";
        }
    }
}
