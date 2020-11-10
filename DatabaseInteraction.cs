using System;
using System.Collections.Generic;
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
            catch
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
                results.Append(reader[0]);
            }
            return results;
        }
        public static void CloseConnection(MySqlConnection database)
        {
            database.Close();
        }
    }
}
