using System;
using System.Collections.Generic;
using System.IO;
using System.Data.SqlClient;
using System.Collections.Specialized;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Anvil.DataContext;
using Lcps.DivisionDirectory.Members;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

using System.DirectoryServices;

namespace Lcps.DivisionDirectory.Ldap
{
    public class LdapConfigRepository : GenericRepository<LdapConfig>
    {
        public const string GuidKey = "775D1488-E996-4039-A94A-3FFD32C303D2";

        public LdapConfigRepository(DbContext context)
            :base(context)
        {}

        public override void Insert(Lcps.DivisionDirectory.Ldap.LdapConfig entity)
        {
            entity.LdapConfigKey = Guid.NewGuid();

            Validate(entity);

            base.Insert(entity);
        }

        public override void Update(LdapConfig entityToUpdate)
        {
            Validate(entityToUpdate);

            base.Update(entityToUpdate);
        }

        private LdapConfig Validate(LdapConfig entity)
        {
            try
            {
                DirectoryEntry de = new DirectoryEntry("LDAP://" + entity.DomainPrincipalName, entity.UserName, entity.Password);
                string dn = de.InvokeGet("dc").ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("Could not connect to domain", ex);
            }

            Anvil.RijndaelEnhanced r = new Anvil.RijndaelEnhanced(GuidKey);
            string pwd = r.Encrypt(entity.Password);
            entity.Password = pwd;

            return entity;

        }

        public string Decryptpassword(LdapConfig config)
        {
            Anvil.RijndaelEnhanced r = new Anvil.RijndaelEnhanced(GuidKey);
            string pwd = r.Decrypt(config.Password);
            return pwd;
        }
    }
}
