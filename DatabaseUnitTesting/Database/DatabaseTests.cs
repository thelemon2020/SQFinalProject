using Microsoft.VisualStudio.TestTools.UnitTesting;
using SQFinalProject;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQFinalProject.Tests
{

    /// 
    /// \class <b>DatabaseTest</b>
    ///
    /// \brief Holds many test methods to ensure <b>Database</b> class is functional
    ///
    /// \see Database
    /// 
    /// \author <i>Chris Lemon</i>
    ///
    [TestClass()]
    public class DatabaseTests
    {
        /// \brief Test method to test that the ExecuteCommand method can make a query on a database
        /// \details <b>Details</b>
        /// Creates a test connection to local database and tries to select everything from a single table. 
        /// checks that the correct string was returned
        /// \param - <b>None</b>
        /// 
        /// \return - <B>Nothing</B>
        /// 
        [TestMethod()]
        public void ExecuteCommandTest_Functional1()
        {
            List<string> expected = new List<string>() { "1", "admin", "admin", "a" } ;
            Database test = new Database("192.168.0.197", "tmsadmin", "12345", "tms");
            List<string> select = new List<string>();
            select.Add("*");
            test.MakeSelectCommand(select, "login", null);
            List<string> results = new List<string>();
            results = test.ExecuteCommand();
            Assert.AreEqual(expected.ToString(), results.ToString());
        }

        /// \brief Test method to test that the ExecuteCommand method can make a query on an external database
        /// \details <b>Details</b>
        /// Creates a test connection to external database and tries to select everything from a single table. 
        /// Checks that correct number of entries is returned
        /// \param - <b>None</b>
        /// 
        /// \return - <B>Nothing</B>
        /// 
        [TestMethod()]
        public void ExecuteCommandTest_Functional2()
        {
            Database test = new Database("159.89.117.198", "DevOSHT", "Snodgr4ss!", "cmp");
            List<string> select = new List<string>();
            select.Add("*");
            test.MakeSelectCommand(select, "Contract", null);
            List<string> results = new List<string>();
            results = test.ExecuteCommand();
            Assert.AreEqual(36, results.Count());
        }

        /// \brief Test method to test that the ExecuteCommand method correctly catches an exception thrown by a bad connection
        /// \details <b>Details</b>
        /// Creates a test connection to external database and tries to select everything from a single table. 
        /// Exception should be thrown because the IP address for the server is incorrect
        /// \param - <b>None</b>
        /// 
        /// \return - <B>Nothing</B>
        /// 
        [TestMethod()]
        public void ExecuteCommandTest_Exception1()
        {
            Database test = new Database("159.89.127.198", "DevOSHT", "Snodgr4ss!", "cmp");
            List<string> select = new List<string>();
            select.Add("*");
            test.MakeSelectCommand(select, "Contract", null);
            List<string> results = new List<string>();
            results = test.ExecuteCommand();
            Assert.AreEqual(null, results);
        }

        /// \brief Test method to test that the ExecuteCommand method correctly catches an exception thrown by a bad query
        /// \details <b>Details</b>
        /// Creates a test connection to external database and tries to select everything from a single table. 
        /// Exception should be thrown because the query cannot be processed as the table name is spelled wrong
        /// \param - <b>None</b>
        /// 
        /// \return - <B>Nothing</B>
        /// 
        [TestMethod()]
        public void ExecuteCommandTest_Exception2()
        {
            Database test = new Database("159.89.117.198", "DevOSHT", "Snodgr4ss!", "cmp");
            List<string> select = new List<string>();
            select.Add("*");
            test.MakeSelectCommand(select, "Contractss", null);
            List<string> results = new List<string>();
            results = test.ExecuteCommand();
            Assert.AreEqual(null, results);
        }

        /// \brief Test method to test that the MakeInsertCommand method does indeed make a proper INSERT query string
        /// \details <b>Details</b>
        /// Passes a list of strings into the method to make sure they come out in the right order and format. 
        /// Checks against a prebuilt string that is a proper query string
        /// \param - <b>None</b>
        /// 
        /// \return - <B>Nothing</B>
        /// 
        [TestMethod()]
        public void MakeInsertCommandTest_Functional()
        {
            string expected = "INSERT INTO carriers VALUES ('test', 'Oshawa', '1', '1', '1.3', '1.4', '1.5')";
            Database test = new Database("192.168.0.197", "tmsadmin", "12345", "tms");
            List<string> carriers = new List<string> { "test", "Oshawa", "1", "1", "1.3", "1.4", "1.5" };
            test.MakeInsertCommand("carriers", carriers);
            Assert.AreEqual(expected, test.userCommand);
        }

        /// \brief Test method to test that the MakeInsertCommand method does indeed make a proper INSERT query string with conditions
        /// \details <b>Details</b>
        /// Passes a list of strings into the method to make sure they come out in the right order and format. 
        /// Checks against a prebuilt string that is a proper query string
        /// \param - <b>None</b>
        /// 
        /// \return - <B>Nothing</B>
        /// 
        [TestMethod()]
        public void OverloadedMakeInsertCommandTest_Functional()
        {
            string expected = "INSERT INTO login (username, password, role) VALUES ('planner', 'planner', 'p');";
            Database test = new Database("192.168.0.197", "tmsadmin", "12345", "tms");
            List<string> fields = new List<string> { "username", "password", "role" };
            List<string> credentials = new List<string> { "planner", "planner", "p" };
            test.MakeInsertCommand("login", fields, credentials);
            Assert.AreEqual(expected, test.userCommand);
        }

        /// \brief Test method to test that the MakeSelectCommand method does indeed make a proper SELECT query string
        /// \details <b>Details</b>
        /// Passes a list of strings into the method to make sure they come out in the right order and format. 
        /// Checks against a prebuilt string that is a proper query string
        /// \param - <b>None</b>
        /// 
        /// \return - <B>Nothing</B>
        /// 
        [TestMethod()]
        public void MakeSelectCommandTest_Functional1()
        {
            string expected = ("SELECT * FROM login;");
            Database test = new Database("192.168.0.197", "tmsadmin", "12345", "tms");
            List<string> select = new List<string>();
            select.Add("*");
            test.MakeSelectCommand(select, "login", null);
            Assert.AreEqual(expected, test.userCommand);
        }

        /// \brief Test method to test that the MakeSelectCommand method does indeed make a proper SELECT query string with conditions
        /// \details <b>Details</b>
        /// Passes a list of strings and a dictionary<string,string> into the method to make sure they come out in the right order and format. 
        /// Checks against a prebuilt string that is a proper query string
        /// \param - <b>None</b>
        /// 
        /// \return - <B>Nothing</B>
        /// 
        [TestMethod()]
        public void MakeSelectCommandTest_Functional2()
        {
            string expected = ("SELECT * FROM login WHERE test = 'test';");
            Database test = new Database("192.168.0.197", "tmsadmin", "12345", "tms");
            List<string> select = new List<string>();
            Dictionary<string, string> conditions = new Dictionary<string, string>();
            conditions.Add("test", "test");
            select.Add("*");
            test.MakeSelectCommand(select, "login", conditions);
            Assert.AreEqual(expected, test.userCommand);
        }

        /// \brief Test method to test that the MakeUpdateCommand method does indeed make a proper UPDATE query string
        /// \details <b>Details</b>
        /// Passes a string, a list of strings and a dictionary<string,string> into the method to make sure they come out in the right order and format. 
        /// Checks against a prebuilt string that is a proper query string
        /// \param - <b>None</b>
        /// 
        /// \return - <B>Nothing</B>
        /// 
        [TestMethod()]
        public void MakeUpdateCommandTest_Functional()
        {
            string expected = "UPDATE login SET username = 'admin' WHERE username = 'test';";
            Database test = new Database("192.168.0.197", "tmsadmin", "12345", "tms");
            string table = "login";
            Dictionary<string, string> updates = new Dictionary<string, string>();
            updates.Add("username", "admin");
            Dictionary<string, string> conditions = new Dictionary<string, string>();
            conditions.Add("username", "test");
            test.MakeUpdateCommand(table, updates, conditions);
            Assert.AreEqual(expected, test.userCommand);
        }
        
        /// \brief Test method to test that BackItUp method successfully generates a backup sql script
        /// \details <b>Details</b>
        /// Creates a test connection to local database and tries to back it up with the BackItUp method.  
        /// Checks that the method returns 0, indicating a successful backup
        /// \param - <b>None</b>
        /// 
        /// \return - <B>Nothing</B>
        /// 
        [TestMethod()]
        public void BackItUpTest_Functional()
        {
            Database test = new Database("192.168.0.197", "tmsadmin", "12345", "tms");
            int confirm = test.BackItUp("C:\\temp\\backup.sql");
            Assert.AreEqual(0, confirm);
        }

        /// \brief Test method to test that BackItUp method catches any exception that may be thrown
        /// \details <b>Details</b>
        /// Creates a test connection to local database and tries to back it up with the BackItUp method.  Bad file path is provided.
        /// Checks that the method returns 1, indicating a failed backup
        /// \param - <b>None</b>
        /// 
        /// \return - <B>Nothing</B>
        /// 
        [TestMethod()]
        public void BackItUpTest_Exception()
        {
            Database test = new Database("192.168.0.197", "tmsadmin", "12345", "tms");
            int confirm = test.BackItUp("X:\\temp\\backup.sql");
            Assert.AreEqual(1, confirm);
        }

        /// \brief Test method to test that Restore method successfully restores a database from a backup script
        /// \details <b>Details</b>
        /// Creates a test connection to local database and tries to restore it with the Restore method.  
        /// Checks that the method returns 0, indicating a successful restore
        /// \param - <b>None</b>
        /// 
        /// \return - <B>Nothing</B>
        /// 
        [TestMethod()]
        public void RestoreTest_Functional()
        {
            Database test = new Database("192.168.0.197", "tmsadmin", "12345", "tms");
            int confirm = test.Restore("C:\\temp\\backup.sql");
            Assert.AreEqual(0, confirm);
        }

        /// \brief Test method to test that Restore method catches any exception that may be thrown
        /// \details <b>Details</b>
        /// Creates a test connection to local database and tries to restore it with the Restore method.  Bad file path is provided.
        /// Checks that the method returns 1, indicating a failed restore
        /// \param - <b>None</b>
        /// 
        /// \return - <B>Nothing</B>
        /// 
        [TestMethod()]
        public void RestoreTest_Exception()
        {
            Database test = new Database("192.168.0.197", "tmsadmin", "12345", "tms");
            int confirm = test.Restore("X:\\temp\\backup.sql");
            Assert.AreEqual(1, confirm);
        }
    }
}