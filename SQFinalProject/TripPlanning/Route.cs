//*********************************************
// File			 : Route.cs
// Project		 : PROG2020 - Term Project
// Programmer	 : Nick Byam, Chris Lemon, Deric Kruse, Mark Fraser
// Last Change   : 2020-12-06
//*********************************************
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
            public string Name { get; set; }    //!<name of the city
            public string Next { get; set; }    //!< the next city
            public int Distance { get; set; }   //!<distance to next city
            public double Time { get; set; }    //!<time to next city

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
                Name = newName;
                Next = newNext;
                Distance = newDistance;
                Time = newTime;
            }
        }
        public List<City> Cities { get; set; }  //!<a list holding every city
        public double TotalTime { get; set; }   //!<the time it takes to complete the route
        public int TotalDistance { get; set; }  //!<the total distance it takes to complete the trip
        public bool East { get; set; }          //!<whether the truck is going east or west

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
            Cities = new List<City>();
            TotalTime = 0.0;
            TotalDistance = 0;
            East = false;
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
            string[] retValues = new string[20];
            string[] endValues = new string[20];
            Dictionary<string, string> conditions = new Dictionary<string, string>();

            string curCity = origin;
            string newName = "";
            string newNext = "";
            int newDistance = 0;
            double newTime = 0.0;

            conditions.Add("destCity", origin);                           
            retValues = Controller.GetCityFromDB(conditions).First().Split(',');

            conditions["destCity"] = end;
            endValues = Controller.GetCityFromDB(conditions).First().Split(',');

            if (int.Parse(retValues[0]) < int.Parse(endValues[0]))
            {
                East = true;
            }
            else East = false;
            
            while (curCity != endValues[6] && East ||
                   curCity != endValues[7] && !East) // Stop when current city is the destination's next
            {
                newName = retValues[1];
                if (East)
                {
                    newNext = retValues[6];
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
                    newNext = retValues[7];
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
                Cities.Add(new City(newName, newNext, newDistance, newTime));
                
                curCity = newNext;

                conditions["destCity"] = curCity;
                if (curCity != "END") retValues = Controller.GetCityFromDB(conditions).First().Split(',');
            }

            CalculateTotalKM();
            CalculateTotalTime();
        }

        /// \brief Adds a potential city as the new destination if it is further than the old one
        /// \details <b>Details</b>
        /// Creates a list of cities that the truck must go through to complete it's route.  
        /// Queries database to determine direction, then in a loop to compile a whole list, 
        /// starting at origin and ending at the end point
        /// 
        /// \param - origin - <b>string</b> - the origin city of the truck
        /// \param - end - <b>string</b> - the final stop of truck 
        /// 
        /// \return - exists - <b>bool</b> - tells the truck if the new city is a new destination or just another stop
        /// 
        public bool AddCity(string newDest)
        {
            bool exists = false;
            foreach (City c in Cities)
            {   // Look for new city in current list
                if (c.Name == newDest)
                {
                    exists = true;
                }
            }
            // Add to the list and recalculate totals if it's not already there
            if (!exists && newDest != "END")
            {
                GetCities(Cities.Last().Next, newDest);
            }

            return exists;
        }

        /// \brief Calculate total time remaining on route
        /// \details <b>Details</b>
        /// Iterates through list of cities and adds together the travel times, and stores in in the TotalTime class property
        /// 
        /// \return - <b>Nothing</b>
        /// 
        public void CalculateTotalTime()
        {
            TotalTime = 0.0;

            foreach (City thisCity in Cities)
            {
                if (thisCity.Name != Cities.Last().Name) TotalTime+= thisCity.Time;
            }
            //Round to 2 decimal places
            TotalTime = Math.Round(TotalTime, 2);
        }

        /// \brief Calculate total KM remaining on route
        /// \details <b>Details</b>
        /// Iterates through list of cities and adds together the each km to next city, and stores in in the TotalDistance class property
        /// 
        /// \return - <b>Nothing</b>
        ///
        public void CalculateTotalKM()
        {
            TotalDistance = 0;

            foreach (City thisCity in Cities)
            {
                if (thisCity.Name != Cities.Last().Name) TotalDistance += thisCity.Distance;
            }
        }

        /// \brief Update route info at each city
        /// \details <b>Details</b>
        /// Removes the city just arrived at from the city struct list and call methods to recalculate time and distance left
        /// 
        /// \return - <b>Nothing</b>
        ///
        public void ArriveAtStop()
        {
            Cities.Remove(Cities[0]);
            CalculateTotalKM();
            CalculateTotalTime();
        }
    }
}
