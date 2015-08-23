using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Anvil;
using Lcps.UI.Context;
using Lcps.DivisionDirectory.Members;
using Lcps.DivisionDirectory.ExternalData;
using PagedList;

namespace Lcps.UI.Areas.Directory.Controllers
{
    public class NWUsersController : Controller
    {

        ExternalDataContext context = new ExternalDataContext();
        ContextManager cm = new ContextManager();

        // GET: Directory/NWUsers
        public ActionResult Index(int? page, string search)
        {
            page = (page == null) ? 1 : page;

            List<NWUser> users = null;
            if (search == null)
                users = context.NWUsers.OrderBy(x => x.LN).ThenBy(x => x.FN).ThenBy(x => x.MN)
                    .ToList();
            else
                users = context.NWUsers
                    .Where(x => x.EntityID.ToLower().Contains(search.ToLower()) |
                        x.SocSecNbrFormatted.ToLower().Contains(search.ToLower()) |
                        x.LN.ToLower().Contains(search.ToLower()) |
                        x.FN.ToLower().Contains(search.ToLower()) |
                        x.UserNameNW.ToLower().Contains(search.ToLower())
                    )
                    .OrderBy(x => x.LN).ThenBy(x => x.FN).ThenBy(x => x.MN)
                    .ToList();

            ViewBag.Header = "NW Users";
            ViewBag.Action = "Index";
            ViewBag.Page = page.Value;
            ViewBag.Search = search;
            ViewBag.Total = users.Count();

            PagedList<NWUser> pg = new PagedList<NWUser>(users, page.Value, 12);

            return View(pg);
        }

        public ActionResult NwStaff(int? page, string search)
        {
            page = (page == null) ? 1 : page;

            List<NWUser> users = null;
            if (search == null)
                users = context.NWUsers.Where(x => !x.EmpType.ToLower().Equals("student")).OrderBy(x => x.LN).ThenBy(x => x.FN).ThenBy(x => x.MN)
                    .ToList();
            else
                users = context.NWUsers
                    .Where(x => (!x.EmpType.ToLower().Equals("student")) & (x.EntityID.ToLower().Contains(search.ToLower()) |
                        x.SocSecNbrFormatted.ToLower().Contains(search.ToLower()) |
                        x.LN.ToLower().Contains(search.ToLower()) |
                        x.FN.ToLower().Contains(search.ToLower()) |
                        x.UserNameNW.ToLower().Contains(search.ToLower()))
                    )
                    .OrderBy(x => x.LN).ThenBy(x => x.FN).ThenBy(x => x.MN)
                    .ToList();

            ViewBag.Header = "NW Staff";
            ViewBag.Action = "NwStaff";
            ViewBag.Page = page.Value;
            ViewBag.Search = search;
            ViewBag.Total = users.Count();

            PagedList<NWUser> pg = new PagedList<NWUser>(users, page.Value, 12);

            return View("Index", pg);
        }

        public ActionResult NwStudent(int? page, string search)
        {
            page = (page == null) ? 1 : page;

            List<NWUser> users = null;
            if (search == null)
                users = context.NWUsers.Where(x => x.EmpType.ToLower().Equals("student")).OrderBy(x => x.LN).ThenBy(x => x.FN).ThenBy(x => x.MN)
                    .ToList();
            else
                users = context.NWUsers
                    .Where(x => (x.EmpType.ToLower().Equals("student")) & (x.EntityID.ToLower().Contains(search.ToLower()) |
                        x.SocSecNbrFormatted.ToLower().Contains(search.ToLower()) |
                        x.LN.ToLower().Contains(search.ToLower()) |
                        x.FN.ToLower().Contains(search.ToLower()) |
                        x.UserNameNW.ToLower().Contains(search.ToLower()))
                    )
                    .OrderBy(x => x.LN).ThenBy(x => x.FN).ThenBy(x => x.MN)
                    .ToList();

            ViewBag.Header = "NW Student";
            ViewBag.Action = "NwStudent";
            ViewBag.Page = page.Value;
            ViewBag.Search = search;
            ViewBag.Total = users.Count();

            PagedList<NWUser> pg = new PagedList<NWUser>(users, page.Value, 12);

            return View("Index", pg);
        }

        public ActionResult StudentsNoAsp(int? page, string search)
        {
            page = (page == null) ? 1 : page;

            List<StudentCandidate> users = null;

            if (search == null)
                users = context.GetStudentsWithNoAsp()
                    .OrderBy(x => x.Surname).ThenBy(x => x.GivenName).ThenBy(x => x.MiddleName)
                    .ToList();
            else
                users = context.GetStudentsWithNoAsp()
                    .Where(x => x.InternalId.ToLower().Contains(search.ToLower()) |
                        x.Surname.ToLower().Contains(search.ToLower()) |
                        x.GivenName.ToLower().Contains(search.ToLower()) |
                        x.UserName.ToLower().Contains(search.ToLower())
                    )
                    .OrderBy(x => x.Surname).ThenBy(x => x.GivenName).ThenBy(x => x.MiddleName)
                    .ToList();

            ViewBag.Header = "Students with No ASP Account";
            ViewBag.Action = "StudentsNoAsp";
            ViewBag.Page = page.Value;
            ViewBag.Search = search;
            ViewBag.Total = users.Count();

            PagedList<StudentCandidate> pg = new PagedList<StudentCandidate>(users, page.Value, 12);

            return View("Students", pg);
        }

        public ActionResult CreateStudentAsp()
        {
            string result = "ok";

            try
            {
                List<StudentCandidate> users = context.GetStudentsWithNoAsp()
                        .OrderBy(x => x.Surname).ThenBy(x => x.GivenName).ThenBy(x => x.MiddleName)
                        .ToList();

                foreach(IDirectoryMember i in users)
                {
                    DirectoryMember m = new DirectoryMember(i);
                    m.ConfirmPassword = m.InitialPassword;
                    cm.MemberContext.Insert(m);
                }
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