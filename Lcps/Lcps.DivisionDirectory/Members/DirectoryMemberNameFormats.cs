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
    public enum DirectoryMemberNameFormats
    {
        None = 0,
        Full = 1, 
        Short = 2,
        Sort = 4
    }
}
