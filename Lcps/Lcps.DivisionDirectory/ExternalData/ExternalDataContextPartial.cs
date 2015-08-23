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

using PagedList;

namespace Lcps.DivisionDirectory.ExternalData
{
    public partial class ExternalDataContext
    {
        public List<NWUser> GetUsers(string search)
        {
            List<NWUser> users = new List<NWUser>();

            if (search == null)
                users = NWUsers.OrderBy(x => x.LN).ThenBy(x => x.FN).ThenBy(x => x.MN).ToList();
            else
                users = NWUsers.Where(
                    x => x.LN.ToLower().Contains(search.ToLower()) |
                         x.MN.ToLower().Contains(search.ToLower()) |
                         x.FN.ToLower().Contains(search.ToLower()) |
                         x.EmpType.ToLower().Contains(search.ToLower()) |
                         x.SchPerDir.ToLower().Contains(search.ToLower()) |
                         x.JobTitle.ToLower().Contains(search.ToLower()) |
                         x.EntityID.ToLower().Contains(search.ToLower()) |
                         x.SocSecNbrFormatted.ToLower().Contains(search.ToLower()) |
                         x.UserNameNW.ToLower().Contains(search.ToLower())
                    ).OrderBy(x => x.LN).ThenBy(x => x.FN).ThenBy(x => x.MN)
                    .ToList();

            return users;

        }

        public List<NwUserCandidate> GetCandidates(List<NWUser> users, DbContext db)
        {
            List<NwUserCandidate> list = new List<NwUserCandidate>();

            foreach(NWUser u in users)
            {
                NwUserCandidate c = new NwUserCandidate(u);
                list.Add(c);
            }

            return list;
        }
    }
}
