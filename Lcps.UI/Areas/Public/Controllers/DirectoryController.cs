using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using System.Data;
using System.Data.Entity;
using System.Net;
using System.Threading.Tasks;
using PagedList;

using Lcps.DivisionDirectory.Members;
using Lcps.Infrastructure;
using Lcps.UI.Context;

namespace Lcps.UI.Areas.Public.Controllers
{
    public class DirectoryController : Controller
    {
        ContextManager _contextManager = new ContextManager();


        // GET: Manage/DirectoryMembers
        public ActionResult Index(int? page, long? filter, string search, string theme)
        {
            long f = (filter == null) ? 0 : filter.Value;

            List<DirectoryMember> members =  this._contextManager.DirectoryContext.DirectoryMembers.GetByFilter(f, search);

            ViewBag.Filter = f;

            if (theme != null)
                Session["Theme"] = theme;

            List<DirectoryMember> dd = new List<DirectoryMember>();

            foreach (DirectoryMember d in members)
            {
                dd.Add(new DirectoryMember(d));
            }

            page = (page == null) ? 1 : page;

            ViewBag.Page = page.Value;

            ViewBag.Total = dd.Count();

            PagedList<DirectoryMember> pg = new PagedList<DirectoryMember>(dd, page.Value, 12);

            return View(pg);
        }

        public ActionResult Filter()
        {
            return View();
        }
    }
}