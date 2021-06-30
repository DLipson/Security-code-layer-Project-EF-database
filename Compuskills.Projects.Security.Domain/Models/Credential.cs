using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compuskills.Projects.Security.Domain.Models
{
    /// <summary>
    /// PROOF of identity.  Credentials only prove who a person is (authentication), they do
    /// not tell us if the person is allowed to open a door.  That's a separate process called
    /// authorization.  Your system will need to track credentials and access rights separately.
    /// Bear in mind that access rights for a given credential might change over time.  For example,
    /// an employee might be promoted and their existing security badge will need to let them in
    /// to more doors.
    /// 
    /// To start with there are three types of credentials available: 1) Security Badge, 2) Fingerprint,
    /// and 3) Key Code
    /// </summary>
    public class Credential
    {
        public Credential()
        {

        }
        public Credential(int credentialTypeID,string credentialCode)
        {            
            CredentialTypeID = credentialTypeID;
            CredentialCode = credentialCode;
        }

        public int CredentialID { get; set; }
        public virtual Employee Employee { get; set; }
        public virtual CredentialType CredentialType { get; set; }
        public int? EmployeeID { get; set; }
        public int CredentialTypeID { get; set; }
        public string CredentialCode { get; set; }
        public virtual ICollection<AuthorizationAttempt> AuthorizationAttempts { get; set; }
    }
}
