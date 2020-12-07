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
    /// 
    /// \class Carrier
    ///
    /// \brief The prupose of this class is create representation of a carrier affiliated with the users of the TMS. It holds information
    /// On truck availability and set company rates taken from the carrier table in the database as well as the internal carrier ID and the
    /// Carrier's name.
    ///
    /// \author <i>Nick Byam</i>
    ///
    public class Carrier
    {
        //! properties
        private const double workDay = 12.0; //!< A constant value of a full workday
        private const int FTLMaxLoad = 0; //!< A constant value of max load of an FTL truck
        private const int LTLMaxLoad = 26; //!< Max pallet count for an LTL truck
        private const int DryVan = 0; //!< The int representation of a dry van
        private const int ReefVan = 1;//!< The int representation of a reefer van
        public int CarrierID { get; set; } //!< The id of the carrier
        public string CarrierName { get; set; } //!< The name of the carrier
        public double FTLRate { get; set; } //!< The FTL Rate charged by the carrier
        public double LTLRate { get; set; } //!< The LTL Rate chargd by the carrier
        public double ReefCharge { get; set; } //!< The Reefer van charge of the company
        public bool newlyCreated { get; set; } //!< Whether or not the carrier was newly created
        public List<string> DepotCities { get; set; } //!< The depot cities that the carrier operates out of
        public List<int> FTLA { get; set; } //!< The FTL truck availability of the carrier
        public List<int> LTLA { get; set; } //!< The LTL Truck availability of the carrier


        /// \brief A basuc constructor for the Carrier class
        /// \details <b>Details</b>
        /// A constructor for the carrier class that does nothing except for instantiate it.
        /// \param - <b>Nothing</b>
        /// \returns - <b>Nothing</b>
        /// 
        /// \see Carrier(List<string> details)
        public Carrier()
        {

        }


        /// \brief A constructor for the carrier class that sets properties from a list of strings
        /// \details <b>Details</b>
        /// A constructor for the Carrier class that takes a list of strings which comes from the database to instantiate the class and set
        /// properties so that the carrier class can properly represent a carrier.
        /// \param - details - <b>List<string></b> - The list of details used to set carrier properties
        /// \returns - <b>Nothing</b>
        /// 
        public Carrier(List<string> details)
        {
            CarrierID = int.Parse(details[0]);
            CarrierName = details[1];
            FTLRate = double.Parse(details[2]);
            LTLRate = double.Parse(details[3]);
            ReefCharge = double.Parse(details[4]);
            DepotCities = new List<string>();
            FTLA = new List<int>();
            LTLA = new List<int>();
        }
    }
}
