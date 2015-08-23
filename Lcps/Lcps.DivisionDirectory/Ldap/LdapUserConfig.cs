using System;
using System.Collections.Generic;
using System.IO;
using System.Data.SqlClient;
using System.Collections.Specialized;

using System.Security;
using System.Security.Principal;
using System.Security.AccessControl;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Lcps.DivisionDirectory.Members;
using Lcps.DivisionDirectory.Scopes;
using Lcps.DivisionDirectory.Ldap;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;

using System.Text.RegularExpressions;

namespace Lcps.DivisionDirectory.Ldap
{
    public class LdapUserConfig
    {

        #region Fields

        LdapContext ldapContext;
        DivisionDirectoryContext dirContext;
        PrincipalContext _principalContext;
        UserPrincipal _userPrincipal;
        LdapConfig _ldapConfig;

        #endregion

        #region Constructors

        public LdapUserConfig(string id, DbContext context)
        {
            ldapContext = new LdapContext(context);
            dirContext = new DivisionDirectoryContext();

            this.Member = dirContext.DirectoryMembers.GetByID(id);

            if (this.Member == null)
                throw new Exception(string.Format("No member with ID {0} was found in the directory", id));

            this.Context = context;

            this.LdapConfig = ldapContext.LdapConfigs.First();

            List<OuAssignment> ous = ldapContext.OuAssignments.Get(x => (x.MembershipScope & this.Member.MembershipScope) == x.MembershipScope).ToList();
            if (ous.Count == 0)
                throw new Exception("There are no OU's assigned to this user filter");

            if (ous.Count() > 1)
                throw new Exception("This member applies to multiple OU Assignments. Please consider refining your filter criteria");

            OuAssignment = ous.First();

            List<GroupAssignmentConfig> cfgs = ldapContext.GroupAssignmentConfigs.Get().ToList();
            this.GroupConfigs = new List<GroupAssignmentConfig>();

            foreach (GroupAssignmentConfig cfg in cfgs)
            {
                List<DirectoryMember> members = dirContext.DirectoryMembers.GetByFilter(cfg.MembershipScope, null);
                if (members.FirstOrDefault(x => x.InternalId == Member.InternalId) != null)
                    GroupConfigs.Add(cfg);
            }


            this.PersonalFolders = new List<PersonalFolder>();
            List<PersonalFolder> fldrs = ldapContext.PersonalFolders.Get().ToList();
            foreach (PersonalFolder f in fldrs)
            {
                List<DirectoryMember> members = dirContext.DirectoryMembers.GetByFilter(f.MembershipScope, null);
                if (members.FirstOrDefault(x => x.InternalId == Member.InternalId) != null)
                    PersonalFolders.Add(f);

            }

            string pwd = ldapContext.LdapConfigs.Decryptpassword(LdapConfig);
            PrincipalContext = new PrincipalContext(ContextType.Domain, LdapConfig.DomainPrincipalName, LdapConfig.UserName, pwd);

            ExistsInLdap = GetExistsInLdap();

            if (GetExistsInLdap())
                LdapUser = UserPrincipal.FindByIdentity(PrincipalContext, IdentityType.SamAccountName, Member.UserName);

        }

        #endregion

        #region Properties

        public DbContext Context { get; set; }

        public LdapConfig LdapConfig { get; set; }

        public DirectoryMember Member { get; set; }

        public OuAssignment OuAssignment { get; set; }

        public List<GroupAssignmentConfig> GroupConfigs { get; set; }

        public List<PersonalFolder> PersonalFolders { get; set; }

        public PrincipalContext PrincipalContext { get; set; }

        public UserPrincipal LdapUser { get; set; }

        public bool ExistsInLdap { get; set; }

        public List<GroupAssignment> GroupAssignments
        {
            get
            {
                List<GroupAssignment> items = new List<GroupAssignment>();

                foreach (GroupAssignmentConfig cfg in GroupConfigs)
                {
                    items.AddRange(cfg.GroupAssignments);

                }

                return items;
            }
        }

        #endregion

        #region LdapSync

        public void SyncUser(bool allowPasswordChange)
        {
            // Does the user exist
            string filter = "(&(samAccountname=" + this.Member.UserName + "))";

            LdapConfig cfg = ldapContext.LdapConfigs.First();

            if (cfg == null)
                throw new Exception("No LDAP Configuration was found in the database.");

            string pwd = ldapContext.LdapConfigs.Decryptpassword(cfg);

            DirectoryEntry root = new DirectoryEntry("LDAP://" + cfg.DomainPrincipalName, cfg.UserName, pwd);
            DirectorySearcher searcher = new DirectorySearcher(root, filter);

            if (searcher.FindOne() == null)
                this.CreateLdapAccount(cfg, allowPasswordChange);
            else
                this.UpdateDirectoryEntry();

        }

