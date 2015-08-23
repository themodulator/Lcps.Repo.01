using System;
using System.Collections.Generic;
using System.IO;
using System.Data.SqlClient;
using System.Collections.Specialized;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Lcps.DivisionDirectory.ExternalData;
using Lcps.DivisionDirectory.Ldap;
using Lcps.DivisionDirectory.Members;


namespace Lcps.DivisionDirectory.ExternalData
{
    public partial class StudentCandidate : IDirectoryMember
    {
        public DirectoryMemberGender Gender
        {
            get
            {
                return (DirectoryMemberGender)System.Enum.Parse(typeof(DirectoryMemberGender), _GenderValue);
            }
            set
            {
                this._GenderValue = value.ToString();
            }
        }
    }
}
