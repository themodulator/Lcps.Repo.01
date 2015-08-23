using System;
using System.Collections.Generic;
using System.IO;
using System.Data.SqlClient;
using System.Collections.Specialized;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

using System.Reflection;

using Lcps.DivisionDirectory.Members;
using Lcps.DivisionDirectory.Scopes;

namespace Lcps.DivisionDirectory.ExternalData
{
    public class NwUserCandidate : NWUser, IDirectoryMemberCandidate
    {
        #region Constants

        public const string STUDENT = "STUDENT";
        public const string NullFieldExpression = "{0} is a required field";
        public const string InvalidTypeExpression = "{0} = {1} is not a valid {2}";
        public const string ZeroScopeTypeExpression = "{0} = {1} does not have membership scope mapped";

        #endregion

        #region Fields


        #endregion

        #region Constructors

        public NwUserCandidate(NWUser user)
        {
            this.Context = new Lcps.Infrastructure.LcpsRepositoryContext();
            this.DirectoryContext = new DivisionDirectoryContext();
            ExternalContext = new ExternalDataContext();
            NWUser = user;

            foreach (PropertyInfo p in user.GetType().GetProperties())
            {
                if (p.CanRead & p.CanWrite)
                {
                    object v = p.GetValue(user, null);
                    p.SetValue(this, v);
                }
            }

            if (EmpType != STUDENT)
            {
                this.IsStudent = false;
                this.Personnel = ExternalContext.Personnels.FirstOrDefault(x => x.SSN.Equals(SocSecNbrFormatted));
                this.ExistsInPersonnel = (this.Personnel != null);
                this.Member = DirectoryContext.DirectoryMembers.First(x => x.InternalId.Equals(this.SocSecNbrFormatted));
                ExistsInDirectory = (Member != null);
                IsActive = (UserStatus == null);
            }
            else
            {
                this.IsStudent = true;
                this.ExistsInPersonnel = false;
                this.Member = DirectoryContext.DirectoryMembers.First(x => x.InternalId.Equals(this.EntityID));
                IsActive = (ExistsInPersonnel) ? Personnel.Active : false;
            }

            this.ValidationErrors = new List<string>();

            long scope = 0L;

            if (IsActive)
                scope += (long)MembershipScopeReserved.Active;
            else
                scope += (long)MembershipScopeReserved.Inactive;

            if (EmpType == STUDENT)
            {
                scope += (long)MembershipScopeReserved.Staff;

                Lcps.DivisionDirectory.Scopes.MembershipScope locationScope = DirectoryContext.MembershipScopes.First(x => x.NWUserCaption == user.SchPerDir);
                Lcps.DivisionDirectory.Scopes.MembershipScope gradeScope = DirectoryContext.MembershipScopes.First(x => x.NWUserCaption == user.JobTitle);

                this.LocationScope = ((locationScope == null) ? 0L : locationScope.BinaryValue);
                this.GradeScope = ((gradeScope == null) ? 0L : gradeScope.BinaryValue);

                if (this.LocationScope == 0L)
                    ValidationErrors.Add(string.Format(ZeroScopeTypeExpression, "SchPerDir", this.SchPerDir));

                if (this.GradeScope == 0L)
                    ValidationErrors.Add(string.Format(ZeroScopeTypeExpression, "JobTitle", this.JobTitle));


                MembershipScope = this.LocationScope + this.GradeScope + scope;

                InternalId = user.EntityID;
                GivenName = FN;
                MiddleName = MN;
                Surname = user.LN;
                try
                {
                    DOB = DateTime.Parse(user.BirthDateStandard);
                }
                catch
                {
                    DOB = DateTime.MaxValue;
                }
                Gender = (user.Gender == "M") ? DirectoryMemberGender.Male : DirectoryMemberGender.Female;


            }
            else
            {
                InternalId = user.SocSecNbrFormatted;

                scope += (long)MembershipScopeReserved.Staff;

                if (ExistsInPersonnel)
                {
                    Lcps.DivisionDirectory.Scopes.MembershipScope locationScope = DirectoryContext.MembershipScopes.First(x => x.PersonnelCaption == Personnel.Location1);
                    Lcps.DivisionDirectory.Scopes.MembershipScope positionScope = DirectoryContext.MembershipScopes.First(x => x.PersonnelCaption == Personnel.Position);
                    Lcps.DivisionDirectory.Scopes.MembershipScope titleScope = DirectoryContext.MembershipScopes.First(x => x.PersonnelCaption == Personnel.Assignment);

                    this.LocationScope = ((locationScope == null) ? 0L : locationScope.BinaryValue);
                    this.PositionScope = ((positionScope == null) ? 0L : positionScope.BinaryValue);
                    this.TitleScope = ((titleScope == null) ? 0L : titleScope.BinaryValue);

                    if (this.LocationScope == 0L)
                        ValidationErrors.Add(string.Format(ZeroScopeTypeExpression, "Location1", Personnel.Location1));

                    if (this.PositionScope == 0L)
                        ValidationErrors.Add(string.Format(ZeroScopeTypeExpression, "Position", Personnel.Position));


                    MembershipScope = this.LocationScope + this.PositionScope + this.TitleScope + scope;

                    GivenName = Personnel.First_Name;
                    MiddleName = Personnel.Middle_Name;
                    Surname = Personnel.Last_Name;
                    DOB = (Personnel.DOB == null) ? DateTime.MaxValue : Personnel.DOB.Value;

                    Gender = (Personnel.Sex == "M") ? DirectoryMemberGender.Male : DirectoryMemberGender.Female;
                }
            }

            UserName = user.UserNameNW;
            InitialPassword = user.Password;
            Email = user.EmailLcps;

            string[] requiredFields = new string[] { "InternalId", "GivenName", "Surname", "UserName", "InitialPassword", "Email" };



            foreach (string rf in requiredFields)
            {
                if (this.GetType().GetProperty(rf).GetValue(this, null) == null)
                {
                    ValidationErrors.Add(string.Format(NullFieldExpression, rf));
                }
            }

            if (this.DOB.Equals(DateTime.MaxValue))
            {
                if (IsStudent)
                    ValidationErrors.Add(string.Format(InvalidTypeExpression, "DOB", user.BirthDateStandard, typeof(DateTime).Name));
                else
                    if (ExistsInPersonnel)
                        ValidationErrors.Add(string.Format(InvalidTypeExpression, "DOB", (Personnel.DOB == null) ? "" : Personnel.DOB.ToString(), typeof(DateTime).Name));
            }

            FullSortName = DirectoryMemberRepository.GetName(DirectoryMemberNameFormats.Full | DirectoryMemberNameFormats.Sort, this.Surname, this.GivenName, this.MiddleName);

            BootStrapStatus = BootStrapStatusName.@default;

            if (ValidationErrors.Count > 0)
                BootStrapStatus = BootStrapStatusName.danger;

            if (ValidationErrors.Count == 0)
            {
                if (IsStudent)
                {
                    if (!ExistsInDirectory) BootStrapStatus = BootStrapStatusName.success;
                }
                else
                {
                    if (!ExistsInPersonnel)
                        BootStrapStatus = BootStrapStatusName.warning;
                    else
                    {
                        if (!ExistsInDirectory)
                            BootStrapStatus = BootStrapStatusName.success;
                        else
                            BootStrapStatus = BootStrapStatusName.@default;
                    }
                }
            }
        }

