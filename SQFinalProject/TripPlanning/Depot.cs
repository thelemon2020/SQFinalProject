/*
* FILE : Depot.cs
* PROJECT : SENG2020 - ML04
* PROGRAMMERs : Chris Lemon, Nick Byam, Deric Kruse, Mark Fraser
* FIRST VERSION : 2020 - 11 - 15
* REVISED ON : 2020 - 12 - 07
* DESCRIPTION : This file defines the Depot class.  
*/
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQFinalProject.TripPlanning
{
    /// 
    /// \class Depot
    ///
    /// \brief This class represents a depot at which trucks can pick up orders and depart from
    ///
    /// \see Trip
    /// \author <i>Chris Lemon</i>
    ///
    public class Depot
    {
        public string carrierName { get; set; }
        public string cityName { get; set; }
        public string FTLA { get; set; }
        public string LTLA { get; set; }
        public ObservableCollection<string> depotCityCollection { get; set; }

        public ObservableCollection<string> depotCarrierCollection { get; set; }

        /// \brief Depot
        /// \details <b>Details</b>
        /// Constructor for the Depot class. Assigns given values to their appropriate property
        /// 
        /// \param - carry -  <b>string</b> - Name for the carrier that uses this depot
        /// \param - cityNames -  <b>string</b> - Name for the next city on the route
        /// \param - FTL - <b>string</b> - Amount of available trucks for FTL
        /// \param - LTL -  <b>string</b> - Amount of available trucks for LTL
        /// 
        /// 
        /// \return - <b>Nothing</b>
        /// 
        public Depot(string carry, string cityNames, string FTL, string LTL)
        {
            depotCityCollection = new ObservableCollection<string>();
            depotCarrierCollection = new ObservableCollection<string>();
            carrierName = carry;
            cityName = cityNames;
            FTLA = FTL;
            LTLA = LTL;
        }

        /// \brief Depot
        /// \details <b>Details</b>
        /// Constructor for the Depot class. For assignment later
        /// 
        /// \param - <b>None</b>
        ///  
        /// \return - <b>Nothing</b>
        /// 
        public Depot()
        {

        }
    }
}
