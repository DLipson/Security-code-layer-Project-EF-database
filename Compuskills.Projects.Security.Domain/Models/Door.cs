using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compuskills.Projects.Security.Domain.Models
{
    public class Door
    {
        public Door()
        {

        }
        public int DoorID { get; set; }
        public string DoorLocation { get; set; }
        public string DoorName { get; set; }
        public virtual ICollection<CredentialType> CredentialTypes { get; set; }
        public virtual ICollection<Credential> Credentials { get; set; }
        public virtual ICollection<Employee> Employees { get; set; }
        public virtual ICollection<AuthorizationAttempt> AuthorizationAttempts { get; set; }

    }
}
