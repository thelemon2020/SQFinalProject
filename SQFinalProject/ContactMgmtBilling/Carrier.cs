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
        
        public Carrier(List<string> details)
        {
            string[] splitDetails = details[0].Split(',');

            CarrierID = int.Parse(splitDetails[0]);
            CarrierName = splitDetails[1];
            FTLRate = double.Parse(details[2]);
            LTLRate = double.Parse(details[3]);
            ReefCharge = double.Parse(details[4]);
        }

        public double CalculateCost(Contract contract)
        {

        }
    }
}
