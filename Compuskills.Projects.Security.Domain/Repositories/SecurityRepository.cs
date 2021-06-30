using System;
using System.Collections.Generic;
using System.Linq;
using Compuskills.Projects.Security.Domain.DataAccess;
using Compuskills.Projects.Security.Domain.Models;

namespace Compuskills.Projects.Security.Domain.Repositories
{
    public class SecurityRepository
    {
        /// <summary>
        /// Add a new credential to the system.  A credential is a PROOF OF IDENTITY.  It does not
        /// authorize the holder to do anything by itself.  You must use the GrantAccess and
        /// RevokeAccess methods with the new credential for that.
        /// </summary>
        /// <param name="credential">The credential to add.  This should be a security badge, fingerprint, etc</param>
        public void AddCredential(Credential credential)
        {
            using (var ctx = new SecurityContext())
            {
                ctx.Credentials.Add(credential);
                ctx.SaveChanges();
            }
        }
        /// <summary>
        /// Get a list of all authorization attempts between the two dates.  This includes both successes
        /// and failures.
        /// </summary>
        /// <param name="from">Start date</param>
        /// <param name="to">End date</param>
        /// <returns>Attempts</returns>
        public List<AuthorizationAttempt> GetActivity(DateTime from, DateTime to)
        {
            using (var ctx = new SecurityContext())
            {
                var results = from aa in ctx.AuthorizationAttempts
                              where aa.AttemptDate > @from && aa.AttemptDate < to
                              select aa;                
                List<AuthorizationAttempt> authattempts = results.ToList();
                return authattempts;
            }


        }
        /// <summary>
        /// Gets a list of only authorization attempts for a specific door by ID.
        /// </summary>
        /// <param name="from">Start date</param>
        /// <param name="to">End date</param>
        /// <param name="doorId">The door to check</param>
        /// <returns></returns>
        public List<AuthorizationAttempt> GetDoorActivity(DateTime from, DateTime to, int doorId)
        {
            using (var ctx = new SecurityContext())
            {
                var results = from aa in ctx.AuthorizationAttempts
                              where aa.AttemptDate > @from && aa.AttemptDate < to && aa.Door.DoorID == doorId
                              select aa;
                List<AuthorizationAttempt> doorAuthAttempts = results.ToList();

                return doorAuthAttempts;
            }
        }
        /// <summary>
        /// Get a list of only "suspicious" authorization attempts.  An attempt is suspicious if it fails AND
        /// there's not a subsequent success within two minutes.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public List<AuthorizationAttempt> GetSuspiciousActivity(DateTime from, DateTime to)
        {
            using (var ctx = new SecurityContext())
            {
                var results = from aa in ctx.AuthorizationAttempts
                              where aa.AttemptDate > @from && aa.AttemptDate < to
                              select aa;
                List<AuthorizationAttempt> AttmptsList = results.ToList();

                List<AuthorizationAttempt> pendingFailsList = new List<AuthorizationAttempt>();
                List<AuthorizationAttempt> suspAttmptsList = new List<AuthorizationAttempt>();

                for (int i = 0; i < AttmptsList.Count; i++)
                {
                    var current = AttmptsList[i];
                    if (current.Succeeded == false)
                    {
                        pendingFailsList.Add(current);

                    }
                    if (current.Succeeded == true)
                    {
                        if (pendingFailsList.Count > 0)
                        {
                            for (int j = 0; j < (pendingFailsList.Count); j++)
                            {
                                TimeSpan timeSpan = current.AttemptDate - pendingFailsList[j].AttemptDate;
                                if (timeSpan.TotalMinutes > 2)
                                {
                                    suspAttmptsList.Add(pendingFailsList[j]);
                                }
                                pendingFailsList.RemoveAt(j);
                                j--;
                            }
                        }
                    }
                }
                return suspAttmptsList;

            }

        }
        /// <summary>
        /// Grant access to the specified door using the specified set of credentials.  Credentials
        /// always work in SETS.  You need all the credentials in the set for the authorization to pass.
        /// Make sure to store this information in the database in a way that you can query entire sets
        /// at a time in the IsAuthorized method.
        /// </summary>
        /// <param name="doorId">The door to grant access to</param>
        /// <param name="credentials">The credentials to be used</param>
        public void GrantAccess(int doorId, IEnumerable<Credential> credentials)
        {
            using (var ctx = new SecurityContext())
            {
                Door door = ctx.Doors.Find(doorId);

                foreach (var crd in credentials)
                {
                    door.Credentials.Add(crd);
                }
                ctx.SaveChanges();               
            }

        }
        /// <summary>
        /// Determine if the authentication items (ie security badge, key code) are valid to open the
        /// door with the specified ID.  This query should look up the authentication requirements for
        /// the door with specified ID and check if the supplied items meet those requirements.
        /// 
        /// This method should also automatically log the attempted authorization and the result.  You
        /// should call the LogAuthorizationAttempt method to log this.
        /// </summary>
        /// <param name="doorId">Database ID of the door to authorize.</param>
        /// <param name="credentials">Items being used to perform the auth (eg security badge, key code)</param>
        /// <returns>true if the request is authorized, otherwise false</returns>
        public bool IsAuthorized(int doorId, IEnumerable<Credential> credentials)
        {
            using (var ctx = new SecurityContext())
            {
                bool pass = true;
                Door door = ctx.Doors.Find(doorId);
                var doorCT = door.CredentialTypes.ToList();
                List<CredentialType> usedCT = new List<CredentialType>();
                foreach (var crd in credentials)
                {
                    usedCT.Add(crd.CredentialType);
                }
                foreach (var ct in doorCT)
                {
                    if (!usedCT.Contains(ct))
                    {
                        pass = false;
                    }
                }
                var doorCRD = door.Credentials.ToList();
                foreach (var crd in credentials)
                {
                    if (!doorCRD.Contains(crd))
                    {
                        pass = false;
                    }
                }


                return pass;

            }


        }
        /// <summary>
        /// Log an attempt to open a door.  This does not validate the attempt, it only stores the result
        /// in the database.
        /// </summary>
        /// <param name="doorId">The door being accessed</param>
        /// <param name="credentials">The items presented to authorize entry</param>
        /// <param name="result">Whether or not entry was allowed</param>
        public void LogAuthorizationAttempt(int doorId, IEnumerable<Credential> credentials, bool result, DateTime attemptDate)
        {
            using (var ctx = new SecurityContext())
            {
                Door door = ctx.Doors.Find(doorId);
                var credentialsList = credentials.ToList();
                AuthorizationAttempt authorizationAttempt = new AuthorizationAttempt(doorId, credentialsList, result, attemptDate);
                ctx.SaveChanges();
            }
        }
        /// <summary>
        /// Reverse a grant access operation.  When you revoke access for a single credential it revokes
        /// access for any group of credentials that credential was in.
        /// NB: In a "real" app, you would need a much more complex grant / revoke access system to
        /// accomodate changes to individual credentials that don't affect other credentials.
        /// </summary>
        /// <param name="doorId">The door being accessed</param>
        /// <param name="credential">The credential</param>
        public void RevokeAccess(int doorId, Credential credential)
        {
            using (var ctx = new SecurityContext())
            {
                Door door = ctx.Doors.Find(doorId);
                if (credential.EmployeeID.HasValue)
                {
                    int? employeeID = credential.EmployeeID;
                    List<Credential> credentials = ctx.Credentials.Where(c => c.EmployeeID == employeeID).ToList();
                    foreach (Credential crd in credentials)
                    {
                        door.Credentials.Remove(crd);
                    }
                }
                else
                {
                    door.Credentials.Remove(credential);
                }
                ctx.SaveChanges();
            }

        }
    }
}
