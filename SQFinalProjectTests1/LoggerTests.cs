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
    /// 
    /// \class <b>LoggerTests</b>
    ///
    /// \brief Holds many test methods to ensure <b>Logger</b> class is functional
    ///
    /// \see Logger
    /// 
    /// \author <i>Chris Lemon</i>
    [TestClass()]
    public class LoggerTests
    {
        /// \brief Tests that a log file can be created 
        /// \details <b>Details</b>
        /// Sets the logger path to a valid filepath and then tries to create a log.  Checks that 0 is returned
        /// \param - <b>None</b>
        /// 
        /// \return - <B>Nothing</B>
        /// 
        [TestMethod()]
        public void LogTest_Functional()
        {
            Logger.path = "C:\\temp\\log.txt";
            int actual = Logger.Log("Log File Created");
            Assert.AreEqual(0,actual);
        }

        /// \brief Tests that a log file can be created 
        /// \details <b>Details</b>
        /// Sets the logger path to a bad filepath and then tries to create a log.  Checks that 1 is returned
        /// \param - <b>None</b>
        /// 
        /// \return - <B>Nothing</B>
        /// 
        [TestMethod()]
        public void LogTest_Exception()
        {
            Logger.path = "X:\\temp\\log.txt";
            int actual = Logger.Log("Log File Created");
            Assert.AreEqual(1, actual);
        }
    }

}