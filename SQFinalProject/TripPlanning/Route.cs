using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQFinalProject.TripPlanning
{    /// 
     /// \class Route
     ///
     /// \brief This class represents the route that one truck might take along it's journey.  
     /// It holds a list of each individual stop, and methods to calculate total trip time and distance. Each <b>Truck</b> class will hold a route
     /// Uses database queries to check for information, which offloads error handling to the <b>DatabaseInteraction</b> class
     ///
     /// \see Trip
     /// \author <i>Chris Lemon, Mark Fraser</i>
     ///
     public class Route
     {
        public struct City //!<a struct holding all the relevant information to calculate a route 
        {
            public string name { get; set; }    //!<name of the city
            public string next { get; set; }    //!< the next city
            public int distance { get; set; }   //!<distance to next city
            public double time { get; set; }    //!<time to next city
            
            /// \brief City
            /// \details <b>Details</b>
            /// Constructor for the City struct. Assigns given values to their appropriate property
            /// 
            /// \param - newName -  <b>string</b> - Name for the new city
            /// \param - newNext -  <b>string</b> - Name for the next city on the route
            /// \param - newDistance - <b>int</b> - Distance to the next city on the route
            /// \param - newTime -  <b>double</b> - Time to the next city on the route
            /// 
            /// \see City
            /// 
            /// \return - <b>Nothing</b>
            /// 
            public City(string newName, string newNext, int newDistance, double newTime)
            {
                name = newName;
                next = newNext;
                distance = newDistance;
                time = newTime;
            }
        }
        public List<City> cities { get; set; }  //!<a list holding every city
        public double totalTime { get; set; }   //!< the time it takes to complete the route
        public int totalDistance { get; set; }  //!< the total distance it takes to complete the trip
        public bool east { get; set; }          //!<whether the truck is going east or west

        /// \brief Route
        /// \details <b>Details</b>
        /// Constructor for the Route class. Assigns empty default values and instantiates the cities List
        /// 
        /// \see Route
        /// 
        /// \return - <b>Nothing</b>
        /// 
        public Route()
        {
            cities = new List<City>();
            totalTime = 0.0;
            totalDistance = 0;
            east = false;
        }

        /// \brief Create a list of cities
        /// \details <b>Details</b>
        /// Creates a list of cities that the truck must go through to complete it's route.  
        /// Queries database to determine direction, then in a loop to compile a whole list, 
        /// starting at origin and ending at the end point
        /// 
        /// \param - origin - <b>string</b> - the origin city of the truck
        /// \param - end - <b>string</b> - the final stop of truck 
        /// 
        /// \return - <b>Nothing</b>
        /// 
        public void GetCities(string origin, string end)
        {
            Database tms = new Database("108.168.17.4","3306", "tmsadmin", "12345", "tms"); /*Consider how to do this... need to not hardcode*/
            string curCity = origin;
            List<string> retValues = new List<string>();
            List<string> endValues = new List<string>();
            Dictionary<string, string> conditions = new Dictionary<string, string>();

            conditions.Add("destCity", origin);                           
            retValues = Controller.GetCityFromDB(tms, conditions);

            conditions["destCity"] = end;
            endValues = Controller.GetCityFromDB(tms, conditions);

            if (int.Parse(retValues[0]) < int.Parse(endValues[0]))
            {
                east = true;
            }
            else east = false;

            string newName = "";
            string newNext = "";
            int newDistance = 0;
            double newTime = 0.0;

            while (curCity != endValues[6])
            {
                newName = retValues[1];
                newNext = retValues[6];
                if (east)
                {
                    if (!int.TryParse(retValues[2], out newDistance))
                    {
                        Logger.Log("Error: Could not parse db return retValue[2] into int newDistance");
                        return;
                    }
                    if (!double.TryParse(retValues[4], out newTime))
                    {
                        Logger.Log("Error: Could not parse db return retValue[4] into double newTime");
                        return;
                    }
                }
                else
                {
                    if (!int.TryParse(retValues[3], out newDistance))
                    {
                        Logger.Log("Error: Could not parse db return retValue[3] into int newDistance");
                        return;
                    }
                    if (!double.TryParse(retValues[5], out newTime))
                    {
                        Logger.Log("Error: Could not parse db return retValue[5] into double newTime");
                        return;
                    }
                }
                City tmpCity = new City(newName, newNext, newDistance, newTime);
                cities.Add(tmpCity);
                
                curCity = newNext;

                conditions["destCity"] = curCity;
                retValues = Controller.GetCityFromDB(tms, conditions);
            }
        }

        /// \brief Calculate total time remaining on route
        /// \details <b>Details</b>
        /// Iterates through list of cities and adds together the travel times, and stores in in the totalTime class property
        /// 
        /// \return - <b>Nothing</b>
        /// 
        public void CalculateTotalTime()
        {
            totalTime = 0.0;

            foreach (City thisCity in cities)
            {
                if (thisCity.name != cities.Last().name) totalTime+= thisCity.time;
            }
            //Round to 2 decimal places
            totalTime = Math.Round(totalTime, 2);
        }

        /// \brief Calculate total KM remaining on route
        /// \details <b>Details</b>
        /// Iterates through list of cities and adds together the each km to next city, and stores in in the totalDistance class property
        /// 
        /// \return - <b>Nothing</b>
        ///
        public void CalculateTotalKM()
        {
            totalDistance = 0;

            foreach (City thisCity in cities)
            {
                if (thisCity.name != cities.Last().name) totalDistance += thisCity.distance;
            }
        }

        /// \brief Update route info at each city
        /// \details <b>Details</b>
        /// Removes the city just arrived at from the city struct list and call methods to recalculate time and distance left
        /// 
        /// \return - <b>Nothing</b>
        ///
        public void arriveAtStop()
        {
            cities.Remove(cities[0]);
            CalculateTotalKM();
            CalculateTotalTime();
        }
    }
}
