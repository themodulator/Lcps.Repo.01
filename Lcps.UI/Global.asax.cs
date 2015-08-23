using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

using System.Xml.Serialization;
using System.IO;
using Lcps.UI.Configuration;
using Lcps.UI.Context;

namespace Lcps.UI
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // Set the default copnnection string by creating an instance of it
            Lcps.UI.Models.LcpsUiContext db = new Models.LcpsUiContext();

            Lcps.DivisionDirectory.Properties.Settings.Default.ConnectionString = db.Database.Connection.ConnectionString;

            Lcps.DivisionDirectory.DivisionDirectoryContext c = new DivisionDirectory.DivisionDirectoryContext();
            c.MembershipScopes.ClearReservedValue();

          //  ContextManager m = new ContextManager();
           // m.SeedSqlScripts();

        }
    }
}
