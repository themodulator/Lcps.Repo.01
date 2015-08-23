using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using PagedList;

using Lcps.UI.Context;
using Lcps.DivisionDirectory.Ldap;
using Anvil;

namespace Lcps.UI.Areas.Ldap.Controllers
{
    public class PersonalFoldersController : Controller
    {
        ContextManager cm = new ContextManager();


        // GET: Ldap/PersonalFolders
        public ActionResult Index(int? page, long? filter)
        {
            List<PersonalFolder> folders = cm.LdapContext.PersonalFolders.Get().OrderBy(x => x.FolderPath).ToList();


            page = (page == null) ? 1 : page;
            ViewBag.Total = folders.Count();

            PagedList<PersonalFolder> pg = new PagedList<PersonalFolder>(folders, page.Value, 12);


            return View(pg);
        }

        public ActionResult Folder()
        {
            return View();
        }

        public ActionResult Delete(Guid id)
        {
            cm.LdapContext.PersonalFolders.Delete(id);
            return RedirectToAction("Index");
        }

        public ActionResult Submit(string p, long s, Guid id, PersonalFolderIdFormats nf)
        {
            string result = "ok";

            try
            {
                PersonalFolder f = null;
                
                if(id.Equals(Guid.Empty))
                {

                    f = new PersonalFolder()
                    {
                        PersonalFolderKey = Guid.NewGuid(),
                        FolderPath = p,
                        NameFormat = nf,
                        MembershipScope = s
                    };

                    cm.LdapContext.PersonalFolders.Insert(f);
                }
                else
                {
                    f = cm.LdapContext.PersonalFolders.GetByID(id);
                    f.MembershipScope = s;
                    f.FolderPath = p;
                    f.NameFormat = nf;
                    cm.LdapContext.PersonalFolders.Update(f);
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