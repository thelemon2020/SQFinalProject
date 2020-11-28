using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQFinalProject.TripPlanning
{
    class Truck
    {
        private const int kMaxPallets = 26;
        public int TripID { get; set; }
        public int CarrierID { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public string VanType { get; set; }
        public double Rate { get; set; }
        public double HoursWorked { get; set; }
        public double ReeferRate { get; set; }
        public List<TripLine> Contracts { get; set; }


        public Truck()
        {
            
        }

        public void ContinueRoute()
        {
            // Move to the next city and simulate the passage of time
            
        }

        public void Load(TripLine contract)
        {
            // minus 2 hours from work day
            // add new contract to List
            // update quantity of pallets
        }

        public void Unload(TripLine contract)
        {
            // minus 2 hours from the work day
            // remove contract from list
            // update quantity of pallets
        }
    }
}
