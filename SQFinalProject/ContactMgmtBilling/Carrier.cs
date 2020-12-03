using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SQFinalProject.ContactMgmtBilling
{
    public class Carrier
    {
        public int CarrierID { get; set; }
        public string CarrierName { get; set; }
        public double FTLRate { get; set; }
        public double LTLRate { get; set; }
        public double ReefCharge { get; set; }
        public bool newlyCreated { get; set; }
        public Carrier()
        {

        }
        
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
