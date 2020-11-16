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
    public static class DatabaseInteraction
    {
        public static MySqlConnection connectToDatabase(string connectionString)
        {
            MySqlConnection connection = null;
            try
            {
                connection = new MySqlConnection(connectionString);
                connection.Open();
            }
            catch (MySqlException e)
            {
                
                return null;
            }
            return connection;
        }
        public static List<string> CommandDatabase(MySqlCommand DBCommand)
        {
            List<string> results = new List<string>();
            MySqlDataReader reader = DBCommand.ExecuteReader();
            while (reader.Read())
            {
                results.Add(reader[1].ToString());
            }
            return results;
        }
        public static void CloseConnection(MySqlConnection database)
        {
            database.Close();
        }
    }
}
