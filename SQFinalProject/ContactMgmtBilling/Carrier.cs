using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQFinalProject.ContactMgmtBilling
{
    class Carrier
    {
        public int CarrierID { get; set; }
        public string CarrierName { get; set; }
        public double FTLRate { get; set; }
        public double LTLRate { get; set; }
        public double ReefCharge { get; set; }

        public Carrier(int cID, string cName, double fRate, double lRate, double rCharge)
        {
            CarrierID = cID;
            CarrierName = cName;
            FTLRate = fRate;
            LTLRate = lRate;
            ReefCharge = rCharge;
        }
    }
}
