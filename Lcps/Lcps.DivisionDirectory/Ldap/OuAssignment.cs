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
    [Table("OuAssignment", Schema = "Ldap")]
    public class OuAssignment
    {
        [Key]
        public Guid OuAssignmentKey { get; set; }

        [Display(Name = "OU DN")]
        [Index("IX_OuAssignment_OuDN", IsUnique = true)]
        [MaxLength(1024)]
        public string OuDistinguishedName { get; set; }

        [Display(Name = "Applies To")]
        public long MembershipScope { get; set; }
    }
}
