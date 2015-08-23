using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;

using Lcps.Infrastructure;

namespace Lcps.UI.Models
{

    public class LcpsUiContext : LcpsRepositoryContext
    {
        public LcpsUiContext()
            : base(GetConnectionString())
        {
        }
        
        public static string GetConnectionString()
        {
            string defaultCnxn = System.Configuration.ConfigurationManager.AppSettings["CurrentConnectionString"];
            return System.Configuration.ConfigurationManager.ConnectionStrings[defaultCnxn].ConnectionString;
        }
    }
}