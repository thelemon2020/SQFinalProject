using Microsoft.VisualStudio.TestTools.UnitTesting;
using SQFinalProject;
using SQFinalProject.ContactMgmtBilling;
using SQFinalProject.TripPlanning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQFinalProject.Tests
{
    /// 
    /// \class <b>TruckTests</b>
    ///
    /// \brief Holds many test methods to ensure <b>Route</b> class is functional
    ///
    /// \see Route
    /// 
    /// \author <i>Mark Fraser</i>
    ///
    [TestClass()]
    class TruckTests
    {
        [TestMethod()]
        public void TestTruckContructor()
        {
            Controller.LoadConfig();
            List<Carrier> carriers = Controller.SetupCarriers();
            Dictionary<string, string> cnd = new Dictionary<string, string>();
            cnd.Add("status", "IN-PROGRESS");
            Contract contract = new Contract(Controller.SelectContract(cnd).First());
            Contract contractTwo = new Contract(Controller.SelectContract(cnd).Last());
            Truck theTruck = new Truck(contract, carriers[int.Parse(Controller.FindCarriersForContract(contract, carriers).First())], contract.Quantity - 2);
            theTruck.AddContract(new TripLine(contractTwo, theTruck.TripID, 2));

        }
    }
}
