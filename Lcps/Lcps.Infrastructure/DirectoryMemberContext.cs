using System;
using System.Collections.Generic;
using System.IO;
using System.Data.SqlClient;
using System.Collections.Specialized;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Anvil;
using Anvil.DataContext;

using Lcps.DivisionDirectory.Members;
using Lcps.DivisionDirectory.Scopes;
using Lcps.DivisionDirectory.Filters;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Lcps.Infrastructure
{
    public class DirectoryMemberContext : GenericRepository<DirectoryMember>
    {

        #region Fields

        MembershipScopeRepository _scopeRepo;

        #endregion

        #region Constructors
      

        public DirectoryMemberContext(DbContext context)
            :base(context)
        {
            DbContext = context;
            AccountManager = new LcpsAccountManager();
        }

        #endregion

        #region Properties

        public LcpsAccountManager AccountManager { get; set; }

        public DbContext DbContext { get; set; }

        #endregion

      

        public override void Insert(DirectoryMember entity)
        {
            if (entity.Id == Guid.Empty.ToString())
                throw new Exception("The User Id cannot be null");

            if(entity.InitialPassword != entity.ConfirmPassword)
                throw new Exception("The passwords do not match");

            string pwd = entity.InitialPassword;
            entity.ConfirmPassword = null;

            Anvil.RijndaelEnhanced enc = new RijndaelEnhanced(DirectoryMember.GuidKey);
            entity.InitialPassword = enc.Encrypt(pwd);

            try
            {
                IdentityResult  r = AccountManager.Create(entity, pwd);
                if(!r.Succeeded)
                {
                    ExceptionCollector ec = new ExceptionCollector();
                    ec.AddRange(r.Errors.ToArray());
                    throw ec.ToException();
                }
            }
            catch(Exception ex)
            {
                ExceptionCollector ec = new ExceptionCollector(ex);
                throw ec.ToException();
            }
        }

        public override void Update(DirectoryMember entityToUpdate)
        {
            base.Update(entityToUpdate);
        }

        public override void Delete(DirectoryMember entityToDelete)
        {
           base.Delete(entityToDelete);
        }

        public override void Delete(object id)
        {
            DirectoryMember m = GetByID(id);
            Delete(m);
        }

        #region Filter

        

        

        #endregion

        #region Scopes

        public MembershipScopeRepository GetScopeRepository()
        {
            if (_scopeRepo == null)
                _scopeRepo = new MembershipScopeRepository(new LcpsRepositoryContext());

            return _scopeRepo;
        }

        public MvcHtmlString ScopeULByQualifier<T>(T item, MembershipScopeQualifier q)
        {
            IDirectoryMember m = item as IDirectoryMember;

            if (item == null)
                throw new Exception("Item must implement IDirectoryMember");

            MembershipScopeRepository r = GetScopeRepository();

            List<MembershipScope> ss = r.GetApplicableScopes(m.MembershipScope);

            TagBuilder ul = new TagBuilder("ul");
            foreach (MembershipScope s in ss)
            {
                TagBuilder li = new TagBuilder("li");
                li.SetInnerText(s.Caption);
                ul.InnerHtml += li.ToString();
            }

            return MvcHtmlString.Create(ul.ToString());
        }
        #endregion

        

        #region Scopes

        public string GetScopeCaption(IDirectoryMember member)
        {
            MembershipScopeRepository r = GetScopeRepository();
            return r.GetCaptionLabel(member.MembershipScope);

        }

        #endregion
    }
}
