using System;
using System.Collections.Generic;
using System.IO;
using System.Data.SqlClient;
using System.Collections.Specialized;

using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace Lcps.DivisionDirectory.Filters
{
    public enum MembershipFilterSchema
    {
        None = 0,
        LdapOu = 1,
        LdapGroup = 2,
        ExclusiveFolder = 4,
        AppRole = 8,
        Contacts = 16
    }
}
