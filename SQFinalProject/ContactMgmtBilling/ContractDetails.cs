using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQFinalProject.ContactMgmtBilling
{
    public class ContractDetails
    {
        public const double LTLUpCharge = 1.05;
        public const double FTLUpCharge = 1.08;
        public const double ReeferUpCharge = 1.09;
        static private int ContractID = 0;
        public int ID { get; set; }
        public string ClientName { get; set; }
        public int JobType { get; set; }
        public int Quantity { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public double Distance { get; set; }
        public double Rate { get; set; }
        public int VanType { get; set; }
        public bool TripComplete { get; set; }


        public ContractDetails(string name, int job, int quant, string origin, string dest, double rate, int van)
        {
            ID = ContractID;
            ClientName = name;
            JobType = job;
            Quantity = quant;
            Origin = origin;
            Destination = dest;
            Rate = rate;
            VanType = van;
            TripComplete = false;
            Distance = 0.00;
            ContractID++;
        }

        public void ResetIdCount()
        {
            ContractID = 0;
        }
    }
}
