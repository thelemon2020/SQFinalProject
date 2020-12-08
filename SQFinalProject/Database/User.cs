using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQFinalProject
{
    /// 
    /// \class User
    /// 
    /// \brief A class that describes a user of the system, their username, pw, ID, and role
    /// 
    /// \author Chris Lemon
    /// 
    class User
    {
        //! Properties
        public string userID { get; set; } //!< The user's id string
        public string userName { get; set; } //!< The user's username
        public string password { get; set; } //!< The user's password
        public string role { get; set; } //!< The user's role in the system (buyer, planner, admin)
        List<string> roleType { get; set; } //!< A list of the available role types

        /// \brief A constructor for the user class
        /// \details <b>Details</b>
        /// A consturctor for the user class that instantiates the roleType
        /// \param - <b>Nothing</b>
        /// \returns - <b>Nothing</b>
        /// 
        public User()
        {
            roleType = new List<string>();          
        }

        /// \brief A constructor for the user class
        /// \details <b>Details</b>
        /// A constructor for the user class that sets up the users id, username, password, and role
        /// \param - ID - <b>string</b> - The user's ID string
        /// \param - user - <b>string</b> - The user's name
        /// \param - pass - <b>string</b> - The user's password
        /// \param - uRole - <b>string</b> - the user's role
        /// \returns - <b>Nothing</b>
        /// 
        public User(string ID, string user, string pass, string uRole)
        {
            userID = ID;
            userName = user;
            password = pass;
            role = uRole;
            roleType = new List<string>();
        }

    }
}
