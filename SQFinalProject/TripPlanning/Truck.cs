using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQFinalProject.TripPlanning
{
    class Truck
    {
        public int TripID { get; set; }
        public int CarrierID { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public string VanType { get; set; }
        public double Rate { get; set; }
        public double ReeferRate { get; set; }
        public List<TripLine> Contracts { get; set; }


        public Truck()
        {
            
        }

    }
}
