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
    public class MainWindowTests
    {
        [TestMethod()]
        public void LoadConfigTest_Functional()
        {
            MainWindow test = new MainWindow();
            test.LoadConfig();
            Assert.IsNotNull(test.TMS_Database);
        }

        [TestMethod()]
        public void LoadConfigTest_Exception()
        {
            MainWindow test = new MainWindow();
            test.LoadConfig();
            Assert.IsTrue(File.Exists("C:\\Users\\Chris\\source\\repos\\SQ\\SQFinalProject\\config\\TMS3.txt"));
        }
    }
}