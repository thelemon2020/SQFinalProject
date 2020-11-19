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
        public static List<List<string>> CommandDatabase(MySqlCommand DBCommand)
        {
            List<List<string>> results = new List<List<string>>();
            List<string> line = new List<string>();
            MySqlDataReader reader = DBCommand.ExecuteReader();
            while (reader.Read())
            {
               for (int i= 0; i<reader.FieldCount; i++)
                {
                    line.Add(reader.GetString(i));
                }
                results.Add(line);
            }
            return results;
        }
        public static void CloseConnection(MySqlConnection database)
        {
            database.Close();
        }
    }
}
