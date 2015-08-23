using System;
using System.Collections.Generic;
using System.IO;
using System.Data.SqlClient;
using System.Collections.Specialized;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

using Anvil.DataContext;
using Lcps.DivisionDirectory.Scopes;
using Lcps.DivisionDirectory.Members;
using Lcps.DivisionDirectory.Filters;
using Lcps.DivisionDirectory.ExternalData;

namespace Lcps.DivisionDirectory
{
    public class DivisionDirectoryContext
    {

        #region Constructors

        public DivisionDirectoryContext()
        {
            this.DbContext = new Infrastructure.LcpsRepositoryContext();

            MembershipScopes = new MembershipScopeRepository();

            MembershipFilters = new MembershipFilterRepository();

            ExternalScopes = new GenericRepository<ExternalScope>(this.DbContext);

            DirectoryMembers = new DirectoryMemberRepository();
        }

        #endregion

        #region Properties

        public Lcps.Infrastructure.LcpsRepositoryContext DbContext { get; set; }

        public MembershipScopeRepository MembershipScopes { get; set; }

        public MembershipFilterRepository MembershipFilters { get; set; }

        public GenericRepository<ExternalScope> ExternalScopes { get; set; }

        public DirectoryMemberRepository DirectoryMembers { get; set; }

        

        #endregion

        #region Get

        public List<DirectoryMember> GetMembersWithNoLdap()
        {
            ExternalDataContext exData = new ExternalDataContext();

            List<DirectoryMember> mm = new List<DirectoryMember>();

            foreach(LdapStaffCandidate c in exData.LdapStaffCandidates.OrderBy(x => x.Surname).ThenBy(x => x.GivenName).ThenBy( x=> x.MiddleName))
            {
                DirectoryMember m = new DirectoryMember(c);
                m.Id = c.Id;
                mm.Add(m);
            }

            return mm;
        }



        #endregion
    }
}
