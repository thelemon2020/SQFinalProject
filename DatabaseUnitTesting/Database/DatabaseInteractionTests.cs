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

    /// 
    /// \class <b>DatabaseInteracticsTest</b>
    ///
    /// \brief Holds many test methods to ensure <b>DatabaseInteractions</b> class is functional
    ///
    /// \see ~DatabaseInteractions
    /// 
    /// \author <i>Chris Lemon</i>
    [TestClass()]
    public class DatabaseInteractionTests
    {
        /// \brief Test that a connection can be made to a database
        /// \details <b>Details</b>
        /// Creates a database object and tries to connection to local database.  Checks that null is not returned, indicating a succesful connection
        /// 
        /// \param - <b>None</b>
        /// 
        /// \return - <B>Nothing</B>
        /// 
        [TestMethod()]
        public void connectToDatabaseTest_Functional()
        {
            Database test = new Database("192.168.0.197", "tmsadmin", "12345", "tms");
            Assert.IsNotNull(DatabaseInteraction.connectToDatabase(test.connectionString));
        }

        /// \brief Test that null is returned if an exception is thrown during a database connection
        /// \details <b>Details</b>
        /// Creates a database object and tries to connection to local database.  A bad IP address is provided.  Checks that null is returned
        /// 
        /// \param - <b>None</b>
        /// 
        /// \return - <B>Nothing</B>
        ///
        [TestMethod()]
        public void connectToDatabaseTest_Exception()
        {
            Database test = new Database("192.168.0.198", "tmsadmin", "12345", "tms");
            Assert.AreEqual(null, DatabaseInteraction.connectToDatabase(test.connectionString));
        }

        /// \brief Test that a query string can be sent to a database and something is returned
        /// \details <b>Details</b>
        /// Creates a database object and tries to connection to local database.  Uses a hardcoded query string to make a query.
        /// Checks that something is returned
        /// 
        /// \param - <b>None</b>
        /// 
        /// \return - <B>Nothing</B>
        ///
        [TestMethod()]
        public void CommandDatabaseTest_Functional()
        {
            Database test = new Database("192.168.0.197", "tmsadmin", "12345", "tms");
            MySql.Data.MySqlClient.MySqlConnection testConnect = DatabaseInteraction.connectToDatabase(test.connectionString);
            MySql.Data.MySqlClient.MySqlCommand command = new MySql.Data.MySqlClient.MySqlCommand("SELECT * FROM login", testConnect);
            Assert.IsNotNull(DatabaseInteraction.CommandDatabase(command));
        }

        /// \brief Test that a query string can be sent to a database and if it fails, null is returned
        /// \details <b>Details</b>
        /// Creates a database object and tries to connection to local database.  Uses a bad hardcoded query string to make a query.
        /// Checks that null is returned
        /// 
        /// \param - <b>None</b>
        /// 
        /// \return - <B>Nothing</B>
        ///
        [TestMethod()]
        public void CommandDatabaseTest_Exception()
        {
            Database test = new Database("192.168.0.197", "tmsadmin", "12345", "tms");
            MySql.Data.MySqlClient.MySqlConnection testConnect = DatabaseInteraction.connectToDatabase(test.connectionString);
            MySql.Data.MySqlClient.MySqlCommand command = new MySql.Data.MySqlClient.MySqlCommand("SELECT FROM login", testConnect);
            Assert.AreEqual(null, DatabaseInteraction.CommandDatabase(command));
        }
        /// \brief Test that a connection can be closed properly
        /// \details <b>Details</b>
        /// Creates a database object and tries to connection to local database then tries to close it.
        /// Checks that something is returned
        /// 
        /// \param - <b>None</b>
        /// 
        /// \return - <B>Nothing</B>
        ///
        [TestMethod()]
        public void CloseConnectionTest_Functional()
        {
            Database test = new Database("192.168.0.197", "tmsadmin", "12345", "tms");
            MySql.Data.MySqlClient.MySqlConnection testConnect = DatabaseInteraction.connectToDatabase(test.connectionString);
            Assert.IsTrue(DatabaseInteraction.CloseConnection(testConnect));
        }
        
        /// \brief Test that if an error happens closing a connection, the exception is caught and false is returned
        /// \details <b>Details</b>
        /// Passes null in to simulate a bad or expired connection.  Check that false is returned
        /// 
        /// \param - <b>None</b>
        /// 
        /// \return - <B>Nothing</B>
        ///
        [TestMethod()]
        public void CloseConnectionTest_Exception()
        {
            Assert.IsFalse(DatabaseInteraction.CloseConnection(null));
        }
        /// \brief Test method to test that RestoreDB method successfully restores a database from a backup script
        /// \details <b>Details</b>
        /// Creates a test connection to local database and tries to restore it with the Restore method.  
        /// Checks that the method returns 0, indicating a successful restore
        /// \param - <b>None</b>
        /// 
        /// \return - <B>Nothing</B>
        /// 
        [TestMethod()]
        public void RestoreDBTest_Functional()
        {
            Database test = new Database("192.168.0.197", "tmsadmin", "12345", "tms");           
            MySqlConnection conn = DatabaseInteraction.connectToDatabase(test.connectionString);
            int actual = DatabaseInteraction.RestoreDB(conn, "c:\\temp\\backup.sql");
            Assert.AreEqual(0, actual);
        }
       
        /// \brief Test method to test that RestoreDB method successfully catches any exceptions and returns null
        /// \details <b>Details</b>
        /// Creates a test connection to local database and tries to restore it with the Restore method.  
        /// Checks that the method returns null, indicating a failed restore
        /// \param - <b>None</b>
        /// 
        /// \return - <B>Nothing</B>
        /// 
        [TestMethod()]
        public void RestoreDBTest_Exception()
        {
            Database test = new Database("192.168.0.197", "tmsadmin", "12345", "tms");
            MySqlConnection conn = DatabaseInteraction.connectToDatabase(test.connectionString);
            int actual = DatabaseInteraction.RestoreDB(conn, "x:\\temp\\backup.sql");
            Assert.AreEqual(1, actual);
        }

        /// \brief Test method to test that BackUp method successfully generates a backup sql script
        /// \details <b>Details</b>
        /// Creates a test connection to local database and tries to back it up with the BackItUp method.  
        /// Checks that the method returns 0, indicating a successful backup
        /// \param - <b>None</b>
        /// 
        /// \return - <B>Nothing</B>
        /// 
        [TestMethod()]
        public void BackUpTest_Functional()
        {
            Database test = new Database("192.168.0.197", "tmsadmin", "12345", "tms");
            MySqlConnection conn = DatabaseInteraction.connectToDatabase(test.connectionString);
            int actual = DatabaseInteraction.BackUpDB(conn, "c:\\temp\\backup.sql");
            Assert.AreEqual(0, actual);
        }

        /// \brief Test method to test that BackUp method catches any exception that may be thrown
        /// \details <b>Details</b>
        /// Creates a test connection to local database and tries to back it up with the BackItUp method.  Bad file path is provided.
        /// Checks that the method returns 1, indicating a failed backup
        /// \param - <b>None</b>
        /// 
        /// \return - <B>Nothing</B>
        /// 
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