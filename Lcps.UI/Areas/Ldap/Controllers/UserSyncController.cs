using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Lcps.DivisionDirectory.Ldap;
using Lcps.DivisionDirectory.Members;
using System.IO;
using Lcps.UI.Context;
using Anvil;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;

namespace Lcps.UI.Areas.Ldap.Controllers
{
    public class UserSyncController : Controller
    {
        ContextManager _contextManager = new ContextManager();

        // GET: Ldap/UserSync
        public ActionResult Index(Guid id)
        {

            LdapUserConfig uc = new LdapUserConfig(id.ToString(), new Lcps.UI.Models.LcpsUiContext());

            return View(uc);
        }

        public ActionResult CreateAccount(string memberId)
        {
            string result = "ok";

            try
            {
                LdapUserConfig uc = new LdapUserConfig(memberId, new Lcps.UI.Models.LcpsUiContext());
                uc.SyncUser(false);
            }
            catch(Exception ex)
            {
                ExceptionCollector ec = new ExceptionCollector(ex);
                result = ec.ToUL();
            }

            return Content(result);
        }

        public ActionResult SyncGroups(Guid id)
        {
            string result = "ok";

            try
            {
                LdapUserConfig uc = new LdapUserConfig(id.ToString(), new Lcps.UI.Models.LcpsUiContext());
                uc.SyncGroupMemberships(true);
            }
            catch(Exception ex)
            {
                ExceptionCollector ec = new ExceptionCollector(ex);
                result = ec.ToUL();
            }

            return Content(result);
        }

        public ActionResult MemberList()
        {
            return View();
        }

        public ActionResult RenameFolder(string currentName, string memberId, PersonalFolderIdFormats format)
        {
            string result = "ok";

            try
            {
                DirectoryInfo d = new DirectoryInfo(currentName);
                DirectoryMember m = _contextManager.DirectoryContext.DirectoryMembers.GetByID(memberId);

                string newName = LdapUserConfig.FormatFolderName(m, format);
                string thisPath = Path.GetDirectoryName(d.FullName);
                string newPath = Path.Combine(thisPath, newName);
                d.MoveTo(newPath);
            }
            catch(Exception ex)
            {
                ExceptionCollector ec = new ExceptionCollector(ex);
                result = ec.ToUL();
            }

            return Content(result);
        }

        public ActionResult AssignRights(string currentName, string memberId)
        {
            string result = "ok";
            try
            {
                LdapUserConfig cfg = new LdapUserConfig(memberId, new Lcps.UI.Models.LcpsUiContext());
                DirectoryEntry de = (DirectoryEntry)cfg.LdapUser.GetUnderlyingObject();

                LdapUserConfig.GrantFullAccessToFolder(currentName, de, cfg.Member.UserName, cfg.LdapConfig.DomainPrincipalName);


            }
            catch(Exception ex)
            {
                ExceptionCollector ec = new ExceptionCollector(ex);
                result = ec.ToUL();
            }

            return Content(result);
        }
        
        public ActionResult CreateFolder(string currentName, string memberId)
        {
            string result = "ok";

            try
            {
                if (!System.IO.Directory.Exists(currentName))
                {
                    System.IO.Directory.CreateDirectory(currentName);
                    LdapUserConfig cfg = new LdapUserConfig(memberId, new Lcps.UI.Models.LcpsUiContext());
                    DirectoryEntry de = (DirectoryEntry)cfg.LdapUser.GetUnderlyingObject();
                    LdapUserConfig.GrantFullAccessToFolder(currentName, de, cfg.Member.UserName, cfg.LdapConfig.DomainPrincipalName);
                }

            }
            catch (Exception ex)
            {
                ExceptionCollector ec = new ExceptionCollector(ex);
                result = ec.ToUL();
            }

            return Content(result);
        }


    }
}