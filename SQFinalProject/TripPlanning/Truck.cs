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
            // increase work time, move to next city
        }

        public void Load(TripLine contract)
        {
            // update pallet count
            // Increase work time by 2 hours
            // Add a new TripLine to the List
        }

        public void Unload(TripLine contract)
        {
            // update pallet count
            // Increase work time by 2 hours
            // Remove tripline from the List
        }

    }
}
