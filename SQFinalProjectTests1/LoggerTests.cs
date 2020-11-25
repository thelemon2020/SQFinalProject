using Microsoft.VisualStudio.TestTools.UnitTesting;
using SQFinalProject;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQFinalProject.Tests
{
    [TestClass()]
    public class LoggerTests
    {
        [TestMethod()]
        public void LogTest_Functional()
        {
            Logger.path = "C:\\temp\\log.txt";
            int actual = Logger.Log("Log File Created");
            Assert.AreEqual(0,actual);
        }

        [TestMethod()]
        public void LogTest_Exception()
        {
            Logger.path = "X:\\temp\\log.txt";
            int actual = Logger.Log("Log File Created");
            Assert.AreEqual(1, actual);
        }
    }

}