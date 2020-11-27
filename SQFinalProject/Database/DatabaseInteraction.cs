using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace SQFinalProject
{   /// 
    /// \class <b>DatabaseInteraction</b>
    ///
    /// \brief The purpose of this class is be used by Database class objects to interact with the actual database
    ///
    /// \author <i>Chris Lemon</i>
    ///
    public static class DatabaseInteraction
    {   /// \brief Used to connect to a database
        /// \details <b>Details</b>
        /// Creates a <i>MySqlConnection</i> and uses it to open a connection to a database and returns that connection for further use
        /// \param - connectionString - <b>string</b> - a string holding all the information neccessary to connect to a database
        /// 
        /// \return - connection - <b>MySqlConnection</b> - The object that represents the connection to a database
        /// 
        public static MySqlConnection connectToDatabase(string connectionString)
        {
            MySqlConnection connection = null;
            
            connection = new MySqlConnection(connectionString);
            connection.Open();
            
            return connection;
        }
        
        /// \brief Sends a query to the database and gets back a response
        /// \details <b>Details</b>
        /// Uses a <i>MySqlCommand</i> object to query the database and creates a <i>MySqlDataReader</i> to get back the the query results
        /// \param - DBCommand - <b>MySqlCommand</b> - an object that holds the connection and the query to be executed on the database
        /// 
        /// \return - results - <b>List<List<string>></b> - A lists of lists that holds each row and each column of the returned query
        /// 
        public static List<string> CommandDatabase(MySqlCommand DBCommand)
        {
            List<string> SQLReturn = new List<string>();
            try
            {
                MySqlDataReader reader = DBCommand.ExecuteReader();
                while (reader.Read())
                {
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        SQLReturn.Add(reader.GetString(i));
                    }
                }
            }
            catch
            {
                SQLReturn = null;
            }           
            return SQLReturn;
        }

        /// \brief Closes a database connection
        /// \details <b>Details</b>
        /// Closes the current connection
        /// \param - database - <b>MySqlConnection</b> - the current database connection
        /// 
        /// \return - <b>Nothing</b>
        /// 
        public static bool CloseConnection(MySqlConnection database)
        {
            database.Close();
            return true;
        }

        /// \brief Backup a <b>MySqlDatabase</b> with a generated .sql script
        /// \details <b>Details</b>
        /// Uses a 3rd party library called MySqlBackUp.Net <ahref="https://www.codeproject.com/Articles/256466/MySqlBackup-NET"></ahref>
        /// to backup the database with a .sql script.
        /// \param - conn - <b>MySqlConnection</b> - the database to be restored
        /// \param - filePath - <b>string</b> - the path that the backup script is saved to.
        /// 
        /// \return - <b>Nothing</b>
        ///
        public static int BackUpDB(MySqlConnection conn, string filePath)
        {
            try
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    using (MySqlBackup mb = new MySqlBackup(cmd))
                    {
                        cmd.Connection = conn;
                        mb.ExportToFile(filePath);
                    }
                }
            }
            catch
            {
                return 1;
            }
            return 0;
        }

        /// \brief Restores a <b>MySqlDatabase</b> from a backup script
        /// \details <b>Details</b>
        /// Uses a 3rd party library called MySqlBackUp.Net <ahref="https://www.codeproject.com/Articles/256466/MySqlBackup-NET"></ahref>
        /// to restore the database from a backup script.
        /// \param - conn - <b>MySqlConnection</b> - the database to be restored
        /// \param - filePath - <b>string</b> - the path that the backup script is saved to.
        /// 
        /// \return - <b>Nothing</b>
        ///
        public static int RestoreDB(MySqlConnection conn, string filePath)
        {
            try
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    using (MySqlBackup mb = new MySqlBackup(cmd))
                    {
                        cmd.Connection = conn;
                        mb.ImportFromFile(filePath);
                    }
                }
            }
            catch
            {
                return 1;
            }
            return 0;           
        }
    }
}
