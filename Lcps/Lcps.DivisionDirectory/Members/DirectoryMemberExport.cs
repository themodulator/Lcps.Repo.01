using System;
using System.Collections.Generic;
using System.IO;
using System.Data.SqlClient;
using System.Collections.Specialized;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Reflection;

namespace Lcps.DivisionDirectory.Members
{
    public class DirectoryMemberExport : IDirectoryMember
    {
        public DirectoryMemberExport()
        { }

        public DirectoryMemberExport(IDirectoryMember member)
        {
            foreach (PropertyInfo p in typeof(IDirectoryMember).GetProperties())
            {
                object v = null;

                try
                {
                    v = p.GetValue(member, null);
                }
                catch (Exception ex)
                {
                    throw new Exception(String.Format("Could not get value for property {0}", p.Name), ex);
                }

                try
                {
                    p.SetValue(this, v, null);
                }
                catch (Exception ex)
                {
                    throw new Exception(String.Format("Could not set value for property {0}", p.Name), ex);
                }
            }
        }

        public string InternalId { get; set; }

        public string GivenName { get; set; }

        public string MiddleName { get; set; }

        public string Surname { get; set; }

        public DateTime DOB { get; set; }

        public DirectoryMemberGender Gender { get; set; }

        public long MembershipScope { get; set; }

        public string Title { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public string InitialPassword { get; set; }
    }
}
