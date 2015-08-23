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

using System.Xml.Serialization;

using Lcps.Infrastructure;

namespace Lcps.DivisionDirectory.Members
{
    public class DirectoryMemberRepository : GenericRepository<DirectoryMember>
    {

        #region Fields

        MembershipScopeRepository _scopeRepo;

        #endregion

        #region Constructors
      

        public DirectoryMemberRepository()
            : base(new Lcps.Infrastructure.LcpsRepositoryContext())
        {
            DbContext = new Lcps.Infrastructure.LcpsRepositoryContext();
            AccountManager = new LcpsAccountManager();
        }

        #endregion

        #region Properties

        public LcpsAccountManager AccountManager { get; set; }

        public Lcps.Infrastructure.LcpsRepositoryContext DbContext { get; set; }

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

        #region Get

        public List<DirectoryMember> GetByFilter(long filter, string search)
        {

            MembershipScopeRepository scopeRepository = new MembershipScopeRepository();

            System.Type t = scopeRepository.ScopeEnumType;

            Enum fv = (Enum)System.Enum.ToObject(t, filter);
            string caption = fv.ToString();

            List<DirectoryMember> source = new List<DirectoryMember>();

            if (!String.IsNullOrEmpty(search))
                source = Get(x => (x.Surname.ToLower().Contains(search.ToLower()) |
                     x.GivenName.ToLower().Contains(search.ToLower()) |
                     x.InternalId.ToLower().Contains(search.ToLower()) |
                     x.UserName.ToLower().Contains(search.ToLower()))).ToList();

            if (!String.IsNullOrEmpty(search))
                source = Get(x =>
                    (x.Surname.ToLower().Contains(search.ToLower()) |
                     x.GivenName.ToLower().Contains(search.ToLower()) |
                     x.InternalId.ToLower().Contains(search.ToLower()) |
                     x.UserName.ToLower().Contains(search.ToLower()))).ToList();

            if (String.IsNullOrEmpty(search))
                source = Get().ToList();



            if (filter > 0)
            {

                foreach (Enum e in Enum.GetValues(typeof(MembershipScopeQualifier)))
                {
                    List<MembershipScope> ss = scopeRepository.GetApplicableScopes(filter).Where(x => x.ScopeQualifier == (MembershipScopeQualifier)e).ToList();
                    if (ss.Count() > 0)
                        source = GetMembers(ss, source);
                }
            }

            return source.OrderBy(x => x.Surname + x.GivenName).ToList();

        }

        private static List<DirectoryMember> GetMembers(List<MembershipScope> scopes, List<DirectoryMember> source)
        {


            List<DirectoryMember> members = new List<DirectoryMember>();

            foreach (MembershipScope s in scopes)
            {
                members.AddRange(source.Where(x => (x.MembershipScope & s.BinaryValue) == s.BinaryValue).ToList());
            }

            return members;
        }


        

        #endregion

        #region Scopes

        public MembershipScopeRepository GetScopeRepository()
        {
            if (_scopeRepo == null)
                _scopeRepo = new MembershipScopeRepository();

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

        #region Name


        public static string GetName(IDirectoryMember member, DirectoryMemberNameFormats format)
        {
            return GetName(format, member.Surname, member.GivenName, member.MiddleName);
        }

        public static string GetName(DirectoryMemberNameFormats format, string surName, string givenName, string middleName)
        {
            string m = (middleName == null ? "" : middleName);

            if (format.HasFlag(DirectoryMemberNameFormats.Full) & !format.HasFlag(DirectoryMemberNameFormats.Sort))
            {
                if (middleName == null)
                    return givenName + " " + surName;
                else
                    return givenName + " " + middleName + " " + surName;
            }

            if (format.HasFlag(DirectoryMemberNameFormats.Short) & !format.HasFlag(DirectoryMemberNameFormats.Sort))
                return givenName + " " + surName;

            if (format.HasFlag(DirectoryMemberNameFormats.Full) & format.HasFlag(DirectoryMemberNameFormats.Sort))
            {
                if (middleName == null)
                    return surName + ", " + givenName;
                else
                    return surName + ", " + givenName + " " + middleName;
            }

            if (format.HasFlag(DirectoryMemberNameFormats.Short) & format.HasFlag(DirectoryMemberNameFormats.Sort))
                return surName + ", " + givenName;

            return "";
        }

        #endregion

        #region Export

        public static byte[] SerializeMembers(IEnumerable<IDirectoryMember> members)
        {
            List<DirectoryMemberExport> export = new List<DirectoryMemberExport>();
            foreach (IDirectoryMember m in members)
            {
                export.Add(new DirectoryMemberExport(m));
            }

            XmlSerializer x = new XmlSerializer(export.GetType());

            MemoryStream ms = new System.IO.MemoryStream();

            x.Serialize(ms, export);

            return ms.ToArray();
        }

        #endregion
    }
}
