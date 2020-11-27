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
    [TestClass()]
    public class DatabaseTests
    {
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

        [TestMethod()]
        public void ExecuteCommandTest_Functional2()
        {
            Database test = new Database("159.89.117.198", "DevOSHT", "Snodgr4ss!", "cmp");
            List<string> select = new List<string>();
            select.Add("*");
            test.MakeSelectCommand(select, "Contract", null);
            List<string> results = new List<string>();
            results = test.ExecuteCommand();
            Assert.AreEqual(6, results.Count());
        }

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

        [TestMethod()]
        public void MakeInsertCommandTest_Functional()
        {
            string expected = "INSERT INTO carriers VALUES ('test', 'Oshawa', '1', '1', '1.3', '1.4', '1.5')";
            Database test = new Database("192.168.0.197", "tmsadmin", "12345", "tms");
            List<string> carriers = new List<string> { "test", "Oshawa", "1", "1", "1.3", "1.4", "1.5" };
            test.MakeInsertCommand("carriers", carriers);
            Assert.AreEqual(expected, test.userCommand);
        }

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

        [TestMethod()]
        public void BackItUpTest_Functional()
        {
            Database test = new Database("192.168.0.197", "tmsadmin", "12345", "tms");
            int confirm = test.BackItUp("C:\\temp\\backup.sql");
            Assert.AreEqual(0, confirm);
        }

        [TestMethod()]
        public void BackItUpTest_Exception()
        {
            Database test = new Database("192.168.0.197", "tmsadmin", "12345", "tms");
            int confirm = test.BackItUp("X:\\temp\\backup.sql");
            Assert.AreEqual(1, confirm);
        }

        [TestMethod()]
        public void RestoreTest_Functional()
        {
            Database test = new Database("192.168.0.197", "tmsadmin", "12345", "tms");
            int confirm = test.Restore("C:\\temp\\backup.sql");
            Assert.AreEqual(0, confirm);
        }

        [TestMethod()]
        public void RestoreTest_Exception()
        {
            Database test = new Database("192.168.0.197", "tmsadmin", "12345", "tms");
            int confirm = test.Restore("X:\\temp\\backup.sql");
            Assert.AreEqual(1, confirm);
        }
    }
}