using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQFinalProject.TripPlanning
{
    class TripLine
    {
        public int ContractID { get; set; }
        public int TripID { get; set; }
        public int Quantity { get; set; }
        public string Destination { get; set; }
        public int EstTime { get; set; }

        public TripLine(int contract, int trip, int qty)
        {
            ContractID = contract;
            TripID = trip;
            Quantity = qty;
            EstTime = 0;
        }
    }
}
