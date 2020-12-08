using SQFinalProject.ContactMgmtBilling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQFinalProject.TripPlanning
{
    /// 
    /// \class TripLine
    ///
    /// \brief This class joins together a <b>Truck</b> with a <b>Contract</b>.  No real error handling is needed as a <b>Truck</b> and a <b>Contract</b> must exist
    /// in a usable state to get a <b>TripLine</b>
    ///
    /// \see Truck
    /// \see Contract
    /// \author <i>Chris Lemon, Nick Byam</i>
    ///
    public class TripLine
    {
        private const double workDay = 12.0;    //!< A constant value of a full workday
        private const double MaxDriveTime = 8.0;//!< A constant value of the maximum time a driver is allowed to drive
        private const double load = 2.0;
        private const double unload = 2.0;
        private const int FTLMaxLoad = 0;       //!< A constant value of max load of an FTL truck
        private const int LTLMaxLoad = 26;      //!< Max pallet count for an LTL truck
        private const int DryVan = 0;           //!< The int representation of a dry van
        private const int ReefVan = 1;          //!< The int representation of a reefer van
        public int ContractID { get; set; }     //!< the contract associated with the trip line
        public int TripID { get; set; }         //!<the truck associated with the trip line
        public int Quantity { get; set; }       //!< quantity of order
        public string Destination { get; set; } //!< destination city
        public int DaysWorked { get; set; }     //!< days(past the first) taken to complete
        public int Distance { get; set; }       //!< distance to complete
        public bool IsDelivered { get; set; }   //!< Flag to mark the payload delivered
        public double TotalTime { get; set; }   //!< Total time it took to deliver the contract
        public double[] HoursPerDay { get; set; } //!< An array that holds how many hours the trip will take broken into days

        /// \brief Constructor for the TripLine class
        /// \details <b>Details</b>
        /// Sets some general properties based on arguments passed in from another method 
        /// 
        /// \param - contract - <b>Contract</b> - the contract to be loaded onto a <b>Truck</b>
        /// \param - truck - <b>Truck</b> - the <b>Truck</b> to load the contract on to
        /// \param - qty -<b>int</b> - number of pallets to be fulfilled on this trip
        /// 
        /// \return N/A
        /// 
        public TripLine(Contract contract, int tripID, int qty)
        {
            ContractID = contract.ID;
            TripID = tripID;
            Quantity = qty;
            Destination = contract.Destination;
            DaysWorked = 0;
            Distance = 0;
            IsDelivered = false;
            TotalTime = 0;
            HoursPerDay = new double[3];
        }

        /// \brief Constructor for the TripLine class
        /// \details <b>Details</b>
        /// Creates a tripline method from TMS database for the contract class to process after completion
        /// 
        /// \param - details - <b>List<string></b> - the query return of a tripline from the tms db
        /// 
        /// \return N/A
        /// 
        public TripLine(string details, string destination)
        {
            string[] splitDetails = details.Split(',');
            ContractID = int.Parse(splitDetails[0]);
            TripID = int.Parse(splitDetails[1]);
            Quantity = int.Parse(splitDetails[2]);
            Destination = destination;
            DaysWorked = int.Parse(splitDetails[3]);
            Distance = int.Parse(splitDetails[4]);
            if (splitDetails[5] == "0")
            {
                IsDelivered = false;
            }
            else
            {
                IsDelivered = true;
            }
            if (splitDetails[6] == "")
            {
                TotalTime = 0;
            }
            else
            {
                TotalTime = float.Parse(splitDetails[6]);
            }
            HoursPerDay = new double[3];   
        }

        public void SaveToDB()
        {
            Controller.SaveTripLineToDB(this);
        }
        

        public void CalculateTripTime(Truck truck)
        {
            // Get the cities that the trip line will be on, and thus the total km and time of the trip excluding load and unload time
            truck.ThisRoute.GetCities(truck.Origin, Destination);
            TotalTime += truck.ThisRoute.TotalTime;
            Distance = truck.ThisRoute.TotalDistance;

            // fill the array with all 0s
            for(int i = 0; i < HoursPerDay.Length; i++)
            {
                HoursPerDay[i] = 0.00;
            }

            // Now check if the trip will take multiple days without loading and unloading considered
            if (TotalTime <= workDay)
            {
                // The trip is less than 12 hours, but since loading and unloading has not been taken into account, we have to check
                // Whether the driver is driving for 8 hours max a day.
                DaysWorked = 1;

                if (TotalTime <= MaxDriveTime) // the trip time without loading and unloading is less than or equal the max driving time
                {
                    // This means we can add the load and unload time and be sure the trip will only take one day
                    HoursPerDay[0] = TotalTime + load + unload;
                }
                else // The trip will have to be broken up into multiple days because the drive time is greater than 8 hours
                {
                    // Break up the operating time each day by using a modulo operation to see how much driving time is remaining
                    HoursPerDay[0] = TotalTime - (TotalTime % MaxDriveTime) + load; // add load time to the first day's driving time
                    HoursPerDay[1] = (TotalTime % MaxDriveTime) + unload; // Add unload time to the remainder of the driving time

                    DaysWorked++;
                }
            }
            else
            {
                HoursPerDay[0] = TotalTime - (TotalTime % MaxDriveTime) + load;
                HoursPerDay[1] = (TotalTime % MaxDriveTime) + unload;

                DaysWorked = 2;
            }
        }


        /// \brief A method that calculates the total cost of a contract
        /// \details <b>Details</b>
        /// A method that adds up all costs of triplines associated with a contract and returns that cost to be added to an account.
        /// \param - contract - <b>Contract</b> - The contract being evaluated
        /// \returns - cost - <b>double</b> - The calculated cost of the contract
        /// 
        public double CalculateCost(Contract contract, Truck truck)
        {
            double cost = 0.00;

            foreach (TripLine trip in contract.Trips)
            {
                if (contract.VanType == DryVan) // dry van
                {
                    if (trip.Quantity == FTLMaxLoad) // FTL
                    {
                        cost += truck.Rate * trip.Distance;
                    }
                    else // LTL 
                    {
                        cost += trip.Quantity * truck.Rate * trip.Distance;
                    }
                }
                else if (contract.VanType == ReefVan) // Reefer Van
                {
                    if (trip.Quantity == 0) // FTL
                    {
                        cost += truck.Rate * trip.Distance * truck.ReeferRate;
                    }
                    else // LTL
                    {
                        cost += trip.Quantity * truck.Rate * truck.ReeferRate * trip.Distance;
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