        #endregion

        #region Personnel

        public Personnel Personnel { get; set; }

        public DirectoryMember Member { get; set; }

        public NWUser NWUser { get; set; }

        #endregion

        public DbContext Context { get; set; }

        public ExternalDataContext ExternalContext { get; set; }

        public DivisionDirectoryContext DirectoryContext { get; set; }

        #region Directory Member Candidate Properties

        public List<string> ValidationErrors { get; set; }

        public bool ExistsInPersonnel { get; set; }

        public bool ExistsInDirectory { get; set; }

        public bool IsStudent { get; set; }

        public bool IsActive { get; set; }

        public long LocationScope { get; set; }

        public long PositionScope { get; set; }

        public long TitleScope { get; set; }

        public long GradeScope { get; set; }

        public BootStrapStatusName BootStrapStatus { get; set; }

        public string FullSortName { get; set; }

        #endregion

        #region Directory Properties

        public string InternalId { get; set; }

        public string GivenName { get; set; }

        public string MiddleName { get; set; }

        public string Surname { get; set; }

        public DateTime DOB { get; set; }

        public DirectoryMemberGender Gender { get; set; }

        public long MembershipScope { get; set; }

        public string Title { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public string InitialPassword { get; set; }

        #endregion

        public DirectoryMember AddToDirectory()
        {
            try
            {
                DivisionDirectoryContext context = new DivisionDirectoryContext();

                DirectoryMember m = new DirectoryMember(this);
                context.DirectoryMembers.Insert(m);
                return m;
            }
            catch(Exception ex)
            {
                throw new Exception("Could not add canidate tp directory", ex);
            }
        }
    }
}
