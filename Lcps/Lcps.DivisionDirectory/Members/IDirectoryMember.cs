using System;
using System.Collections.Generic;
using System.IO;
using System.Data.SqlClient;
using System.Collections.Specialized;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lcps.DivisionDirectory.Members
{
    public interface IDirectoryMember
    {


        string InternalId { get; set; }

        string GivenName { get; set; }

        string MiddleName { get; set; }

        string Surname { get; set; }

        DateTime DOB { get; set; }

        DirectoryMemberGender Gender { get; set; }

        long MembershipScope { get; set; }

        string Title { get; set; }

        string UserName { get; set; }

        string Email { get; set; }

        string InitialPassword { get; set; }

    }
}
