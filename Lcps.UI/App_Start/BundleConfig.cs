using System.Web;
using System.Web.Optimization;

using Lcps.UI.Configuration;
using System.Xml.Serialization;
using System.IO;
using System.Web;

using System.Collections.Generic;

namespace Lcps.UI
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            UIConfiguration cfg = null;

            XmlSerializer xml = new XmlSerializer(typeof(UIConfiguration));


            string root = Path.Combine(HttpContext.Current.Server.MapPath("~/"), "UIConfig.xml");

            using (StreamReader s = new StreamReader(root))
            {
                cfg = (UIConfiguration)xml.Deserialize(s);
            }

            List<string> items = new List<string>();
            items.Add("~/Content/font-awesome.css");
            items.Add("~/Content/site.css");
            items.Add(cfg.DefaultTheme);

            bundles.Add(new StyleBundle("~/Content/css").Include(
                items.ToArray()
            ));
        }
    }
}
