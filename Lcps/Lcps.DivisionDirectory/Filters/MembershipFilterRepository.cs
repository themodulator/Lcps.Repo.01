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

namespace Lcps.DivisionDirectory.Filters
{
    public class MembershipFilterRepository : GenericRepository<MembershipFilter>
    {
        public MembershipFilterRepository()
            : base(Lcps.Infrastructure.LcpsRepositoryContext.Create())
        {
            this.DbContext = Lcps.Infrastructure.LcpsRepositoryContext.Create();
        }


        #region Properties

        public Lcps.Infrastructure.LcpsRepositoryContext DbContext { get; set; }

        #endregion
    }
}
