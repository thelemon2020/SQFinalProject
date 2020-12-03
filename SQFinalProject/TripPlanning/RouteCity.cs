using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQFinalProject.TripPlanning
{
    class RouteCity
    {
        public int routeID { get; set; }
        public string cityName { get; set; }
        public int kmToEast { get; set; }
        public int kmToWest { get; set; }
        public double hToEast { get; set; }
        public double hToWest { get; set; }
        public string east { get; set; }
        public string west { get; set; }
        public bool newlyCreated { get; set; }

        public RouteCity()
        {

        }
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
