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
using Lcps.DivisionDirectory.Ldap;
using Lcps.Infrastructure;
using Lcps.UI.Context;
using Lcps.DivisionDirectory.ExternalData;
using Anvil;

namespace Lcps.UI.Areas.Directory.Controllers
{
    public class MembersController : Controller
    {

        public const string DirMemberSessionVar = "DirectoryMemberList";

        #region Fields

        LcpsRepositoryContext _dbContext = new LcpsRepositoryContext();
        ContextManager _contextManager = new ContextManager();

        #endregion

        #region Get

        // GET: Manage/DirectoryMembers
        public ActionResult Index(int? page, long? filter, string search, string theme)
        {
            ViewBag.Error = null;

            

            long f = (filter == null) ? 0 : filter.Value;

            ContextManager cm = new ContextManager();

            List<DirectoryMember> members = cm.DirectoryContext.DirectoryMembers.GetByFilter(f, search);

            ViewBag.Filter = f;

            if (theme != null)
                Session["Theme"] = theme;

            List<DirectoryMember> dd = new List<DirectoryMember>();

            foreach (DirectoryMember d in members)
            {
                DirectoryMember m = new DirectoryMember(d);
                m.Id = d.Id;
                dd.Add(m);
            }

            Session[DirMemberSessionVar] = dd;

            page = (page == null) ? 1 : page;

            ViewBag.Page = page.Value;
            ViewBag.Action = "Index";
            ViewBag.Header = "Directory Members";
            ViewBag.Total = dd.Count();

            PagedList<DirectoryMember> pg = new PagedList<DirectoryMember>(dd, page.Value, 12);

            return View(pg);
        }

        public ActionResult Filter()
        {
            ViewBag.Error = null;
            return View();
        }

        #endregion

        #region Create

