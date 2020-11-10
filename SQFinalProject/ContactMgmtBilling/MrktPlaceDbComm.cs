using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQFinalProject.ContactMgmtBilling
{
    public class MrktPlaceDbComm :DBComm
    {
        public MrktPlaceDbComm(string dbIP, string userName, string password, string command, string table) : base(dbIP, userName, password, command, table)
        {
            
        }

        public ContractDetails SelectContract()
        {
            // Get a contract from the database
        }

        public List<ContractDetails> ShowActiveContracts()
        {
            // return a list populated with active contracts
        }
    }
}
