using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using SQFinalProject.TripPlanning;

namespace SQFinalProject.ContactMgmtBilling
{
    public class Carrier
    {
        private const double workDay = 12.0;
        private const int FTLMaxLoad = 0;
        private const int LTLMaxLoad = 26;
        private const int DryVan = 0;
        private const int ReefVan = 1;
        public int CarrierID { get; set; }
        public string CarrierName { get; set; }
        public double FTLRate { get; set; }
        public double LTLRate { get; set; }
        public double ReefCharge { get; set; }
        public bool newlyCreated { get; set; }
        public List<string> DepotCities { get; set; }
        public List<int> FTLA { get; set; }
        public List<int> LTLA { get; set; }
        public Carrier()
        {

        }
        
        public Carrier(List<string> details)
        {
            CarrierID = int.Parse(details[0]);
            CarrierName = details[1];
            FTLRate = double.Parse(details[2]);
            LTLRate = double.Parse(details[3]);
            ReefCharge = double.Parse(details[4]);
        }

        public double CalculateCost(Contract contract)
        {
            double cost = 0.00;

            foreach (TripLine trip in contract.Trips)
            {
                if (contract.VanType == DryVan) // dry van
                {
                    if (trip.Quantity == FTLMaxLoad) // FTL
                    {
                        cost += FTLRate * trip.Distance;
                    }
                    else // LTL 
                    {
                        cost += trip.Quantity * LTLRate * trip.Distance;
                    }
                }
                else if (contract.VanType == ReefVan) // Reefer Van
                {
                    if (trip.Quantity == 0) // FTL
                    {
                        cost += FTLRate * trip.Distance * ReefCharge;
                    }
                    else // LTL
                    {
                        cost += trip.Quantity * LTLRate * ReefCharge * trip.Distance;
                    }
                }

                if (trip.DaysWorked > 0)
                {
                    cost += 150.00 * trip.DaysWorked; // add 150.00 for each extra day
                }
            }
            return cost;
        }
    }
}