        public ActionResult Create()
        {
            ViewBag.Error = null;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="InternalId,Surname,GivenName,MiddleName,DOB,Gender,UserName,Title,InitialPassword,ConfirmPassword,Email")] DirectoryMember member)
        {
            ViewBag.Error = null;

            if(ModelState.IsValid)
            {
                member.Id = Guid.NewGuid().ToString();
                _contextManager.MemberContext.Insert(member);

                return RedirectToAction("Edit", new { id = member.Id });
            }

            return View(member);
        }

        #endregion

        #region Delete

        public ActionResult Delete(string id)
        {
            string result = "ok";

            try
            {
                DirectoryMember m = _contextManager.MemberContext.GetByID(id);

                _contextManager.MemberContext.Delete(m);
            }
            catch (Exception ex)
            {
                result = (new ExceptionCollector(ex).ToUL());
            }

           
            return Content(result);
        }

        #endregion

        #region Edit

        public ActionResult Edit(string id)
        {
            ViewBag.Error = null;

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            DirectoryMemberRepository db = new DirectoryMemberRepository();

            DirectoryMember m = db.GetByID(id);
            
            if (m == null)
            {
                return HttpNotFound();
            }
            return View(m);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,InternalId,Surname,GivenName,MiddleName,DOB,Gender,Email,Title")] DirectoryMember member)
        {
            ViewBag.Error = null;

            DirectoryMember m = _contextManager.MemberContext.GetByID(member.Id);

            try
            {
                m.InternalId = member.InternalId;
                m.Surname = member.Surname;
                m.GivenName = member.GivenName;
                m.MiddleName = member.MiddleName;
                m.DOB = member.DOB;
                m.Gender = member.Gender;
                m.Email = member.Email;
                m.Title = member.Title;

                _contextManager.MemberContext.Update(m);

                ViewBag.Success = true;

                return View(member);
            }            
            catch(Exception ex)
            {
                ExceptionCollector ec = new ExceptionCollector(ex);
                foreach(string s in ec)
                {
                    ModelState.AddModelError("", s);
                }

                ViewBag.Success = false;

                return View(member);

            }
            

        }

        #endregion

        #region Scope

        public ActionResult Scope(string id)
        {
            ViewBag.Error = null;

            DirectoryMember m = _contextManager.MemberContext.GetByID(id);
            return View(m);
        }

        public ActionResult UpdateScope(string id, long s)
        {
            ViewBag.Error = null;

            DirectoryMember m = _contextManager.MemberContext.GetByID(id);
            m.MembershipScope = s;
            _contextManager.MemberContext.Update(m);

            return View("Edit", m);
        }

        #endregion

        #region Staff

        public ActionResult StaffCandidates(int? page, long? filter, string search, string theme)
        {
            ViewBag.Error = null;

            long f = (filter == null) ? 0 : filter.Value;

            ExternalDataContext context = new ExternalDataContext();
            List<StaffCandidate> staff = context.GetSatffCandidatesWithNoAsp().ToList();

            List<DirectoryMember> members = new List<DirectoryMember>();
            foreach(StaffCandidate s in staff)
            {
                DirectoryMember m = new DirectoryMember(s);
                members.Add(m);
            }


            ViewBag.Filter = f;

            if (theme != null)
                Session["Theme"] = theme;

            List<DirectoryMember> dd = new List<DirectoryMember>();

            foreach (DirectoryMember d in members)
            {
                dd.Add(new DirectoryMember(d));
            }

            Session[DirMemberSessionVar] = dd;

            page = (page == null) ? 1 : page;

            ViewBag.Page = page.Value;
            ViewBag.Action = "StaffCandidates";
            ViewBag.Header = "New Staff";
            ViewBag.Total = dd.Count();

            PagedList<DirectoryMember> pg = new PagedList<DirectoryMember>(dd, page.Value, 12);

            return View("Index", pg);
        }

        public ActionResult GetNewLdap(int? page, long? filter, string search, string theme)
        {
            ViewBag.Error = null;

            long f = (filter == null) ? 0 : filter.Value;

            List<DirectoryMember> dd = _contextManager.DirectoryContext.GetMembersWithNoLdap();

            Session[DirMemberSessionVar] = dd;

            ViewBag.Filter = f;

            if (theme != null)
                Session["Theme"] = theme;

            
            page = (page == null) ? 1 : page;

            ViewBag.Page = page.Value;
            ViewBag.Action = "GetNewLdap";
            ViewBag.Header = "New LDAP Accounts";
            ViewBag.Total = dd.Count();

            PagedList<DirectoryMember> pg = new PagedList<DirectoryMember>(dd, page.Value, 12);

            return View("Index", pg);
        }

        public ActionResult LdapConfig(string id)
        {
            LdapUserConfig u = new LdapUserConfig(id, new LcpsRepositoryContext());
            return View(u);
        }

        public ActionResult CreateNewStaff()
        {
            DirectoryMemberRepository mbrCntxt = new DirectoryMemberRepository();

            ExternalDataContext context = new ExternalDataContext();
            List<StaffCandidate> staff = context.GetSatffCandidatesWithNoAsp().ToList();

            List<DirectoryMember> members = new List<DirectoryMember>();
            foreach (StaffCandidate s in staff)
            {
                DirectoryMember m = new DirectoryMember(s);
                m.ConfirmPassword = m.InitialPassword;
                mbrCntxt.Insert(m);
            }

            return RedirectToAction("StaffCandidates");
        }

        public ActionResult CreateNewStaffMember(string id)
        {
            DirectoryMemberRepository mbrCntxt = new DirectoryMemberRepository();

            ExternalDataContext context = new ExternalDataContext();

            StaffCandidate candidate = context.StaffCandidates.First(x => x.InternalId.ToLower().Equals(id.ToLower()));

            DirectoryMember m = new DirectoryMember(candidate);
            m.ConfirmPassword = m.InitialPassword;
            mbrCntxt.Insert(m);

            return RedirectToAction("StaffCandidates");

        }

        #endregion

        #region Export

        public FileResult Download(long? filter, string search)
        {
            Lcps.UI.Context.ContextManager cm = new ContextManager();

            List<DirectoryMember> members = (List<DirectoryMember>)Session[DirMemberSessionVar];

            byte[] fileBytes = DirectoryMemberRepository.SerializeMembers(members);
            string fileName = "directory.xml";
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }

        #endregion

        #region Ldap

        public ActionResult SyncLdapAccount(Guid id)
        {
            string result = "ok";

            LdapUserConfig cfg = new LdapUserConfig(id.ToString(), _dbContext);
            
            try
            {
                cfg.SyncUser(false);
            }
            catch (Exception ex)
            {
                ExceptionCollector ec = new ExceptionCollector(ex);
                result = ec.ToUL();
            }

            return Content(result);
        }

        #endregion

    }
}