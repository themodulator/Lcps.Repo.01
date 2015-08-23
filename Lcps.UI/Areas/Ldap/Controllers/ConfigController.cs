using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Lcps.Infrastructure;
using Lcps.DivisionDirectory.Ldap;

using Anvil;

namespace Lcps.UI.Areas.Ldap.Controllers
{
    public class ConfigController : Controller
    {
        LdapContext context = new LdapContext(new LcpsRepositoryContext());

        // GET: Ldap/Config
        public ActionResult Index()
        {
            LdapConfig cfg = context.LdapConfigs.First();

            return View(cfg);
        }

        public ActionResult Update([Bind(Include = "DomainPrincipalName,UserName,Password")] LdapConfig config)
        {
            ViewBag.Result = "ok";

            LdapConfig test = context.LdapConfigs.First(x => x.DomainPrincipalName.ToLower().Equals(config.DomainPrincipalName.ToLower()));

            if(ModelState.IsValid)
            {
                try
                {
                    if (test == null)
                        context.LdapConfigs.Insert(config);
                    else
                        context.LdapConfigs.Update(config);
                }
                catch (Exception ex)
                {
                    ExceptionCollector ec = new ExceptionCollector(ex);
                    ViewBag.Result = ec.ToUL();
                }
            }

            return View("Index");
        }
    }
}