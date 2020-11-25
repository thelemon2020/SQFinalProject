using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySql.Data.MySqlClient;
using SQFinalProject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQFinalProject.Tests
{
    [TestClass()]
    public class DatabaseInteractionTests
    {
        [TestMethod()]
        public void connectToDatabaseTest_Functional()
        {
            Database test = new Database("192.168.0.197", "tmsadmin", "12345", "tms");
            Assert.IsNotNull(DatabaseInteraction.connectToDatabase(test.connectionString));
        }

        [TestMethod()]
        public void connectToDatabaseTest_Exception()
        {
            Database test = new Database("192.168.0.198", "tmsadmin", "12345", "tms");
            Assert.AreEqual(null, DatabaseInteraction.connectToDatabase(test.connectionString));
        }

        [TestMethod()]
        public void CommandDatabaseTest_Functional()
        {
            Database test = new Database("192.168.0.197", "tmsadmin", "12345", "tms");
            MySql.Data.MySqlClient.MySqlConnection testConnect = DatabaseInteraction.connectToDatabase(test.connectionString);
            MySql.Data.MySqlClient.MySqlCommand command = new MySql.Data.MySqlClient.MySqlCommand("SELECT * FROM login", testConnect);
            Assert.IsNotNull(DatabaseInteraction.CommandDatabase(command));
        }

        [TestMethod()]
        public void CommandDatabaseTest_Exception()
        {
            Database test = new Database("192.168.0.197", "tmsadmin", "12345", "tms");
            MySql.Data.MySqlClient.MySqlConnection testConnect = DatabaseInteraction.connectToDatabase(test.connectionString);
            MySql.Data.MySqlClient.MySqlCommand command = new MySql.Data.MySqlClient.MySqlCommand("SELECT FROM login", testConnect);
            Assert.AreEqual(null, DatabaseInteraction.CommandDatabase(command));
        }

        [TestMethod()]
        public void CloseConnectionTest_Functional()
        {
            Database test = new Database("192.168.0.197", "tmsadmin", "12345", "tms");
            MySql.Data.MySqlClient.MySqlConnection testConnect = DatabaseInteraction.connectToDatabase(test.connectionString);
            Assert.IsTrue(DatabaseInteraction.CloseConnection(testConnect));
        }

        [TestMethod()]
        public void RestoreDBTest_Functional()
        {
            Database test = new Database("192.168.0.197", "tmsadmin", "12345", "tms");           
            MySqlConnection conn = DatabaseInteraction.connectToDatabase(test.connectionString);
            int actual = DatabaseInteraction.RestoreDB(conn, "c:\\temp\\backup.sql");
            Assert.AreEqual(0, actual);
        }

        [TestMethod()]
        public void RestoreDBTest_Exception()
        {
            Database test = new Database("192.168.0.197", "tmsadmin", "12345", "tms");
            MySqlConnection conn = DatabaseInteraction.connectToDatabase(test.connectionString);
            int actual = DatabaseInteraction.RestoreDB(conn, "x:\\temp\\backup.sql");
            Assert.AreEqual(1, actual);
        }

        [TestMethod()]
        public void BackUpTest_Functional()
        {
            Database test = new Database("192.168.0.197", "tmsadmin", "12345", "tms");
            MySqlConnection conn = DatabaseInteraction.connectToDatabase(test.connectionString);
            int actual = DatabaseInteraction.BackUpDB(conn, "c:\\temp\\backup.sql");
            Assert.AreEqual(0, actual);
        }

        [TestMethod()]
        public void BackUpTest_Exception()
        {
            Database test = new Database("192.168.0.197", "tmsadmin", "12345", "tms");
            MySqlConnection conn = DatabaseInteraction.connectToDatabase(test.connectionString);
            int actual = DatabaseInteraction.BackUpDB(conn, "x:\\temp\\backup.sql");
            Assert.AreEqual(1, actual);
        }
    }
}