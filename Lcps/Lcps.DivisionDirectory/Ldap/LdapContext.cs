using System;
using System.Collections.Generic;
using System.IO;
using System.Data.SqlClient;
using System.Collections.Specialized;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

using Anvil.DataContext;

namespace Lcps.DivisionDirectory.Ldap
{
    public class LdapContext
    {
        #region Fields

        private LdapConfigRepository _configRepo;

        private GenericRepository<OuAssignment> _ouRepo;

        private GenericRepository<GroupAssignmentConfig> _groupConfigs;

        private GenericRepository<GroupAssignment> _groupAssignments;

        private PersonalFolderRepository _personalFolders;

        

        #endregion


        public LdapContext(DbContext context)
        {
            _configRepo = new LdapConfigRepository(context);
            _ouRepo = new GenericRepository<OuAssignment>(context);
            _groupConfigs = new GenericRepository<GroupAssignmentConfig>(context);
            _groupAssignments = new GenericRepository<GroupAssignment>(context);
            _personalFolders = new PersonalFolderRepository(context);
        }

        public LdapConfigRepository LdapConfigs
        {
            get { return _configRepo;  }
        }

        public GenericRepository<OuAssignment> OuAssignments
        {
            get { return _ouRepo;  }
        }

        public GenericRepository<GroupAssignmentConfig> GroupAssignmentConfigs
        { get { return _groupConfigs; } }

        public GenericRepository<GroupAssignment> GroupAssignments
        { get { return _groupAssignments; } }

        public PersonalFolderRepository PersonalFolders
        { get { return _personalFolders; } }

    }
}
