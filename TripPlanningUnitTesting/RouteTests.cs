using Microsoft.VisualStudio.TestTools.UnitTesting;
using SQFinalProject;
using SQFinalProject.TripPlanning;
using SQFinalProject.ContactMgmtBilling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQFinalProject.Tests
{
    /// 
    /// \class <b>RouteTests</b>
    ///
    /// \brief Holds many test methods to ensure <b>Route</b> class is functional
    ///
    /// \see Route
    /// 
    /// \author <i>Mark Fraser</i>
    ///
    [TestClass()]
    public class RouteTests
    {
        [TestMethod()]
        public void TestTruckContructor()
        {
            Controller.LoadConfig();
            List<Carrier> carriers = Controller.SetupCarriers();
            Dictionary<string, string> cnd = new Dictionary<string, string>();
            cnd.Add("status", "IN-PROGRESS");
            List <Contract> contracts = new List<Contract>();
            List<string> details = Controller.GetAllContractsFromDB();
            foreach (string c in details)
            {
                contracts.Add(new Contract(c));
            }
            List<string> s = Controller.FindCarriersForContract(contracts[1], carriers);
            Truck theTruck = new Truck(contracts[1], carriers[1], contracts[1].Quantity - 2);
            theTruck.AddContract(new TripLine(contracts[2], theTruck.TripID, 2));

        }
        /// \brief Test method to test that the GetCities Method can get a city from the tms database
        /// \details <b>Details</b>
        /// Creates a test route and tries to get the route for London to Toronto from the tms database,
        /// and checks to make sure a city named "London" is first in the List
        /// \param - <b>None</b>
        /// 
        /// \return - <B>Nothing</B>
        /// 
        [TestMethod()]
        public void GetCitiesTest_Functional1()
        {
            
            TripPlanning.Route test = new Route();

            test.GetCities("London", "Toronto");
            
            Assert.AreEqual(test.Cities.First().Name, "London");
            
        }

        /// \brief Test method to test that the GetCities Method can get a city from the tms database
        /// \details <b>Details</b>
        /// Creates a test route and tries to get the route for London to Toronto from the tms database,
        /// and checks to make sure the direction was correctly set to east (bool east == true)
        /// \param - <b>None</b>
        /// 
        /// \return - <B>Nothing</B>
        /// 
        [TestMethod()]
        public void GetCitiesTest_Functional2()
        {

            Route test = new Route();

            test.GetCities("London", "Toronto");

            Assert.IsNotNull(test.East);

        }

        /// \brief Test method to test that the GetCities Method can get a city from the tms database
        /// \details <b>Details</b>
        /// Creates a test route and tries to get the route for Windsor to Ottawa from the tms database,
        /// and checks to make sure the final city in the List is Ottawa
        /// \param - <b>None</b>
        /// 
        /// \return - <B>Nothing</B>
        /// 
        [TestMethod()]
        public void GetCitiesTest_Boundary1()
        {

            Route test = new Route();

            test.GetCities("Windsor", "Ottawa");

            Assert.AreEqual("Ottawa", test.Cities.Last().Name);

        }

        /// \brief Test method to test that the CalculateTtotalTime Method can properly calculate time for a route
        /// \details <b>Details</b>
        /// Creates a test route, gets the route for London to Toronto from the tms database,
        /// and checks to make sure the CalculateTotalTime method sets totalTime to 3 hours
        /// \param - <b>None</b>
        /// 
        /// \return - <B>Nothing</B>
        /// 
        [TestMethod()]
        public void CalculateTotalTimeTest_Functional1()
        {

            Route test = new Route();

            test.GetCities("London", "Toronto");
            test.CalculateTotalTime();

            Assert.AreEqual(3, test.TotalTime);

        }

        /// \brief Test method to test that the CalculateTotalTime method can properly calculate time for a route
        /// \details <b>Details</b>
        /// Creates a test route, gets the route for Windsor to Ottawa from the tms database,
        /// and checks to make sure the CalculateTotalTime method sets totalTime to 12.15 hours
        /// \param - <b>None</b>
        /// 
        /// \return - <B>Nothing</B>
        /// 
        [TestMethod()]
        public void CalculateTotalTimeTest_Boundary1()
        {

            Route test = new Route();

            test.GetCities("Windsor", "Ottawa");
            test.CalculateTotalTime();

            Assert.AreEqual(12.15, test.TotalTime);

        }

        /// \brief Test method to test that the CalculateTotalKM Method can properly calculate total distance
        /// \details <b>Details</b>
        /// Creates a test route, gets the route for London to Toronto from the tms database,
        /// and checks to make sure the CalculateTotalKM method sets totalDistance to 196 KM
        /// \param - <b>None</b>
        /// 
        /// \return - <B>Nothing</B>
        /// 
        [TestMethod()]
        public void CalculateTotalKMTest_Functional1()
        {

            Route test = new Route();

            test.GetCities("London", "Toronto");
            test.CalculateTotalKM();

            Assert.AreEqual(196, test.TotalDistance);

        }

        /// \brief Test method to test that the ArriveAtStop Method can properly remove a city
        /// \details <b>Details</b>
        /// Creates a test routefrom London to Toronto and checks to make sure the ArriveAtStop
        /// method removes London so the first city should be Hamilton
        /// \param - <b>None</b>
        /// 
        /// \return - <B>Nothing</B>
        /// 
        [TestMethod()]
        public void ArriveAtStopTest_Functional1()
        {

            Route test = new Route();

            test.GetCities("London", "Toronto");
            test.ArriveAtStop();

            Assert.AreEqual("Hamilton", test.Cities[0].Name);

        }
    }
}
