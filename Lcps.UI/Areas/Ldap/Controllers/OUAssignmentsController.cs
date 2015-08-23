using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Lcps.Infrastructure;
using Lcps.DivisionDirectory.Ldap;

using Anvil;
using PagedList;

namespace Lcps.UI.Areas.Ldap.Controllers
{
    public class OUAssignmentsController : Controller
    {
        LdapContext context = new LdapContext(new LcpsRepositoryContext());

        // GET: Ldap/OUAssignments
        public ActionResult Index(int? page)
        {

            List<OuAssignment> ou = context.OuAssignments.Get().OrderBy(x => x.OuDistinguishedName).ToList();
            
          

            page = (page == null) ? 1 : page;
            @ViewBag.Page = page.Value;

            PagedList<OuAssignment> pg = new PagedList<OuAssignment>(ou, page.Value, 12);

            return View(pg);
        }


        public ActionResult  Create()
        {
            return View();
        }

        public ActionResult CreateAssignment(string ou, long filter)
        {
            string result = "ok";

            OuAssignment assignment = new OuAssignment() { 
                OuAssignmentKey = Guid.NewGuid(),
                MembershipScope = filter,
                OuDistinguishedName = ou
            };

            try
            {
                context.OuAssignments.Insert(assignment);

            }
            catch(Exception ex)
            {
                ExceptionCollector ec = new ExceptionCollector(ex);
                result = ec.ToUL();
            }

            return Content(result);

        }
    }
}