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

using Lcps.DivisionDirectory.Scopes;
using Lcps.DivisionDirectory.Filters;


namespace Lcps.DivisionDirectory
{
    public interface iDivisionDirectoryRepository
    {
        
        DbSet<MembershipScope> MembershipScopes { get; set; }

        DbSet<MembershipFilter> MembershipFilters { get; set; }

        DbSet<ExternalScope> ExternalScopes { get; set; }

    }
}
