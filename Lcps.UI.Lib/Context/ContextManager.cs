using System;
using System.Collections.Generic;
using System.IO;
using System.Data.SqlClient;
using System.Collections.Specialized;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Lcps.Infrastructure;
using Lcps.DivisionDirectory;
using Lcps.DivisionDirectory.Members;
using Lcps.DivisionDirectory.Ldap;

using System.Reflection;

namespace Lcps.UI.Context
{
    public class ContextManager
    {
        #region Fields

        LcpsRepositoryContext _dbContext = new LcpsRepositoryContext();
        DivisionDirectoryContext _directoryCntxt;
        DirectoryMemberRepository _memberCntxt;
        LdapContext _ldapCntxt;

        #endregion

        #region Constructors

        public ContextManager()
        {
            _directoryCntxt = new DivisionDirectoryContext();
            _memberCntxt = new DirectoryMemberRepository();
            _ldapCntxt = new LdapContext(_dbContext);

        }

        #endregion

        #region Properties

        public DivisionDirectoryContext DirectoryContext
        {
            get
            {
                return _directoryCntxt;
            }
        }

        public DirectoryMemberRepository MemberContext
        {
            get {  return _memberCntxt; }
        }

        public LdapContext LdapContext
        {
            get { return _ldapCntxt; }
        }

        #endregion

        #region Seed

        public void SeedSqlScripts()
        {
            List<Assembly> assemblies = new List<Assembly>();

            // Get a list of assemblies referenced by the repositories in this class
            foreach(PropertyInfo p in this.GetType().GetProperties())
            {
                Assembly asm = p.PropertyType.Assembly;
                if(!assemblies.Contains(asm))
                {
                    assemblies.Add(asm);
                    string[] names = asm.GetManifestResourceNames();
                    foreach(string sql in names.Where(x => x.ToLower().EndsWith(".sql")).ToArray())
                    {
                         using (StreamReader sr = new StreamReader(asm.GetManifestResourceStream(sql)))
                         {
                             string content = sr.ReadToEnd();
                             string[] commands = content.Split(new string[] {"GO"},  StringSplitOptions.None);
                             foreach(string command in commands)
                             {
                                 _dbContext.Database.ExecuteSqlCommand(command);
                             }
                         }
                    }
                }
            }
        }

        #endregion
    }
}
