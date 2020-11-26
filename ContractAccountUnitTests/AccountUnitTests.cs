using Microsoft.VisualStudio.TestTools.UnitTesting;
using SQFinalProject;
using SQFinalProject.ContactMgmtBilling;
using System;

namespace SQFinalProject.Tests
{
    [TestClass()]
    public class AccountUnitTests
    {
        [TestMethod()]
        public void TestMethod1()
        {
            ContractDetails cd = new ContractDetails("testname", 0, 0, "Toronto", "Montreal", 0);
            Account testAcc = new Account();
            testAcc.AddNewContract(cd.ID, cd);
            Account testAcc2 = new Account(cd);
            Assert.AreEqual(testAcc.GetAllContracts(), testAcc2.GetAllContracts());
        }
    }
}
