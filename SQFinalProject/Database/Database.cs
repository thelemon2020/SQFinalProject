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
            //<set starting properties to be able to connect to server
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
        public List<string> ExecuteCommand()
        {
            List<string> SQLReturn = new List<string>();
            try
            {
                currentConnection = DatabaseInteraction.connectToDatabase(connectionString); //< create a connection to a database
                SQLCommand = new MySqlCommand(userCommand, currentConnection); //<creat a MySqlCommand object to interact with database
                SQLReturn = DatabaseInteraction.CommandDatabase(SQLCommand); //<query database and get the return
                DatabaseInteraction.CloseConnection(currentConnection); //<close the connection to the database
            }
            catch(Exception e) //<catch any exception that may be thrown during the query process
            {
                SQLReturn = null; //<set the query return to null to signify a problem with the query
            }          
            return SQLReturn; //<return the query results to the calling function
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
            StringBuilder InsertCommand = new StringBuilder();  //<instantiate a stringbuiler for use in building the query string
            InsertCommand.AppendFormat("INSERT INTO {0} VALUES (", table); //<set the initial part of the query
            int i = 0;
            int countLoops = values.Count() - 1;
            foreach (string value in values) //<iterate through each string in the list to format the insert query string
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
            userCommand = InsertCommand.ToString();//<set user command variable to newly created query string
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
            StringBuilder InsertCommand = new StringBuilder(); //<instantiate a stringbuiler for use in building the query string
            InsertCommand.AppendFormat("INSERT INTO {0} (", table);  //<set the initial part of the query
            int i = 0;
            int countLoops = fields.Count() - 1;
            foreach(string field in fields)//<iterate through each string in the list to format which columns to insert the data into
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
            foreach (string value in values) //<iterate through values to format the order of the data being inserted
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
            userCommand = InsertCommand.ToString(); //< set userCommand to the newly created query string
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
            StringBuilder selectCommand = new StringBuilder(); //<instantiate string builder for use in building a query string
            selectCommand.AppendFormat("SELECT"); //< set the first part of the query string
            int i = 0;
            int countLoops = fields.Count() - 1;
            foreach (string entry in fields) //<iterate through the fields that are to be selected and append them to string
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
            if (conditions!=null) //<check if there are optional select conditions
            {
                selectCommand.AppendFormat(" WHERE"); //< add conditional WHERE clause
                i = 0;
                countLoops = conditions.Count() - 1;
                foreach (KeyValuePair<string, string> entry in conditions)  //<iterate through conditions and append them to string
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
            userCommand = selectCommand.ToString(); //< set userCommand to the newly created query string
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
            StringBuilder updateCommand = new StringBuilder(); //<instantiate string builder for use in building a query string
            updateCommand.AppendFormat("UPDATE {0} SET", table); //<set up 
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
            foreach (KeyValuePair<string, string> entry in conditions)//<iterate through conditions and append them to string
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
            userCommand = updateCommand.ToString();//< set userCommand to the newly created query string
        }


        /// \brief Creates an BackUp script of a <b>MySqlDatabase</b>
        /// \details <b>Details</b>
        /// Connects to a database and uses a 3rd party library called MySqlBackUp.Net <ahref="https://www.codeproject.com/Articles/256466/MySqlBackup-NET"></ahref>
        /// to create a backup script of the database.  The connection is then closed
        /// \param - filePath - <b>string</b> - the path to save the backup script to
        /// 
        /// \return - <b>Nothing</b>
        /// 
        public int BackItUp(string filePath)
        {
            currentConnection = DatabaseInteraction.connectToDatabase(connectionString); //<create connection
            int didWork = DatabaseInteraction.BackUpDB(currentConnection,filePath); //<try to back up database and find out if it worked or not
            DatabaseInteraction.CloseConnection(currentConnection); //<close the connection
            return didWork;//<return if backup was successful or not
        }

        /// \brief Restores a <b>MySqlDatabase</b> from a backup script
        /// \details <b>Details</b>
        /// Connects to a database and uses a 3rd party library called MySqlBackUp.Net <ahref="https://www.codeproject.com/Articles/256466/MySqlBackup-NET"></ahref>
        /// to restore the database from a backup script.  The connection is then closed
        /// \param - filePath - <b>string</b> - the path that the backup script is saved to.
        /// 
        /// \return - <b>Nothing</b>
        ///
        public int Restore(string filePath)
        {
            currentConnection = DatabaseInteraction.connectToDatabase(connectionString); //<connect to database
            int didWork = DatabaseInteraction.RestoreDB(currentConnection, filePath);//<restore database 
            DatabaseInteraction.CloseConnection(currentConnection);//<close connection
            return didWork;//<return if it worked or not
        }
    }
}
