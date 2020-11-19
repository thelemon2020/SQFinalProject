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

        public Database(string dbIP, string userName, string password, string table)
        {
            ip = dbIP;
            user = userName;
            pass = password;
            schema = table;
            connectionString = "server=" + ip + ";uid=" + user + ";pwd=" + pass + ";database=" + schema;
        }

        public List<List<string>> ExecuteCommand()
        {
            List<List<string>> SQLReturn = new List<List<string>>();
            currentConnection = DatabaseInteraction.connectToDatabase(connectionString);
            SQLCommand = new MySqlCommand(userCommand, currentConnection);
            SQLReturn = DatabaseInteraction.CommandDatabase(SQLCommand);
            DatabaseInteraction.CloseConnection(currentConnection);
            return SQLReturn;
        }
        public void MakeInsertCommand(string table, List<string> values)
        {
            StringBuilder InsertCommand = new StringBuilder();
            InsertCommand.AppendFormat("INSERT INTO {0} VALUES (", table);
            int i = 0;
            int countLoops = values.Count() - 1;
            foreach (string value in values)
            {
                if (i==countLoops)
                {
                    InsertCommand.AppendFormat("{0}", value);
                }
                else
                {
                    InsertCommand.AppendFormat("{0}, ", value);
                }
                i++;
            }
            InsertCommand.AppendFormat(")");
            userCommand = InsertCommand.ToString();
        }
        public void MakeInsertCommand(string table, List<string> fields, List<string> values)
        {
            StringBuilder InsertCommand = new StringBuilder();
            InsertCommand.AppendFormat("INSERT INTO {0} (", table);
            int i = 0;
            int countLoops = fields.Count() - 1;
            foreach(string field in fields)
            {
                if (i==countLoops)
                {
                    InsertCommand.AppendFormat("{0})", field);
                }
                else
                {
                    InsertCommand.AppendFormat("{0}, ", field);
                }
                i++;
            }
            InsertCommand.AppendFormat(" VALUES (");
            i = 0;
            countLoops = values.Count() - 1;
            foreach (string value in values)
            {
                if (i == countLoops)
                {
                    InsertCommand.AppendFormat("'{0}'", value);
                }
                else
                {
                    InsertCommand.AppendFormat("'{0}', ", value);
                }
                i++;
            }
            InsertCommand.AppendFormat(");");
            userCommand = InsertCommand.ToString();
        }
        public void MakeSelectCommand(List<string> columns, string table, Dictionary<string, string> conditions)
        {
            StringBuilder selectCommand = new StringBuilder();
            selectCommand.AppendFormat("SELECT");
            int i = 0;
            int countLoops = columns.Count() - 1;
            foreach (string entry in columns)
            {
                if (i == countLoops)
                {
                    selectCommand.AppendFormat(" {0}", entry);
                }
                else
                {
                    selectCommand.AppendFormat(" {0},", entry);
                }
                i++;
            }
            selectCommand.AppendFormat(" FROM {0}", table);
            if (conditions.Count != 0)
            {
                selectCommand.AppendFormat(" WHERE");
                i = 0;
                countLoops = conditions.Count() - 1;
                foreach (KeyValuePair<string, string> entry in conditions)
                {
                    if (i == countLoops)
                    {
                        selectCommand.AppendFormat(" {0} = '{1}'", entry.Key, entry.Value);
                    }
                    else
                    {
                        selectCommand.AppendFormat(" {0} = '{1}' AND", entry.Key, entry.Value);
                    }
                    i++;
                }
                selectCommand.AppendFormat(";");
                userCommand = selectCommand.ToString();
            }
        }

        public void MakeUpdateCommand(string table, Dictionary<string, string> updateValues, Dictionary<string, string> conditions)
        {
            StringBuilder updateCommand = new StringBuilder();
            updateCommand.AppendFormat("UPDATE {0} SET", table);
            int i = 0;
            int countLoops = updateValues.Count() - 1;
            foreach (KeyValuePair<string, string> entry in updateValues)
            {
                if (i == countLoops)
                {
                    updateCommand.AppendFormat(" {0} = '{1}'", entry.Key, entry.Value);
                }
                else
                {
                    updateCommand.AppendFormat(" {0} = '{1}',", entry.Key, entry.Value);
                }
                i++;
            }
            updateCommand.AppendFormat(" WHERE");
            i = 0;
            countLoops = conditions.Count() - 1;
            foreach (KeyValuePair<string, string> entry in conditions)
            {
                if (i == countLoops)
                {
                    updateCommand.AppendFormat(" {0} = '{1}'", entry.Key, entry.Value);
                }
                else
                {
                    updateCommand.AppendFormat(" {0} = '{1}' AND", entry.Key, entry.Value);
                }
                i++;
            }
            updateCommand.AppendFormat(";");
            userCommand = updateCommand.ToString();
        }
    }
}
