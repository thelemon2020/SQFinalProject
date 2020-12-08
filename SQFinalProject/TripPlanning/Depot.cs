//*********************************************
// File			 : Depot.cs
// Project		 : PROG2020 - Term Project
// Programmer	 : Nick Byam, Chris Lemon, Deric Kruse, Mark Fraser
// Last Change   : 2020-12-06
//*********************************************
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQFinalProject.TripPlanning
{
    public class Depot
    {
        public string carrierName { get; set; }
        public string cityName { get; set; }
        public string FTLA { get; set; }
        public string LTLA { get; set; }
        public ObservableCollection<string> depotCityCollection { get; set; }

        public ObservableCollection<string> depotCarrierCollection { get; set; }
        public Depot(string carry, string cityNames, string FTL, string LTL)
        {
            depotCityCollection = new ObservableCollection<string>();
            depotCarrierCollection = new ObservableCollection<string>();
            carrierName = carry;
            cityName = cityNames;
            FTLA = FTL;
            LTLA = LTL;
        }

        public Depot()
        {

        }
    }
}
