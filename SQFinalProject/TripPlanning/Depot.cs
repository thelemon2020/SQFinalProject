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
    /// \brief A class representing a depot city
    /// 
    /// \author <i>Chris Lemon</i>

    public class Depot
    {
        //! Properties
        public string carrierName { get; set; } //!< The carrier that operates out of the depot city
        public string cityName { get; set; } //!< the city name
        public string FTLA { get; set; } //!< The FTL availability at the city
        public string LTLA { get; set; } //!< The LTL availability at the city
        public ObservableCollection<string> depotCityCollection { get; set; } //!< A collection of depot cities which the UI can see

        public ObservableCollection<string> depotCarrierCollection { get; set; } //!< A collection of carriers at the depot city


        /// \brief A constructor for the Depot class
        /// \details <b>Details</b>
        /// A constructor for the depot city that instantiates the collection and sets the properties of the class
        /// \param - carry - <b>string</b> - The name of the carrier
        /// \param - cityNames - <b>string</b> - The name of the city
        /// \param - FTL - <b>string</b> - The ftl availability 
        /// \param - LTL - <b>string</b> -The ltl availability
        /// \returns - <b>Nothing</b>
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

        /// \brief A constructor for the Depot class
        /// \details <b>Details</b>
        /// A basic constructor for the depot class that takes no parameters
        /// \param - <b>Nothing</b>
        /// \returns - <b>Nothing</b>
        /// 
        public Depot()
        {

        }
    }
}
