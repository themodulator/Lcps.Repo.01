using System;
using System.Collections.Generic;
using System.IO;
using System.Data.SqlClient;
using System.Collections.Specialized;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lcps.DivisionDirectory
{
    public enum BootStrapStatusName
    {
        @default = 0,
        success = 1,
        info = 2,
        warning = 4,
        danger = 8
    }
}
