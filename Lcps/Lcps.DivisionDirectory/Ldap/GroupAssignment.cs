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
    [Table("GroupAssignment", Schema = "Ldap")]
    public class GroupAssignment
    {
        [Key]
        public Guid GroupAssignmentKey { get; set; }

        [Display(Name = "Config Key")]
        [ForeignKey("GroupAssignmentConfig")]
        [Required]
        public Guid GroupConfigKey { get; set; }

        public virtual GroupAssignmentConfig GroupAssignmentConfig { get; set; }

        [Display(Name = "Group DN")]
        [Required]
        public string GroupDN { get; set; }

    }
}
