using System;
using System.Collections.Generic;
using System.IO;
using System.Data.SqlClient;
using System.Collections.Specialized;

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Lcps.DivisionDirectory.Scopes
{
    public class MembershipScopeEditorItem
    {
        public System.Type MembershipScopeType { get; set; }

        public MembershipScope Scope { get; set; }

        public long Challenge { get; set; }

        public bool Checked
        {
            get
            {
                string[] names = System.Enum.GetNames(MembershipScopeType);
                (new MembershipScopeRepository (new Lcps.DivisionDirectory.)

                Enum e = (Enum)Enum.Parse(MembershipScopeType, Scope.LiteralName);
                Enum cv = (Enum)Enum.ToObject(MembershipScopeType, Challenge);

                return cv.HasFlag(e);
            }

        }
    }
}
