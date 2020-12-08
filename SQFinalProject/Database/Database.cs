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
    /// \class Database
    ///
    /// \brief The purpose of this class is to act as an abstraction for the <b>DatabaseInteraction</b> class. 
    /// It takes arguments passed in from the various <b>Window</b> classes, parses them and then passes them onto the various <b>DatabaseInteraction</b> methods.
    /// All error handling is handled by the DatabaseInteraction methods, which just pass back null if there is an error.  As the returns from all <b>Database</b>
    /// methods are <b>strings</b> or <b>List<string></b>, a null is easily parsed as a failed query
    ///
    /// \author <i>Chris Lemon</i>
    ///
    public class Database
    {
        //! Properties
        public string connectionString { get; set; }//!<the string used to connect to the database via MySqlConnector
        public string ip { get; set; }//!<The ip of the database to connect to
        public string user { get; set; }//!<The username used to login to the database
        public string port { get; set; }
        public string pass { get; set; }//!<The password used to login to the database
        public string schema { get; set; }//!<The database schema to interact with
        public MySqlConnection currentConnection { get; set; }//!<The active connection with the database
        public MySqlCommand SQLCommand { get; set; }//!<The command to be sent to the server
        public string userCommand { get; set; }//!<The command to be converted to MySqlCommand object

        /// \brief To instantiate a new Database object with arguments supplied from a config file
        /// \details <b>Details</b>
        /// Instantiates a Database object, using arguments that are held in an external config file.  It sets enough properties to create a MySQLConnection object
        /// 
        /// \param - dbIP - <b>string</b> - The ip address of the database to be connected to
        /// \param - userName - <b>string</b> - the username used to log in to the server
        /// \param - password - <b>string</b> - the password used to log in to the server
        /// \param - table - <b>string</b> - the schema to be manipulated
        /// 
        /// \return - <b>Nothing</b>
        ///
        public Database(string dbIP, string userport, string userName, string password, string table)
        {
            //!<set starting properties to be able to connect to server
            ip = dbIP;
            user = userName;
            pass = password;
            schema = table;
            port = userport;
            connectionString = "server=" + ip + ";port=" + port + ";uid=" + user + ";pwd=" + pass + ";database=" + schema;
        }
        
        /// \brief Used to send a command to a database and get a response back
        /// \details <b>Details</b>
        /// Calls other methods that allow for a connection to a database to be made and a query to be made to the database
        /// 
        /// \param - <b>None</b>
        /// 
        /// \return - SQLReturn - <b>List<List<string>></b> - This list holds whatever the response from the database was
        /// 
        public List<string> ExecuteCommand()
        {
            List<string> SQLReturn = new List<string>();
            try
            {
                currentConnection = DatabaseInteraction.connectToDatabase(connectionString); // create a connection to a database
                SQLCommand = new MySqlCommand(userCommand, currentConnection); //creat a MySqlCommand object to interact with database
                SQLReturn = DatabaseInteraction.CommandDatabase(SQLCommand); //query database and get the return
                DatabaseInteraction.CloseConnection(currentConnection); //close the connection to the database
            }
            catch(Exception e) //catch any exception that may be thrown during the query process
            {
                Logger.Log("MySQL Command Failed - " + e.Message);
                SQLReturn = null; //set the query return to null to signify a problem with the query
            }          
            return SQLReturn; //return the query results to the calling function
        }
        
        /// \brief Creates an Insert Command
        /// \details <b>Details</b>
        /// Uses a <i>StringBuilder</i> to combine parameters into a usable SQL Command.  It can take any number of variables.  
        /// This is the default method, when the number of values being inserted matched the number of columns in a table
        /// 
        /// \param - table - <b>string</b> - the table to be inserted into
        /// \param - values - <b>List<string></b> - the new values to be inserted
        /// 
        /// \return - <b>Nothing</b>
        /// 
        public void MakeInsertCommand(string table, List<string> values)
        {
            StringBuilder InsertCommand = new StringBuilder();  //instantiate a stringbuiler for use in building the query string
            InsertCommand.AppendFormat("INSERT INTO {0} VALUES (", table); //set the initial part of the query
            int i = 0;
            int countLoops = values.Count() - 1;
            foreach (string value in values) //iterate through each string in the list to format the insert query string
            {
                string value1;
                if (value.Contains("'"))
                {
                    value1 = value.Replace("'","''");
                }
                else
                {
                    value1 = value;
                }
                if (i==countLoops)
                {
                    InsertCommand.AppendFormat("'{0}'", value1);
                }
                else
                {
                    InsertCommand.AppendFormat("'{0}', ", value1);
                }
                i++;
            }
            InsertCommand.AppendFormat(")");
            userCommand = InsertCommand.ToString();//set user command variable to newly created query string
        }

        /// \brief Creates an Insert Command
        /// \details <b>Details</b>
        /// Uses a <i>StringBuilder</i> to combine parameters into a usable SQL Command.  It can take any number of variables.  
        /// This is the overloaded method that is used when the entire row isn't being filled
        /// 
        /// \param - table - <b>string</b> - the table to be inserted into
        /// \param - fields - <b>List<string></b> - the columns to insert into
        /// \param - values - <b>List<string></b> - the new values to be inserted
        /// 
        /// \return - <b>Nothing</b>
        /// 
        public void MakeInsertCommand(string table, List<string> fields, List<string> values)
        {
            StringBuilder InsertCommand = new StringBuilder(); //instantiate a stringbuiler for use in building the query string
            InsertCommand.AppendFormat("INSERT INTO {0} (", table);  //set the initial part of the query
            int i = 0;
            int countLoops = fields.Count() - 1;
            foreach(string field in fields)//iterate through each string in the list to format which columns to insert the data into
            {
                string field1;
                if (field.Contains("'"))
                {
                    field1 = field.Replace("'", "''");
                }
                else
                {
                    field1 = field;
                }
                if (i==countLoops)
                {
                    InsertCommand.AppendFormat("{0})", field1);
                }
                else
                {
                    InsertCommand.AppendFormat("{0}, ", field1);
                }
                i++;
            }
            InsertCommand.AppendFormat(" VALUES (");
            i = 0;
            countLoops = values.Count() - 1;
            foreach (string value in values) //iterate through values to format the order of the data being inserted
            {
                string value1;
                if (value.Contains("'"))
                {
                    value1 = value.Replace("'", "''");
                }
                else
                {
                    value1 = value;
                }
                if (i == countLoops)
                {
                    InsertCommand.AppendFormat("'{0}'", value1);
                }
                else
                {
                    InsertCommand.AppendFormat("'{0}', ", value1);
                }
                i++;
            }
            InsertCommand.AppendFormat(");");
            userCommand = InsertCommand.ToString(); // set userCommand to the newly created query string
        }

        /// \brief Creates an Select Command
        /// \details <b>Details</b>
        /// Uses a <i>StringBuilder</i> to combine parameters into a usable SQL Command.  It can take any number of variables.  
        /// The command is used to get information out of the database
        /// 
        /// \param - fields - <b>List<string></b> - the columns to be returned
        /// \param - table - <b>string</b> - the table to be inserted into
        /// \param - conditions - <b>Dictionary<string, string></b> - the conditions that need to be met for a row or parts of a row to be returned
        /// 
        /// \return - <b>Nothing</b>
        /// 
        public void MakeSelectCommand(List<string> fields, string table, Dictionary<string, string> conditions, Dictionary<string, string> order)
        {
            StringBuilder selectCommand = new StringBuilder(); //instantiate string builder for use in building a query string
            selectCommand.AppendFormat("SELECT"); // set the first part of the query string
            int i = 0;
            int countLoops = fields.Count() - 1;
            foreach (string entry in fields) // iterate through the fields that are to be selected and append them to string
            {
                string entry1;
                if (entry.Contains("'"))
                {
                    entry1 = entry.Replace("'", "''");
                }
                else
                {
                    entry1 = entry;
                }
                if (i == countLoops)
                {
                    selectCommand.AppendFormat(" {0}", entry1);
                }
                else
                {
                    selectCommand.AppendFormat(" {0},", entry1);
                }
                i++;
            }
            selectCommand.AppendFormat(" FROM {0}", table);
            if (conditions!=null) // check if there are optional select conditions
            {
                selectCommand.AppendFormat(" WHERE"); // add conditional WHERE clause
                i = 0;
                countLoops = conditions.Count() - 1;
                foreach (KeyValuePair<string, string> entry in conditions)  // iterate through conditions and append them to string
                {
                    string value1;
                    if (entry.Value.Contains("'"))
                    {
                        value1 = entry.Value.Replace("'", "''");
                    }
                    else
                    {
                        value1 = entry.Value;
                    }
                    if (i == countLoops)
                    {
                        selectCommand.AppendFormat(" {0} = '{1}'", entry.Key, value1);
                    }
                    else
                    {
                        selectCommand.AppendFormat(" {0} = '{1}' AND", entry.Key, value1);
                    }
                    i++;
                }
            }
            if (order!=null)
            {
                selectCommand.AppendFormat(" ORDER BY {0} {1}", order.Keys.First(), order.Values.First());
            }
            selectCommand.AppendFormat(";");
            userCommand = selectCommand.ToString(); // set userCommand to the newly created query string
        }


        /// \brief 
        /// \details <b>Details</b>
        /// 
        /// \param - 
        /// \returns - 
        /// 
        /// \see
        public void MakeDeleteCommand(string table, Dictionary<string, string> conditions)
        {
            StringBuilder selectCommand = new StringBuilder(); //instantiate string builder for use in building a query string
            selectCommand.AppendFormat("DELETE FROM {0}", table); // set the first part of the query string
            int i = 0;           
            if (conditions != null) // check if there are optional select conditions
            {
                selectCommand.AppendFormat(" WHERE"); // add conditional WHERE clause
                i = 0;
                int countLoops = conditions.Count() - 1;
                foreach (KeyValuePair<string, string> entry in conditions)  // iterate through conditions and append them to string
                {
                    string value1;
                    if (entry.Value.Contains("'"))
                    {
                        value1 = entry.Value.Replace("'", "''");
                    }
                    else
                    {
                        value1 = entry.Value;
                    }
                    if (i == countLoops)
                    {
                        selectCommand.AppendFormat(" {0} = '{1}'", entry.Key, value1);
                    }
                    else
                    {
                        selectCommand.AppendFormat(" {0} = '{1}' AND", entry.Key, value1);
                    }
                    i++;
                }
            }
            selectCommand.AppendFormat(";");
            userCommand = selectCommand.ToString(); // set userCommand to the newly created query string
        }


        /// \brief Creates an Update Command
        /// \details <b>Details</b>
        /// Uses a <i>StringBuilder</i> to combine parameters into a usable SQL Command.  It can take any number of variables.  
        /// The command is used to update a field or fields in a table
        /// 
        /// \param - table - <b>string</b> - the table to be inserted into
        /// \param - updateValues - <b>Dictionary<string, string></b> - the columns to be updated and the values to be used
        /// \param - conditions - <b>Dictionary<string, string></b> - the conditions that need to be met for a row or parts of a row to be updated
        /// 
        /// \return - <b>Nothing</b>
        /// 
        public void MakeUpdateCommand(string table, Dictionary<string, string> updateValues, Dictionary<string, string> conditions)
        {
            StringBuilder updateCommand = new StringBuilder(); // instantiate string builder for use in building a query string
            updateCommand.AppendFormat("UPDATE {0} SET", table); // set up 
            int i = 0;
            int countLoops = updateValues.Count() - 1;
            foreach (KeyValuePair<string, string> entry in updateValues)
            {
                string value1;
                if (entry.Value.Contains("'"))
                {
                    value1 = entry.Value.Replace("'", "''");
                }
                else
                {
                    value1 = entry.Value;
                }
                if (i == countLoops)
                {
                    updateCommand.AppendFormat(" {0} = '{1}'", entry.Key, value1);
                }
                else
                {
                    updateCommand.AppendFormat(" {0} = '{1}',", entry.Key, value1);
                }
                i++;
            }
            updateCommand.AppendFormat(" WHERE");
            i = 0;
            countLoops = conditions.Count() - 1;
            foreach (KeyValuePair<string, string> entry in conditions)// iterate through conditions and append them to string
            {
                string value1;
                if (entry.Value.Contains("'"))
                {
                    value1 = entry.Value.Replace("'", "''");
                }
                else
                {
                    value1 = entry.Value;
                }
                if (i == countLoops)
                {
                    updateCommand.AppendFormat(" {0} = '{1}'", entry.Key, value1);
                }
                else
                {
                    updateCommand.AppendFormat(" {0} = '{1}' AND", entry.Key, value1);
                }
                i++;
            }
            updateCommand.AppendFormat(";");
            userCommand = updateCommand.ToString();// set userCommand to the newly created query string
        }


        /// \brief Creates an BackUp script of a <b>MySqlDatabase</b>
        /// \details <b>Details</b>
        /// Connects to a database and uses a 3rd party library called MySqlBackUp.Net <a href="https://www.codeproject.com/Articles/256466/MySqlBackup-NET"></a>
        /// to create a backup script of the database.  The connection is then closed
        /// 
        /// \param - filePath - <b>string</b> - the path to save the backup script to
        /// 
        /// \return - <b>Nothing</b>
        /// 
        public int BackItUp(string filePath)
        {
            try
            {
                currentConnection = DatabaseInteraction.connectToDatabase(connectionString); // create connection
                int didWork = DatabaseInteraction.BackUpDB(currentConnection, filePath); // try to back up database and find out if it worked or not
                DatabaseInteraction.CloseConnection(currentConnection); //close the connection
                return didWork;//return if backup was successful or not
            }
            catch (Exception e)
            {
                Logger.Log("DB Backup Failed - " + e.Message);
                return 1;
            }
            
        }

        /// \brief Restores a <b>MySqlDatabase</b> from a backup script
        /// \details <b>Details</b>
        /// Connects to a database and uses a 3rd party library called MySqlBackUp.Net <a href="https://www.codeproject.com/Articles/256466/MySqlBackup-NET"></a>
        /// to restore the database from a backup script.  The connection is then closed
        /// 
        /// \param - filePath - <b>string</b> - the path that the backup script is saved to.
        /// 
        /// \return - <b>Nothing</b>
        ///
        public int Restore(string filePath)
        {
            try
            {
                currentConnection = DatabaseInteraction.connectToDatabase(connectionString); // connect to database
                int didWork = DatabaseInteraction.RestoreDB(currentConnection, filePath);// restore database 
                DatabaseInteraction.CloseConnection(currentConnection);// close connection
                return didWork;// return if it worked or not
            }
            catch (Exception e)
            {
                Logger.Log("DB Backup Failed - " + e.Message);
                return 1;
            }
        }


        /// \brief A method that allows for INNER JOIN Select queries to be made to the database
        /// \details <b>Details</b>
        /// A method that is used to make INNER JOIN Select queries to the database of the user's choice
        /// \param - fields - <b>List<string></b> - The fields the user wants to be shown
        /// \param - tables - <b>List<string></b> - The tables to inner join on
        /// \param - IDs - <b>List<string></b> - The values to be compared for the join
        /// \returns - <b>Nothing</b>
        /// 
        /// \see MakeSelectCommand(List<string> fields, string table, Dictionary<string, string> conditions, Dictionary<string, string> order)
        public void MakeInnerJoinSelect(List<string> fields, List<string> tables, List<string> IDs, Dictionary<string, string> conditions)
        {
            StringBuilder selectCmd = new StringBuilder();
            selectCmd.Append("SELECT ");
            int countLoops = fields.Count() - 1;
            int i = 0;

            foreach(string field in fields)
            {
                if(i == countLoops)
                {
                    selectCmd.AppendFormat("{0} ", field);
                }
                else
                {
                    selectCmd.AppendFormat("{0}, ", field);
                }
                i++;
            }
            selectCmd.AppendFormat("FROM {0} ", tables[0]);

            for(int j = 0; j < tables.Count; j++) // this is the area that requires a little work.
            {
                if(j > 0)
                {
                    selectCmd.Append("INNER JOIN ");
                    selectCmd.AppendFormat("{0} ON ", tables[j]);
                    selectCmd.AppendFormat("{0}.{1} = {2}.{1} ", tables[j - 1], IDs[j - 1], tables[j]);
                }
            }
            if (conditions != null)
            {
                selectCmd.Append("WHERE ");
                countLoops = conditions.Count() - 1;
                i = 0;
                foreach(KeyValuePair<string, string> entry in conditions)
                {
                    if(i == countLoops)
                    {
                        selectCmd.AppendFormat("{0} = '{1}'", entry.Key, entry.Value);
                    }
                    else
                    {
                        selectCmd.AppendFormat("{0} = '{1}', ", entry.Key, entry.Value);
                    }
                }
            }

            selectCmd.Append(";");
            userCommand = selectCmd.ToString();
        }


        /// \brief A method that generates a SQL Select command using a BETWEEN Clause
        /// \details <b>Details</b>
        /// A method that takes a list of fields to select, a table to select from, a dictionary of conditions that act as delimiters
        /// for a WHERE - BETWEEN clause. The dictionary cannot be null or empty here else an exception will be thrown. The values of the 
        /// kv pairs in the dictionary hold the start or end point for the between condtions, the key is the column.
        /// \param - fields - <b>List<string></b> - The columns the user specifically wants to select
        /// \param - table - <b>string</b> - the table the user wants to perform a select on
        /// \param - searchPoints - <b>Dictionary<string, string></b> - The dictionary containing the column names and value limits to search between.
        /// \returns - <b>Nothing</b> 
        /// 
        public void MakeBetweenSelect(List<string> fields, string table, Dictionary<string, string> searchPoints)
        {
            if(searchPoints == null || searchPoints.Count == 0 )
            {
                throw new ArgumentNullException("searchPoints", "This value cannot be null or empty");
            }

            StringBuilder selectCmd = new StringBuilder();
            selectCmd.Append("SELECT ");
            int countLoops = fields.Count() - 1;
            int i = 0;

            foreach (string field in fields)
            {
                if (i == countLoops)
                {
                    selectCmd.AppendFormat("{0} ", field);
                }
                else
                {
                    selectCmd.AppendFormat("{0}, ", field);
                }
                i++;
            }
            selectCmd.AppendFormat("FROM {0} WHERE ", table);

            i = 0;
            foreach(KeyValuePair<string, string> entry in searchPoints)
            {
                if(i%2 == 0)
                {
                    selectCmd.AppendFormat("{0} BETWEEN {1} AND ", entry.Key, entry.Value);
                }
                else
                {
                    selectCmd.AppendFormat("{0}", entry.Value);
                    if(i != searchPoints.Count - 1)
                    {
                        selectCmd.Append(", ");
                    }
                }
                i++;
            }

            selectCmd.Append(";");
            userCommand = selectCmd.ToString();
        }
    }
}
