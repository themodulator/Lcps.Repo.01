using System;
using System.Collections.Generic;
using System.IO;
using System.Data.SqlClient;
using System.Collections.Specialized;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lcps.DivisionDirectory.Scopes
{
    [Flags]
    public enum MembershipScopeReserved
    {
        None = 0,
        Active = 1,
        Inactive = 2,
        Student = 4,
        Staff = 8,
        Outsource = 65536 //Max Value
    }
}
