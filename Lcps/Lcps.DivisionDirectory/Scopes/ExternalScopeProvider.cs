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
    public enum ExternalScopeProvider
    {
        None = 0,
        NwUser = 1,
        Personnel = 2
    }
}
