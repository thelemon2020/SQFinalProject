using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQFinalProject
{
    class User
    {
        public string userID { get; set; }
        public string userName { get; set; }
        public string password { get; set; }
        public string role { get; set; }
        List<string> roleType { get; set; }

        public User()
        {
            roleType = new List<string>();          
        }

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
