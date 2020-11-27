using Microsoft.VisualStudio.TestTools.UnitTesting;
using SQFinalProject;
using SQFinalProject.ContactMgmtBilling;
using System;
using System.Collections.Generic;

namespace SQFinalProject.Tests
{
    [TestClass()]
    public class AccountUnitTests
    {
        [TestMethod()]
        public void TestBalancesFunctional()
        {
            ContractDetails cd = new ContractDetails("testname", 0, 0, "Toronto", "Montreal", 0);
            cd.Cost = 500.0;
            cd.Distance = 1000;
            cd.Rate = 4.5;
            cd.FTLUpCharge = 1.05;
            Account testAcc = new Account();
            testAcc.AddNewContract(cd.ID, cd);
            Account testAcc2 = new Account(cd);
            Assert.AreEqual(testAcc.Balance, testAcc2.Balance);
        }

        [TestMethod()]
        public void TestBalancesBoundary()
        {
            ContractDetails cd1 = new ContractDetails("Test", 0, 0, "Toronto", "Montreal", 0);
            ContractDetails cd2 = new ContractDetails("Test2", 0, 0, "Toronto", "Montreal", 0);

            cd1.Cost = 500.0;
            cd2.Cost = 499.999999999;
            cd1.FTLUpCharge = 1.05;
            cd2.FTLUpCharge = 1.05;
            cd1.Distance = 300;
            cd2.Distance = 300.000000001;
            cd1.Rate = 4.5;
            cd2.Rate = 4.5;

            Account a1 = new Account(cd1);
            Account a2 = new Account(cd2);

            Assert.AreNotEqual(a1.Balance, a2.Balance);
        }

        [TestMethod()]
        public void TestBalancesBoundary2()
        {
            ContractDetails cd = new ContractDetails("Test", 1, 20, "Toronto", "Montreal", 1);
            ContractDetails cd2 = new ContractDetails("test2", 1, 20, "Toronto", "Montreal", 1);
            cd.LTLUpCharge = 1.08;
            cd2.LTLUpCharge = 1.079999999999999999;
            cd.ReeferUpCharge = 1.07;
            cd2.ReeferUpCharge = 1.0700000000000001;
            cd.Rate = 0.25;
            cd2.Rate = 0.25;
            cd.Distance = 500;
            cd2.Distance = 500;
            cd.Cost = 150;
            cd2.Cost = 149.999999999999999999;

            Account ac = new Account(cd);
            Account ac2 = new Account(cd2);

            Assert.AreEqual(ac.Balance, ac2.Balance);
        }

        [TestMethod()]
        public void TestContractList()
        {
            ContractDetails cd1 = new ContractDetails("Test", 0, 0, "Windsor", "Moncton", 1);
            ContractDetails cd2 = new ContractDetails("Test", 0, 0, "Windsor", "Moncton", 1);
            ContractDetails cd3 = new ContractDetails("Test", 0, 0, "Windsor", "Moncton", 1);
            List<ContractDetails> tmpList = new List<ContractDetails>();
            tmpList.Add(cd1);
            tmpList.Add(cd2);
            tmpList.Add(cd3);

            Account a1 = new Account(tmpList);
            Account a2 = new Account(tmpList);

            Assert.AreEqual(a1.GetAllContracts()[0], a2.GetAllContracts()[0]);
        }

        [TestMethod()]
        public void TestContractListException()
        {
            ContractDetails cd1 = new ContractDetails("Test", 0, 0, "Windsor", "Moncton", 1);
            ContractDetails cd2 = new ContractDetails("Test", 0, 0, "Windsor", "Moncton", 1);
            ContractDetails cd3 = new ContractDetails("Test", 0, 0, "Windsor", "Moncton", 1);
            List<ContractDetails> tmpList = new List<ContractDetails>();
            tmpList.Add(cd1);
            tmpList.Add(cd2);
            tmpList.Add(cd3);

            Account a1 = new Account(tmpList);
            Account a2 = new Account(tmpList);

            Assert.AreNotEqual(a1.GetAllContracts()[1], a2.GetAllContracts()[2]);
        }

        [TestMethod()]
        public void TestUncalcContracts()
        {
            ContractDetails cd1 = new ContractDetails("Test", 0, 0, "Windsor", "Moncton", 1);
            ContractDetails cd2 = new ContractDetails("Test", 0, 0, "Windsor", "Moncton", 1);
            ContractDetails cd3 = new ContractDetails("Test", 0, 0, "Windsor", "Moncton", 1);
            List<ContractDetails> tmpList = new List<ContractDetails>();
            tmpList.Add(cd1);
            tmpList.Add(cd2);
            tmpList.Add(cd3);

            Account a1 = new Account(tmpList);
            Account a2 = new Account(tmpList);

            Assert.AreEqual(a1.GetUncalcContracts()[2], a2.GetUncalcContracts()[2]);
        }

        [TestMethod()]
        public void TestUncalcContractsExceptional()
        {
            ContractDetails cd1 = new ContractDetails("Test", 0, 0, "Windsor", "Moncton", 1);
            ContractDetails cd2 = new ContractDetails("Test", 0, 0, "Windsor", "Moncton", 1);
            ContractDetails cd3 = new ContractDetails("Test", 0, 0, "Windsor", "Moncton", 1);
            List<ContractDetails> tmpList = new List<ContractDetails>();

            cd1.Cost = 500;
            cd1.Distance = 200;
            cd1.Rate = 4.6;

            tmpList.Add(cd1);
            tmpList.Add(cd2);
            tmpList.Add(cd3);

            Account a1 = new Account(tmpList);
            Account a2 = new Account(tmpList);

            Assert.AreNotEqual(a1.GetUncalcContracts()[0], a2.GetAllContracts()[0]);
        }
    }
}
