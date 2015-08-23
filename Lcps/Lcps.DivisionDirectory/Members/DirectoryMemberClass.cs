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
    public enum DirectoryMemberClass
    {
        Inactive = 0,
        External = 1,
        Staff = 2,
        Student = 4
    }
}
