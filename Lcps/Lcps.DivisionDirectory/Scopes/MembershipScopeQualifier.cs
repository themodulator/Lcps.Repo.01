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
    public enum MembershipScopeQualifier
    {
        None = 0,
        Reserved = 1,
        Location = 2,
        Type = 4,
        Position = 8,
        Grade = 16,
        Department = 32,
        Role = 64
        
    }
}
