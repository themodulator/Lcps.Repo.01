using System;
using System.Collections.Generic;
using System.IO;
using System.Data.SqlClient;
using System.Collections.Specialized;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lcps.DivisionDirectory.Ldap
{
    [Table("LdapConfig", Schema = "Ldap")]
    public class LdapConfig
    {
        [Key]
        public Guid LdapConfigKey { get; set; }

        [Display(Name = "Domain")]
        [Index("IX_ConfigDomain_DomainName", IsUnique = true)]
        [MaxLength(256)]
        public string DomainPrincipalName { get; set; }

        [Display(Name = "User")]
        public string UserName { get; set; }

        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
