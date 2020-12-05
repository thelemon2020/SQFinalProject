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
        public double TotalTime { get; set; }   //!< the time it takes to complete the route
        public int TotalDistance { get; set; }  //!< the total distance it takes to complete the trip
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
        public void GetCities(Database tms, string origin, string end)
        {
            string curCity = origin;
            List<string> retValues = new List<string>();
            List<string> endValues = new List<string>();
            Dictionary<string, string> conditions = new Dictionary<string, string>();

            conditions.Add("destCity", origin);                           
            retValues = Controller.GetCityFromDB(conditions);

            conditions["destCity"] = end;
            endValues = Controller.GetCityFromDB(conditions);

            if (int.Parse(retValues[0]) < int.Parse(endValues[0]))
            {
                East = true;
            }
            else East = false;

            string newName = "";
            string newNext = "";
            int newDistance = 0;
            double newTime = 0.0;

            while (curCity != endValues[6])
            {
                newName = retValues[1];
                newNext = retValues[6];
                if (East)
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
                Cities.Add(new City(newName, newNext, newDistance, newTime));
                
                curCity = newNext;

                conditions["destCity"] = curCity;
                retValues = Controller.GetCityFromDB(conditions);
            }
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
