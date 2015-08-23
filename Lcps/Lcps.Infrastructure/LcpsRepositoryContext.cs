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

using Lcps.DivisionDirectory.Members;
using Lcps.DivisionDirectory.Ldap;

using Lcps.DivisionDirectory;

namespace Lcps.Infrastructure
{
    public class LcpsRepositoryContext : IdentityDbContext<DirectoryMember>, iDivisionDirectoryRepository
    {
        public LcpsRepositoryContext()
            : base(Properties.Settings.Default.ConnectionString, throwIfV1Schema: false)
        {

        }

        public LcpsRepositoryContext(string connectionNameOrString)
            : base(connectionNameOrString, throwIfV1Schema: false)
        {
            Properties.Settings.Default.ConnectionString = connectionNameOrString;
        }

        public static LcpsRepositoryContext Create()
        {
            return new LcpsRepositoryContext();
        }

        public DbSet<DivisionDirectory.Scopes.MembershipScope> MembershipScopes { get; set; }

        public DbSet<DivisionDirectory.Filters.MembershipFilter> MembershipFilters { get; set; }

        public DbSet<DivisionDirectory.Scopes.ExternalScope> ExternalScopes { get; set; }

        public DbSet<LdapConfig> LdapConfigs { get; set; }

        public DbSet<OuAssignment> OuAssignments { get; set; }

        public DbSet<GroupAssignmentConfig> GroupAssignmentConfigs { get; set; }

        public DbSet<GroupAssignment> GroupAssignments { get; set; }

        public DbSet<PersonalFolder> PersonalFolders { get; set; }

    }
}
