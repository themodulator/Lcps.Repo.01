using System;
using System.Collections.Generic;
using System.IO;
using System.Data.SqlClient;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Anvil.DataContext;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;


namespace Lcps.DivisionDirectory.Ldap
{
    public class PersonalFolderRepository : GenericRepository<PersonalFolder>
    {
        public PersonalFolderRepository(DbContext context)
            :base(context)
        {

        }

        public override void Insert(PersonalFolder entity)
        {
            if (!Directory.Exists(entity.FolderPath))
                throw new Exception("The folder does not exists");

            if (entity.MembershipScope == 0L)
                throw new Exception("Please provide a member filter");

            base.Insert(entity);
        }
    }
}
