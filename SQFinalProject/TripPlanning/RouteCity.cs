//*********************************************
// File			 : RouteCity.cs
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
{
    /// 
    /// \class RouteCity
    /// 
    /// \brief A class that represents a city along the route a truck takes
    /// 
    /// \author <i>Chris Lemon</i>
    class RouteCity
    {
        //! Properties
        public int routeID { get; set; }//!< The route ID
        public string cityName { get; set; } //!< The city name
        public int kmToEast { get; set; } //!< How many kilometers to the next east city
        public int kmToWest { get; set; } //!< How many kilometers to the next west city
        public double hToEast { get; set; } //!< How many hours to the next east city
        public double hToWest { get; set; } //!< How many hours to the next west city
        public string east { get; set; } //!< name of the city to the east
        public string west { get; set; } //!< name of the city to the west
        public bool newlyCreated { get; set; } //!< Whether or not the city was newly added


        /// \brief A constructor for the RouteCity Class
        /// \details <b>Details</b>
        /// A basic constructor for the route city, takes no parameters
        /// \param - <b>Nothing</b>
        /// \returns - <b>Nothing</b>
        /// 
        public RouteCity()
        {

        }


        /// \brief A constructor for the route city class
        /// \details <b>Details</b>
        /// A constructor for the RouteCity Class that sets its properties
        /// \param - id - <b>int</b> - The city ID
        /// \param - name - <b>string</b> - The name of the city
        /// \param - kmEast - <b>int</b> - How many kilometers to the next east city
        /// \param - kWest - <b>int</b> - How many kilometers to the next wast city
        /// \param - hEast - <b>double</b> - How many hours to the next east city
        /// \param - hWest - <b>double</b> - How many hours to the next west city
        /// \param - cityeast - <b>string</b> - The name of the city to the east.
        /// \param - citywest - <b>string</b> - The name of the city to the west.
        /// \returns - <b>Nothing</b>
        /// 
        public RouteCity(int id, string name, int kmEast, int kmWest, double hEast, double hWest, string cityeast, string citywest)
        {
            routeID = id;
            cityName = name;
            kmToEast = kmEast;
            kmToWest = kmWest;
            hToEast = hEast;
            hToWest = hWest;
            east = cityeast;
            west = citywest;
        }
    }


}
