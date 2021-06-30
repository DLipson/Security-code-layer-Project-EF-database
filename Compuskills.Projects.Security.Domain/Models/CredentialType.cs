using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compuskills.Projects.Security.Domain.Models
{
   public class CredentialType
    {
        public CredentialType()
        {

        }
        public CredentialType(string credentialTypeName, bool employeeSpecific, bool doorSpecific)
        {
            
            CredentialTypeName = credentialTypeName;
            EmployeeSpecific = employeeSpecific;
            DoorSpecific = doorSpecific;           
        }

        public int CredentialTypeID { get; set; }
        public string CredentialTypeName { get; set; }
        public bool EmployeeSpecific { get; set; }
        public bool DoorSpecific { get; set; }
        public ICollection<Credential> Credentials { get; set; }
        public ICollection<Door> Doors { get; set; }
    }
}
