using System;
using System.Collections.Generic;
using System.IO;
using System.Data.SqlClient;
using System.Collections.Specialized;

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using System.Web.Mvc;
using System.Web.Mvc.Html;

using System.Reflection;

using Anvil;

using Lcps.DivisionDirectory.Scopes;

namespace Lcps.DivisionDirectory.Members
{
    [Table("DirectoryMember", Schema = "DivisionDirectory")]
    public class DirectoryMember : IdentityUser, IDirectoryMember
    {
        #region Constants

        public const string GuidKey = "6B32BFFF-7A74-4036-AEA0-EE6A27E88FC2";

        #endregion

        #region Constructors

        public DirectoryMember()
        { }

        public DirectoryMember(IDirectoryMember directoryMember)
        {
            foreach (PropertyInfo p in typeof(IDirectoryMember).GetProperties())
            {
                object v = null;

                try
                {
                    v = p.GetValue(directoryMember, null);
                }
                catch(Exception ex)
                {
                    throw new Exception(String.Format("Could not get value for property {0}", p.Name), ex);
                }

                try
                {
                    p.SetValue(this, v, null);
                }
                catch(Exception ex)
                {
                    throw new Exception(String.Format("Could not set value for property {0}", p.Name), ex);
                }
            }
        }

        #endregion


        #region Properties

        [Display(Name = "Division ID")]
        [MaxLength(128)]
        [Required(ErrorMessage = "ID is required")]
        [Index("IX_User_InternalId", IsUnique = true)]
        public string InternalId { get; set; }

        [Index("IX_User_ID", IsUnique = true, Order = 1)]
        [Required(ErrorMessage = "First name is required")]
        [MaxLength(75)]
        [Display(Name = "First")]
        public string GivenName { get; set; }

        [Index("IX_User_ID", IsUnique = true, Order = 2)]
        [MaxLength(75)]
        [Display(Name = "Middle")]
        public string MiddleName { get; set; }

        [Index("IX_User_ID", IsUnique = true, Order = 3)]
        [MaxLength(75)]
        [Required(ErrorMessage = "Last name is required")]
        public string Surname { get; set; }

        [Index("IX_User_ID", IsUnique = true, Order = 4)]
        [DataType(DataType.Date)]
        [Required]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DOB { get; set; }

        [Required]
        public DirectoryMemberGender Gender { get; set; }

        [Index("IX_MembershipScope", IsUnique = false)]
        [Display(Name = "Scope")]
        public long MembershipScope { get; set; }

        [Display(Name = "Title")]
        [MaxLength(256)]
        [Required(ErrorMessage = "Job title is required")]
        public string Title { get; set; }

        [MaxLength(256)]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        [Required]
        public string InitialPassword { get; set; }

        [MaxLength(256)]
        [Display(Name = "Confirm")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Name")]
        public string FullName
        {
            get 
            {
                return DirectoryMemberRepository.GetName(this, DirectoryMemberNameFormats.Full);
            }
        }

        [Display(Name = "Name")]
        public string FullSortName
        {
            get
            {
                return DirectoryMemberRepository.GetName(this, DirectoryMemberNameFormats.Sort | DirectoryMemberNameFormats.Full);
            }
        }

        #endregion

        #region User Identity

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<DirectoryMember> manager, string authenticationType)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            // Add custom user claims here
            return userIdentity;
        }

        #endregion

        #region Password

        public string DecryptPassword()
        {
            try
            {
                Anvil.RijndaelEnhanced enc = new Anvil.RijndaelEnhanced(GuidKey);
                string pwd = enc.Decrypt(this.InitialPassword);
                return pwd;
            }
            catch (Exception ex)
            {
                ExceptionCollector ec = new ExceptionCollector(ex);
                return ec.ToUL();

            }
        }

        #endregion

        #region Scope

        public List<MembershipScope> Scopes(DbContext context)
        {
            MembershipScopeRepository r = new MembershipScopeRepository();
            return r.GetApplicableScopes(this.MembershipScope);
        }

        public string ScopeCaption(DbContext context)
        {
            MembershipScopeRepository r = new MembershipScopeRepository();
            return r.GetCaptionLabel(this.MembershipScope);
        }



        public List<MembershipScope> ComparableScopes(DbContext context)
        {
            return Scopes(context).Where(x => x.ScopeQualifier == MembershipScopeQualifier.Location |
                x.ScopeQualifier == MembershipScopeQualifier.Position |
                x.ScopeQualifier == MembershipScopeQualifier.Type |
                x.ScopeQualifier == MembershipScopeQualifier.Grade |
                x.ScopeQualifier == MembershipScopeQualifier.Reserved).ToList();
        }

        public MembershipScopeQualifier ScopeQualifiers(DbContext context)
        {
            MembershipScopeQualifier q = MembershipScopeQualifier.None;

            List<MembershipScope> ss = Scopes(context);
            
            foreach(MembershipScope s in ss)
            
            {
                if (!q.HasFlag(s.ScopeQualifier))
                    q = q | s.ScopeQualifier;
            }
            
            return q;
        }


        public List<MembershipScope> ScopesByQualifier(List<MembershipScope> scopes, MembershipScopeQualifier q)
        {
            return scopes.Where(x => x.ScopeQualifier == q).ToList();
        }

        public MvcHtmlString ScopesByQualifierUL(MembershipScopeQualifier q, DbContext context)
        {
            List<MembershipScope> scopes = Scopes(context);
            scopes = ScopesByQualifier(scopes, q);

            TagBuilder ul = new TagBuilder("ul");

            foreach(MembershipScope s in scopes)
            {
                TagBuilder li = new TagBuilder("li");
                li.SetInnerText(s.Caption);
                ul.InnerHtml += li.ToString();
            }

            return MvcHtmlString.Create(ul.ToString());
        }

        #endregion
    }
}
