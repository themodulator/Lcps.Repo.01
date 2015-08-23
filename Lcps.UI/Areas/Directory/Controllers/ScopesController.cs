using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using Lcps.UI.Context;
using Lcps.DivisionDirectory.Scopes;

namespace Lcps.UI.Areas.Directory.Controllers
{
    public class ScopesController : Controller
    {
        ContextManager _contextManager = new ContextManager();

        // GET: Directory/Scopes
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            return View();
        }

        // POST: DirectorySetup/MembershipScopes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Caption,ScopeQualifier")] MembershipScope membershipScope)
        {
            MembershipScope ms = _contextManager.DirectoryContext.MembershipScopes.First(x => x.Caption.ToLower().Equals(membershipScope.Caption.ToLower()));
            if (ms != null)
                ModelState.AddModelError("", string.Format("{0} already exists as {1}", ms.Caption, ms.ScopeQualifier.ToString()));


            if (membershipScope.ScopeQualifier == MembershipScopeQualifier.None)
                ModelState.AddModelError("", "Please choose a valid qualifier");

            if (ModelState.IsValid)
            {
                membershipScope.MembershipScopeId = Guid.NewGuid();
                _contextManager.DirectoryContext.MembershipScopes.Insert(membershipScope);
                return RedirectToAction("Index");
            }

            return View(membershipScope);
        }

        public ActionResult ClearReserved()
        {
            _contextManager.DirectoryContext.MembershipScopes.ClearReservedValue();

            return View("Index");
        }


             // GET: DirectorySetup/MembershipScopes/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MembershipScope membershipScope = _contextManager.DirectoryContext.MembershipScopes.GetByID(id);
            if (membershipScope == null)
            {
                return HttpNotFound();
            }
            return View(membershipScope);
        }

        // POST: DirectorySetup/MembershipScopes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MembershipScopeId,BinaryValue,Caption,ScopeQualifier,NWUserCaption,PersonnelCaption")] MembershipScope membershipScope)
        {
            if (ModelState.IsValid)
            {
                _contextManager.DirectoryContext.MembershipScopes.Update(membershipScope);
                return RedirectToAction("Index");
            }
            return View(membershipScope);
        }

    }
}