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
{   /// 
    /// \class <b>Database</b>
    ///
    /// \brief The purpose of this class is to connect to and interact with a MYSQL Database
    ///
    /// \author <i>Chris Lemon</i>
    ///
    public class Database
    {
        //! Properties
        public string connectionString { get; set; }//<the string used to connect to the database via MySqlConnector
        public string ip { get; set; }//<The ip of the database to connect to
        public string user { get; set; }//<The username used to login to the database
        public string pass { get; set; }//<The password used to login to the database
        public string schema { get; set; }//<The database schema to interact with
        public MySqlConnection currentConnection { get; set; }//<The active connection with the database
        public MySqlCommand SQLCommand { get; set; }//<The command to be sent to the server
        public string userCommand { get; set; }//<The command to be converted to MySqlCommand object

        /// \brief To instantiate a new Database object with arguments supplied from a config file
        /// \details <b>Details</b>
        /// Instantiates a Databse object, using arguments that are held in an external config file.  It sets enough properties to create a MySQLConnection object
        /// \param - dbIP - <b>string</b> - The ip address of the database to be connected to
        /// \param - userName - <b>string</b> - the username used to log in to the server
        /// \param - password - <b>string</b> - the password used to log in to the server
        /// \param - table - <b>string</b> - the schema to be manipulated
        /// 
        /// \return - <b>Nothing</b>
        ///
        public Database(string dbIP, string userName, string password, string table)
        {
            //set starting properties to be able to connect to server
            ip = dbIP;
            user = userName;
            pass = password;
            schema = table;
            connectionString = "server=" + ip + ";uid=" + user + ";pwd=" + pass + ";database=" + schema;
        }
        
        /// \brief Used to send a command to a database and get a response back
        /// \details <b>Details</b>
        /// Calls other methods that allow for a connection to a database to be made and a query to be made to the database
        /// \param - <b>None</b>
        /// 
        /// \return - SQLReturn - <b>List<List<string>></b> - This list holds whatever the response from the database was
        /// 
        public List<List<string>> ExecuteCommand()
        {
            List<List<string>> SQLReturn = new List<List<string>>();
            try
            {
                currentConnection = DatabaseInteraction.connectToDatabase(connectionString);
                SQLCommand = new MySqlCommand(userCommand, currentConnection);
                SQLReturn = DatabaseInteraction.CommandDatabase(SQLCommand);
                DatabaseInteraction.CloseConnection(currentConnection);
            }
            catch(Exception e)
            {
                SQLReturn = null;
            }          
            return SQLReturn;
        }
        
        /// \brief Creates an Insert Command
        /// \details <b>Details</b>
        /// Uses a <i>StringBuilder</i> to combine parameters into a usable SQL Command.  It can take any number of variables.  
        /// This is the default method, when the number of values being inserted matched the number of columns in a table
        /// \param - table - <b>string</b> - the table to be inserted into
        /// \param - values - <b>List<string></b> - the new values to be inserted
        /// 
        /// \return - <b>Nothing</b>
        /// 
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
                    InsertCommand.AppendFormat("'{0}'", value);
                }
                else
                {
                    InsertCommand.AppendFormat("'{0}', ", value);
                }
                i++;
            }
            InsertCommand.AppendFormat(")");
            userCommand = InsertCommand.ToString();
        }

        /// \brief Creates an Insert Command
        /// \details <b>Details</b>
        /// Uses a <i>StringBuilder</i> to combine parameters into a usable SQL Command.  It can take any number of variables.  
        /// This is the overloaded method that is used when the entire row isn't being filled
        /// \param - table - <b>string</b> - the table to be inserted into
        /// \param - fields - <b>List<string></b> - the columns to insert into
        /// \param - values - <b>List<string></b> - the new values to be inserted
        /// 
        /// \return - <b>Nothing</b>
        /// 
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

        /// \brief Creates an Select Command
        /// \details <b>Details</b>
        /// Uses a <i>StringBuilder</i> to combine parameters into a usable SQL Command.  It can take any number of variables.  
        /// The command is used to get information out of the database
        /// \param - fields - <b>List<string></b> - the columns to be returned
        /// \param - table - <b>string</b> - the table to be inserted into
        /// \param - conditions - <b>Dictionary<string, string></b> - the conditions that need to be met for a row or parts of a row to be returned
        /// 
        /// \return - <b>Nothing</b>
        /// 
        public void MakeSelectCommand(List<string> fields, string table, Dictionary<string, string> conditions)
        {
            StringBuilder selectCommand = new StringBuilder();
            selectCommand.AppendFormat("SELECT");
            int i = 0;
            int countLoops = fields.Count() - 1;
            foreach (string entry in fields)
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
            if (conditions!=null)
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
            }
            selectCommand.AppendFormat(";");
            userCommand = selectCommand.ToString();
        }


        /// \brief Creates an Update Command
        /// \details <b>Details</b>
        /// Uses a <i>StringBuilder</i> to combine parameters into a usable SQL Command.  It can take any number of variables.  
        /// The command is used to update a field or fields in a table
        /// \param - table - <b>string</b> - the table to be inserted into
        /// \param - updateValues - <b>Dictionary<string, string></b> - the columns to be updated and the values to be used
        /// \param - conditions - <b>Dictionary<string, string></b> - the conditions that need to be met for a row or parts of a row to be updated
        /// 
        /// \return - <b>Nothing</b>
        /// 
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


        /// \brief Creates an BackUp script of a <b>MySqlDatabase</b>
        /// \details <b>Details</b>
        /// Connects to a database and uses a 3rd party library called MySqlBackUp.Net <ahref="https://www.codeproject.com/Articles/256466/MySqlBackup-NET"></ahref>
        /// to create a backup script of the database.  The connection is then closed
        /// \param - filePath - <b>string</b> - the path to save the backup script to
        /// 
        /// \return - <b>Nothing</b>
        /// 
        public void BackItUp(string filePath)
        {
            currentConnection = DatabaseInteraction.connectToDatabase(connectionString);
            DatabaseInteraction.BackUpDB(currentConnection,filePath);
            DatabaseInteraction.CloseConnection(currentConnection);
        }

        /// \brief Restores a <b>MySqlDatabase</b> from a backup script
        /// \details <b>Details</b>
        /// Connects to a database and uses a 3rd party library called MySqlBackUp.Net <ahref="https://www.codeproject.com/Articles/256466/MySqlBackup-NET"></ahref>
        /// to restore the database from a backup script.  The connection is then closed
        /// \param - filePath - <b>string</b> - the path that the backup script is saved to.
        /// 
        /// \return - <b>Nothing</b>
        ///
        public void Restore(string filePath)
        {
            currentConnection = DatabaseInteraction.connectToDatabase(connectionString);
            DatabaseInteraction.RestoreDB(currentConnection, filePath);
            DatabaseInteraction.CloseConnection(currentConnection);
        }
    }
}