        public void CreateLdapAccount(LdapConfig cfg, bool allowPasswordChange)
        {
            string pwd = ldapContext.LdapConfigs.Decryptpassword(cfg);
            DirectoryEntry ou = new DirectoryEntry("LDAP://" + OuAssignment.OuDistinguishedName, cfg.UserName, pwd);

            string fn = Regex.Replace(Member.GivenName, @"[^A-Za-z0-9]+", "");
            string mn = null;
            if(Member.MiddleName != null)
                mn = Regex.Replace(Member.MiddleName, @"[^A-Za-z0-9]+", "");
            string ln = Regex.Replace(Member.Surname, @"[^A-Za-z0-9]+", "");




            string name = Lcps.DivisionDirectory.Members.DirectoryMemberRepository.GetName(DirectoryMemberNameFormats.Full | DirectoryMemberNameFormats.Sort, ln, fn, mn);
            string dn = string.Format("CN={0} ({1})", name.Replace(",", "\\,"), Member.UserName);

            var principalContext = new PrincipalContext(ContextType.Domain, cfg.DomainPrincipalName, cfg.UserName, pwd);


            string memberPw = Member.DecryptPassword();

            bool enabled = false;
            if ((Member.MembershipScope & Convert.ToInt64(MembershipScopeReserved.Active)) == Convert.ToInt64(MembershipScopeReserved.Active))
                enabled = true;

            if ((Member.MembershipScope & Convert.ToInt64(MembershipScopeReserved.Inactive)) == Convert.ToInt64(MembershipScopeReserved.Inactive))
                enabled = false;


            LdapUser = new UserPrincipal(principalContext, Member.UserName, memberPw, enabled);

            string scope = this.dirContext.MembershipScopes.GetCaptionLabel(Member.MembershipScope);
            LdapUser.Description = scope;
            LdapUser.DisplayName = Lcps.DivisionDirectory.Members.DirectoryMemberRepository.GetName(Member, DirectoryMemberNameFormats.Short | DirectoryMemberNameFormats.Sort);
            LdapUser.UserCannotChangePassword = (!allowPasswordChange);
            LdapUser.Surname = Member.Surname;
            LdapUser.GivenName = Member.GivenName;
            LdapUser.UserPrincipalName = Member.UserName + "@" + cfg.DomainPrincipalName;
            LdapUser.PasswordNeverExpires = true;
            LdapUser.EmployeeId = Member.InternalId;
            LdapUser.EmailAddress = Member.Email;

            try
            {
                LdapUser.Save();
            }
            catch (Exception ex)
            {
                throw new Exception("Could not create user", ex);
            }

            ou.RefreshCache();

            DirectoryEntry de = null;

            try
            {
                de = (DirectoryEntry)LdapUser.GetUnderlyingObject();
                de.InvokeSet("division", pwd);
                de.InvokeSet("comment", DateTime.Now.ToString());
                de.MoveTo(ou, dn);
                de.CommitChanges();
                de.RefreshCache();
                ou.RefreshCache();

                LdapUser = UserPrincipal.FindByIdentity(PrincipalContext, IdentityType.SamAccountName, Member.UserName);

            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Could not move user {0} to OU", dn), ex);
            }



            SyncGroupMemberships(false);

            bool gErr = true;
            int x = 1;

            while(gErr == true)
            {
                try
                {
                    x++;

                    DirectoryEntry thisU = (DirectoryEntry)LdapUser.GetUnderlyingObject();
                    thisU.CommitChanges();
                    ou.RefreshCache();
                    thisU.RefreshCache();
                    

                    foreach(GroupPrincipal g in LdapUser.GetGroups())
                    {
                        string n = g.Name;
                    }
                    gErr = false;
                }
                catch
                {
                    LdapUser = UserPrincipal.FindByIdentity(PrincipalContext, IdentityType.SamAccountName, Member.UserName);
                    gErr = true;
                }
            }

            try
            {
                foreach (PersonalFolder f in PersonalFolders)
                {
                    string n = Member.UserName;
                    if (f.NameFormat == PersonalFolderIdFormats.Fullname)
                    {
                        FormatFolderName(Member, f);
                    }

                    string pth = Path.Combine(f.FolderPath, n);
                    if (!Directory.Exists(pth))
                        Directory.CreateDirectory(pth);

                    GrantFullAccessToFolder(pth, de, Member.UserName, cfg.DomainPrincipalName);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Could not create personal folders", ex);
            }

        }

        public static string FormatFolderName(IDirectoryMember member, PersonalFolder pf)
        {
            return FormatFolderName(member, pf.NameFormat);
        }

        public static string FormatFolderName(IDirectoryMember member, PersonalFolderIdFormats format)
        {
            if (format == PersonalFolderIdFormats.Fullname)
            {
                string name = DirectoryMemberRepository.GetName(member, DirectoryMemberNameFormats.Full | DirectoryMemberNameFormats.Sort) + " - " + member.UserName;
                return name;
            }
            else
            {
                return member.UserName;
            }
        }

        public static string GetFolderPath(IDirectoryMember member, PersonalFolder pf)
        {
            if (pf.NameFormat == PersonalFolderIdFormats.Fullname)
            {
                string name = DirectoryMemberRepository.GetName(member, DirectoryMemberNameFormats.Full | DirectoryMemberNameFormats.Sort) + " - " + member.UserName;

                return Path.Combine(pf.FolderPath, name);
            }
            else
            {
                return Path.Combine(pf.FolderPath, member.UserName);
            }
        }

        public void SyncGroupMemberships(bool? lookForCurrent)
        {
            bool doClear = (lookForCurrent == null) ? false : lookForCurrent.Value;

            try
            {
                LdapUser.GetGroups();
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Error getting group membership for {0}", LdapUser.SamAccountName), ex);
            }


            if (doClear)
            {
                try
                {
                    foreach (GroupPrincipal g in LdapUser.GetGroups())
                    {
                        if (!g.Name.ToLower().Equals("domain users"))
                        {
                            g.Members.Remove(LdapUser);
                            g.Save();
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Could not determine if user belongs to any groups", ex);
                }
            }

            foreach (GroupAssignmentConfig cfg in this.GroupConfigs)
            {
                foreach (GroupAssignment ga in cfg.GroupAssignments)
                {
                    try
                    {
                        GroupPrincipal gp = GroupPrincipal.FindByIdentity(PrincipalContext, IdentityType.DistinguishedName, ga.GroupDN);
                        if (!LdapUser.IsMemberOf(gp))
                        {
                            gp.Members.Add(LdapUser);
                            gp.Save();
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(String.Format("Error adding user to {0}", ga.GroupDN), ex);
                    }
                }
            }

            try
            {
                LdapUser.Save();
            }
            catch (Exception ex)
            {
                throw new Exception("Error comitting changes to user", ex);
            }


        }

        public static void GrantFullAccessToFolder(string folderPath, DirectoryEntry de, string userName, string principalDomain)
        {
            try
            {
                if (!System.IO.Directory.Exists(folderPath))
                    System.IO.Directory.CreateDirectory(folderPath);

                DirectoryInfo dInfo = new DirectoryInfo(folderPath);
                DirectorySecurity dSecurity = dInfo.GetAccessControl();

                byte[] sidByte = de.InvokeGet("objectSid") as byte[];
                SecurityIdentifier sid = new SecurityIdentifier(sidByte, 0);

                string id = userName + "@" + principalDomain;

                dSecurity.AddAccessRule(new FileSystemAccessRule(sid, FileSystemRights.FullControl, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
                dInfo.SetAccessControl(dSecurity);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error granting user {0} full persmissions to folder {1}", userName, folderPath), ex);
            }
        }

        public void UpdateDirectoryEntry()
        {
            SyncGroupMemberships(true);

        }

        #endregion

        #region LDAP

        private bool GetExistsInLdap()
        {
            try
            {
                string filter = "(&(samAccountname=" + this.Member.UserName + "))";

                string pwd = ldapContext.LdapConfigs.Decryptpassword(LdapConfig);

                DirectoryEntry root = new DirectoryEntry("LDAP://" + LdapConfig.DomainPrincipalName, LdapConfig.UserName, pwd);
                DirectorySearcher searcher = new DirectorySearcher(root, filter);

                if (searcher.FindOne() == null)
                    return false;
                else
                    return true;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while looking for user in LDAP", ex);
            }
        }


        public UserPrincipal GetUserPrincipal(PrincipalContext pc)
        {
            UserPrincipal u = UserPrincipal.FindByIdentity(pc, IdentityType.SamAccountName, Member.UserName);
            return u;
        }

        public List<GroupPrincipal> GetLdapGroups(PrincipalContext pc)
        {
            UserPrincipal u = UserPrincipal.FindByIdentity(pc, IdentityType.SamAccountName, Member.UserName);
            List<GroupPrincipal> groups = u.GetGroups().Cast<GroupPrincipal>().ToList();
            return groups;
        }

        #endregion
    }
}
