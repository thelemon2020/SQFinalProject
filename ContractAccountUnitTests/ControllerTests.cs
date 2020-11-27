using Microsoft.VisualStudio.TestTools.UnitTesting;
using SQFinalProject;
using SQFinalProject.ContactMgmtBilling;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContractAccountUnitTests
{
    [TestClass]
    public class ControllerTests
    {
        [TestMethod()]
        public void CreateAccountTest()
        {
            var ac1 = Controller.CreateNewAccount();
            Assert.IsNotNull(ac1);
        }

        [TestMethod()]
        public void CreateContractTest()
        {
            var cd = Controller.CreateNewContract("name", 0, 0, "test", "test1", 0);
            Assert.IsNotNull(cd);
        }

        [TestMethod()]
        public void AddContractsTest()
        {
            var cd = Controller.CreateNewContract("name", 0, 0, "test", "test1", 0);
            var cd2 = Controller.CreateNewContract("name", 0, 0, "test", "test1", 0);
            var ac = Controller.CreateNewAccount(cd);
            Controller.AddContractToAccount(ac, cd2);

            Assert.AreEqual(cd.ToString(), ac.GetAllContracts()[0]);
        }

        [TestMethod()]
        public void GetDatabaseContractsTest()
        {
            var db = new Database("159.89.117.198", "DevOSHT", "Snodgr4ss!", "cmp");
            var result = Controller.GetAllContractsFromDB(db);

            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public void GetDatabaseContractsExceptionTest()
        {
            var db = new Database("129.89.117.198", "DevOSHT", "nodgr4ss", "cmp");
            var result = Controller.GetAllContractsFromDB(db);

            Assert.IsNull(result);
        }

        [TestMethod()]
        public void GetOneContractTest()
        {
            var db = new Database("159.89.117.198", "DevOSHT", "Snodgr4ss!", "cmp");
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("Client_Name", "Space J");
            var result = Controller.SelectContract(db, dict);
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("{0} {1} {2} {3} {4} {5}", result[0], result[1], result[2], result[3], result[4], result[5]);
            string tmp = "Space J 0 0 Kingston Toronto 1";

            Assert.AreEqual(tmp, sb.ToString());
        }

        [TestMethod()]
        public void GetOneContractExceptionTest()
        {
            var db = new Database("59.89.117.198", "DevOSHT", "Snodgr4ss!", "cmp");
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("Job_Type", "1");
            var result = Controller.SelectContract(db, dict);

            Assert.IsNull(result);
        }
    }
}
