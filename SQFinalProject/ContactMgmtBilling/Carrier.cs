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
        public int FTLA { get; set; }
        public double LTLRate { get; set; }
        public int LTLA { get; set; }
        public double ReefCharge { get; set; }
        public bool newlyCreated { get; set; }
        public List<string> DepotCities { get; set; }
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
            FTLA = int.Parse(details[5]);
            LTLA = int.Parse(details[6]);
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
                        cost += FTLRate * trip.EstKM;
                    }
                    else // LTL 
                    {
                        cost += trip.Quantity * LTLRate * trip.EstKM;
                    }
                }
                else if (contract.VanType == ReefVan) // Reefer Van
                {
                    if (trip.Quantity == 0) // FTL
                    {
                        cost += FTLRate * trip.EstKM * ReefCharge;
                    }
                    else // LTL
                    {
                        cost += trip.Quantity * LTLRate * ReefCharge * trip.EstKM;
                    }
                }

                if (trip.EstTime > workDay)
                {
                    cost += 150.00; // add 150.00 for the first extra day
                    double timeRemaining = trip.EstTime % 12; // check if more than one extra day in delivery time is needed
                    while (timeRemaining > workDay) // for every extra delivery day, add another 150.00
                    {
                        cost += 150.00;
                    }
                }
            }
            return cost;
        }
    }
}
