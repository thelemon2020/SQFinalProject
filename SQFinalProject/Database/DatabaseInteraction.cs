﻿//*********************************************
// File			 : DatabaseInteraction.cs
// Project		 : PROG2020 - Term Project
// Programmer	 : Nick Byam, Chris Lemon, Deric Kruse, Mark Fraser
// Last Change   : 2020-12-06
//*********************************************
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace SQFinalProject
{   
    /// 
    /// \class DatabaseInteraction
    ///
    /// \brief The purpose of this class is be used by Database class objects to interact with the actual database.  
    /// It contains all the logic neccesary to connect to, disconnect from, query, back up and restore a database.  
    /// Each interaction is surrounded by a <i>try-catch</i> block to catch any exceptions and returns null to the calling method if an exception is caught
    ///
    /// \author <i>Chris Lemon</i>
    ///
    public static class DatabaseInteraction
    {   /// \brief Used to connect to a database
        /// \details <b>Details</b>
        /// Creates a <i>MySqlConnection</i> and uses it to open a connection to a database and returns that connection for further use
        /// 
        /// \param - connectionString - <b>string</b> - a string holding all the information neccessary to connect to a database
        /// 
        /// \return - connection - <b>MySqlConnection</b> - The object that represents the connection to a database
        /// 
        public static MySqlConnection connectToDatabase(string connectionString)
        {
            MySqlConnection connection = null; // create MySqlConnection object           
            try
            {
                connection = new MySqlConnection(connectionString); // instantiate the MySqLConnection object
                connection.Open(); // open connection
            }
            catch (Exception e)
            {
                connection = null;
                Logger.Log("Failed to connect to server - " + e.Message);
            }
            return connection; // return the MySqlConnection object
        }
        
        /// \brief Sends a query to the database and gets back a response
        /// \details <b>Details</b>
        /// Uses a <i>MySqlCommand</i> object to query the database and creates a <i>MySqlDataReader</i> to get back the the query results
        /// 
        /// \param - DBCommand - <b>MySqlCommand</b> - an object that holds the connection and the query to be executed on the database
        /// 
        /// \return - results - <b>List<List<string>></b> - A lists of lists that holds each row and each column of the returned query
        /// 
        public static List<string> CommandDatabase(MySqlCommand DBCommand)
        {
            List<string> SQLReturn = new List<string>();
            try
            {
                MySqlDataReader reader = DBCommand.ExecuteReader(); // instantiate MySqlReader to get query returns from database
                while (reader.Read()) //read all query lines
                {
                    StringBuilder row = new StringBuilder();
                    for (int i = 0; i < reader.FieldCount; i++) //iterate through each field
                    {                        
                        string type = reader.GetDataTypeName(i);
                        type = type.ToLower();
                        if (type.Contains("varchar"))
                        {
                            if (reader.GetValue(i) == null)
                            {
                                row.Append("");
                            }
                            else
                            {
                                row.AppendFormat("{0}", reader.GetString(i)); // add to list of strings
                            }                           
                        }
                        else if(type.Contains("int"))
                        {
                            if (reader.GetValue(i) == null)
                            {
                                row.Append("");
                            }
                            else
                            {
                                row.AppendFormat("{0}", reader.GetInt64(i).ToString()); // add to list of strings
                            }                            
                        }
                        else if (type.Contains("double"))
                        {
                            if (reader.GetValue(i) == null)
                            {
                                row.Append("");
                            }
                            else
                            {
                                row.AppendFormat("{0}", reader.GetDouble(i).ToString()); // add to list of strings
                            }                           
                        }
                        else if(type.Contains("date"))
                        {
                            if(reader.GetValue(i) == null)
                            {
                                row.Append("");
                            }
                            else
                            {
                                row.AppendFormat("{0}", reader.GetDateTime(i).ToString());
                            }
                        }
                        if (i !=reader.FieldCount - 1)
                        {
                            row.AppendFormat(",");
                        }
                    }
                    SQLReturn.Add(row.ToString());
                    row.Clear();
                }
            }
            catch(Exception e)
            {
                SQLReturn = null; //if there is an exception throw, return null
                Logger.Log("MySQL Query Failed- " + e.Message);
            }           
            return SQLReturn;//return list of strings containing query returns
        }

        /// \brief Closes a database connection
        /// \details <b>Details</b>
        /// Closes the current connection and checks that it closed correctly
        /// 
        /// \param - database - <b>MySqlConnection</b> - the current database connection
        /// 
        /// \return - <b>Nothing</b>
        /// 
        public static bool CloseConnection(MySqlConnection database)
        {
            try
            {
                database.Close(); // close the database connection
            }
            catch(Exception e)
            {
                Logger.Log("Failed to close connection to server - " + e.Message);
                return false; //return false if something goes wrong
            }
            return true; // return true if connection closes successfully 
        }

        /// \brief Backup a <b>MySqlDatabase</b> with a generated .sql script
        /// \details <b>Details</b>
        /// Uses a 3rd party library called MySqlBackUp.Net <a href="https://www.codeproject.com/Articles/256466/MySqlBackup-NET"></a>
        /// to backup the database with a .sql script.
        /// 
        /// \param - conn - <b>MySqlConnection</b> - the database to be restored
        /// \param - filePath - <b>string</b> - the path that the backup script is saved to.
        /// 
        /// \return - <b>Nothing</b>
        ///
        public static int BackUpDB(MySqlConnection conn, string filePath)
        {
            try
            {
                using (MySqlCommand cmd = new MySqlCommand()) // create MySqlCommand to connect to database
                {
                    using (MySqlBackup mb = new MySqlBackup(cmd)) // create MySqlBackup object
                    {
                        cmd.Connection = conn; // set connection
                        mb.ExportToFile(filePath); //make backup sql script
                    }
                }
            }
            catch(Exception e)
            {
                Logger.Log("Failed to backup Database - " + e.Message);
                return 1; //returns 1 if backup fails
            }
            return 0; // returns 0 if backup is successful
        }

        /// \brief Restores a <b>MySqlDatabase</b> from a backup script
        /// \details <b>Details</b>
        /// Uses a 3rd party library called MySqlBackUp.Net <a href="https://www.codeproject.com/Articles/256466/MySqlBackup-NET"></a> to restore the database from a backup script.
        /// 
        /// 
        /// \param - conn - <b>MySqlConnection</b> - the database to be restored
        /// \param - filePath - <b>string</b> - the path that the backup script is saved to.
        /// 
        /// \return - <b>Nothing</b>
        ///
        public static int RestoreDB(MySqlConnection conn, string filePath)
        {
            try
            {
                using (MySqlCommand cmd = new MySqlCommand())  // create MySqlCommand to connect to database
                {
                    using (MySqlBackup mb = new MySqlBackup(cmd)) // create MySqlBackup object
                    {
                        cmd.Connection = conn; // set connection
                        mb.ImportFromFile(filePath);  //restore from backup sql script
                    }
                }
            }
            catch(Exception e)
            {
                Logger.Log("Failed to restore Database - " + e.Message);
                return 1; //!<returns 1 if restore fails
            }
            return 0;  //< returns 0 if restore is successful
        }
    }
}
