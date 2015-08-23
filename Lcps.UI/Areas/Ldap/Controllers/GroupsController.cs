using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Lcps.UI.Context;
using Lcps.DivisionDirectory.Ldap;
using Anvil;
using PagedList;


namespace Lcps.UI.Areas.Ldap.Controllers
{
    public class GroupsController : Controller
    {

        LdapContext ldapContext = new LdapContext(new Lcps.UI.Models.LcpsUiContext());

        // GET: Ldap/Groups
        public ActionResult Index(int? page, long? scope)
        {
            page = (page == null) ? 1 : page.Value;

            List<GroupAssignmentConfig> items = ldapContext.GroupAssignmentConfigs.Get().OrderBy(x => x.Caption).ToList();

            if (scope != null)
                items = items.Where(x => ((x.MembershipScope & scope) == scope)).ToList();

            ViewBag.Total = items.Count();

            PagedList<GroupAssignmentConfig> pg = new PagedList<GroupAssignmentConfig>(items, page.Value, 4);

            return View(pg);
        }

        public ActionResult Create()
        {
            return View();
        }



        public ActionResult CreateGroupConfig(string caption, long filter, Guid id)
        {
            string result = "ok";


            try
            {
                if (filter == 0)
                    throw new Exception("Please select a category to filter members by");

                if (id.Equals(Guid.Empty))
                {
                    GroupAssignmentConfig config = new GroupAssignmentConfig()
                    {
                        Caption = caption,
                        MembershipScope = filter,
                        GroupAssignmentKey = Guid.NewGuid()
                    };

                    ldapContext.GroupAssignmentConfigs.Insert(config);
                }
                else
                {
                    GroupAssignmentConfig config = ldapContext.GroupAssignmentConfigs.GetByID(id);
                    config.Caption = caption;
                    config.MembershipScope = filter;
                    ldapContext.GroupAssignmentConfigs.Update(config);
                }

            }
            catch (Exception ex)
            {
                ExceptionCollector ec = new ExceptionCollector(ex);
                result = ec.ToUL();
            }

            return Content(result);

        }

        public ActionResult Delete(Guid id)
        {
            ldapContext.GroupAssignmentConfigs.Delete(id);
            return RedirectToAction("Index");
        }

        public ActionResult AddAssignment(Guid configId, string groupDn)
        {
            string result = "ok";

            try
            {
                GroupAssignment g = new GroupAssignment()
                {
                    GroupAssignmentKey = Guid.NewGuid(),
                    GroupDN = groupDn,
                    GroupConfigKey = configId

                };

                ldapContext.GroupAssignments.Insert(g);

            }
            catch (Exception ex)
            {
                ExceptionCollector ec = new ExceptionCollector(ex);
                result = ec.ToUL();
            }

            return Content(result);
        }

        public ActionResult RemoveAssignment(Guid id)
        {
            string result = "ok";

            try
            {
                ldapContext.GroupAssignments.Delete(id);
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