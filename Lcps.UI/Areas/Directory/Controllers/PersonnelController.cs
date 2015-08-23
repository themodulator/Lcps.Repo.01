using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Lcps.DivisionDirectory.ExternalData;
using PagedList;

namespace Lcps.UI.Areas.Directory.Controllers
{
    public class PersonnelController : Controller
    {
        ExternalDataContext context = new ExternalDataContext();

        // GET: Directory/Personnel
        public ActionResult Index(int? page, string search)
        {
            @ViewBag.Header = "Personnel";

            page = (page == null) ? 1 : page;

            List<Personnel> users = null;
            if (search == null)
                users = context.Personnels.OrderBy(x => x.Last_Name).ThenBy(x => x.First_Name).ThenBy(x => x.Middle_Name)
                    .ToList();
            else
                users = context.Personnels
                    .Where(x => x.SSN.ToLower().Contains(search.ToLower()) |
                        x.Last_Name.ToLower().Contains(search.ToLower()) |
                        x.First_Name.ToLower().Contains(search.ToLower())
                    )
                    .OrderBy(x => x.Last_Name).ThenBy(x => x.First_Name).ThenBy(x => x.Middle_Name)
                    .ToList();


            ViewBag.Action = "Index";
            ViewBag.Page = page.Value;
            ViewBag.Search = search;
            ViewBag.Total = users.Count();

            PagedList<Personnel> pg = new PagedList<Personnel>(users, page.Value, 12);

            return View(pg);
        }

        public ActionResult NotInNwUsers(int? page, string search)
        {
            @ViewBag.Header = "Personnel Not In NWUsers";

            page = (page == null) ? 1 : page;

            List<Personnel> users = null;
            if (search == null)
                users = context.GetPersonnelNotInNWUsers()
                    .ToList();
            else
                users = context.GetPersonnelNotInNWUsers()
                    .Where(x => x.SSN.ToLower().Contains(search.ToLower()) |
                        x.Last_Name.ToLower().Contains(search.ToLower()) |
                        x.First_Name.ToLower().Contains(search.ToLower())
                    )
                    .OrderBy(x => x.Last_Name).ThenBy(x => x.First_Name).ThenBy(x => x.Middle_Name)
                    .ToList();


            ViewBag.Action = "NotInNwUsers";
            ViewBag.Page = page.Value;
            ViewBag.Search = search;
            ViewBag.Total = users.Count();

            PagedList<Personnel> pg = new PagedList<Personnel>(users, page.Value, 12);

            return View("Index", pg);

        }
    }
}