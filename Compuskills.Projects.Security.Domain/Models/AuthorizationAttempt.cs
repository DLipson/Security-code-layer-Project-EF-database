using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compuskills.Projects.Security.Domain.Models
{
    /// <summary>
    /// Tracks an attempt to authorize entry to a door in the building.  Includes the door that
    /// was accessed, the date/time, and the items that were used to authenticate the entry.
    /// </summary>
    public class AuthorizationAttempt
    {
        public AuthorizationAttempt()
        {

        }
        public AuthorizationAttempt(int doorID, ICollection<Credential> credentials, bool succeeded, DateTime attemptDate)
        {
            DoorID = doorID;
            Credentials = credentials;
            Succeeded = succeeded;
            AttemptDate = attemptDate;
        }

        public int AuthorizationAttemptID { get; set; }
        public virtual Door Door { get; set; }
        public int DoorID { get; set; }
        public DateTime AttemptDate { get; set; }
        public virtual ICollection<Credential> Credentials { get; set; } 
        public bool Succeeded { get; set; }


        
    }
}
