using System;
using System.Collections.Generic;
using System.IO;
using System.Data.SqlClient;
using System.Collections.Specialized;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lcps.DivisionDirectory.ExternalData
{
    [Flags]
    public enum NwUserDirectoryStatus
    {
        None = 0,
        ExistsInPersonnel = 1,
        ExistsInDirectory = 2,
        Invalid = 4
    }
}
