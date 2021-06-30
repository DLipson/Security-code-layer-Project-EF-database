using Compuskills.Projects.Security.Domain.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compuskills.Projects.Security.Domain.DataAccess
{
    public class SecurityContext : DbContext
    {
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Credential> Credentials { get; set; }
        public DbSet<CredentialType> CredentialTypes { get; set; }
        public DbSet<Door> Doors { get; set; }
        public DbSet<AuthorizationAttempt> AuthorizationAttempts { get; set; }



    }
}
