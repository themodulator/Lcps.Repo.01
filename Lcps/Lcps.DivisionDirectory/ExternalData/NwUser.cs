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

using Lcps.DivisionDirectory;
using Lcps.DivisionDirectory.Members;
using Lcps.DivisionDirectory.Scopes;

namespace Lcps.DivisionDirectory.ExternalData
{
    public partial class NWUser
    {
        ExternalDataContext context = new ExternalDataContext();
        DirectoryMemberRepository _memberRepo;
        MembershipScopeRepository _scopeRepo;
        List<string> _validationErrors = new List<string>();


        public DirectoryMemberRepository GetMemberRepo()
        {
            if (_memberRepo == null)
                _memberRepo = new DirectoryMemberRepository();

            return _memberRepo;
        }

        public MembershipScopeRepository ScopeRepository()
        {
            if (_scopeRepo == null)
                _scopeRepo = new MembershipScopeRepository();

            return _scopeRepo;
        }

        public string[] ValidationErrors
        {
            get { return _validationErrors.ToArray(); }
        }


        public NwUserDirectoryStatus GetDirectoryStatus()
        {
            NwUserDirectoryStatus status = NwUserDirectoryStatus.None;

            DirectoryMemberRepository repo = new DirectoryMemberRepository();

            if(!EmpType.ToLower().Equals("student"))
            {
                StaffCandidate p = context.StaffCandidates.FirstOrDefault(x => x.InternalId.Equals(SocSecNbrFormatted));
                if (p != null)
                    status = status | NwUserDirectoryStatus.ExistsInPersonnel;
            }
            else
            {
                MembershipScope gScope = this.ScopeRepository().First(x => x.NWUserCaption.ToLower().Equals(this.JobTitle.ToLower()));

                if(gScope == null)
                    _validationErrors.Add(string.Format("Missing grade scope {0}", this.JobTitle));



                MembershipScope lScope = this.ScopeRepository().First(x => x.NWUserCaption.ToLower().Equals(this.SchPerDir.ToLower()));
                if(gScope == null)
                    _validationErrors.Add(string.Format("Missing location scope {0}", this.SchPerDir));



                if(_validationErrors.Count() > 0)
                {
                    if(!status.HasFlag(NwUserDirectoryStatus.Invalid))
                        status = status | NwUserDirectoryStatus.Invalid;

                }
            }

            DirectoryMember m = repo.First(x => x.UserName.Equals(this.UserNameNW));
            if (m != null)
                status = status | NwUserDirectoryStatus.ExistsInDirectory;

            return status;
        }

        public string GetDirectoryId()
        {
            DirectoryMemberRepository repo = new DirectoryMemberRepository();

            DirectoryMember m = repo.First(x => x.UserName.Equals(this.UserNameNW));
            if (m == null)
                return Guid.Empty.ToString();
            else
                return m.Id;
        }

    }
}
