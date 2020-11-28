using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQFinalProject.TripPlanning
{    /// 
     /// \class <b>Route</b>
     ///
     /// \brief This class represents the route that one truck might take along it's journey.  
     /// It holds a list of each individual stop, and methods to calculate total trip time and distance. Each <b>Truck</b> class will hold a route
     /// Uses database queries to check for information, which offloads error handling to the 
     ///
     /// \see Trip
     /// \author <i>Chris Lemon</i>
     ///
     class Route
     {
        private struct city //<a struct holding all the relevant information to calculate a route 
        {
            string name { get; set; } //<name of the city
            string next { get; set; } //< the next city
            int distance { get; set; } //<distance to next city
            double time { get; set; } //<time to next city           
        }
        List<city> cities { get; set; } //<a list holding every city
        private double totalTime { get; set; } //< the time it takes to complete the route
        private int totalDistance { get; set; } //< the total distance it takes to complete the trip
        private bool east { get; set; } //<whether the truck is going east or west
        
        public Route()
        {

        }

        /// \brief Create a list of cities
        /// \details <b>Details</b>
        /// Creates a list of cities that the truck must go through to complete it's route.  
        /// Queries database in a loop to compile whole list, starting at origin and ending at the end point
        /// 
        /// \param - origin - <b>string</b> - the origin city of the truck
        /// \param - end - <b>string</b> - the final stop of truck 
        /// 
        /// \return - <b>Nothing</b>
        /// 
        private void GetCities(string origin, string end)
        {

        }

        /// \brief Calculate total time remaining on route
        /// \details <b>Details</b>
        /// Iterates through list of cities and adds together the travel times, and stores in in the totalTime class property
        /// 
        /// \return - <b>Nothing</b>
        /// 
        private void calculateTotalTime()
        {

        }

        /// \brief Calculate total KM remaining on route
        /// \details <b>Details</b>
        /// Iterates through list of cities and adds together the each km to next city, and stores in in the totalDistance class property
        /// 
        /// \return - <b>Nothing</b>
        ///
        private void calculateTotalKM()
        {
          
        }

        /// \brief Update route info at each city
        /// \details <b>Details</b>
        /// Removes the city just arrived at from the city struct list and call methods to recalculate time and distance left
        /// \return - <b>Nothing</b>
        ///
        private void arriveAtStop()
        {

        }
    }
}
