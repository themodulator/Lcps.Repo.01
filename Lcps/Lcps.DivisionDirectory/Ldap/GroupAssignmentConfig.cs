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
    [Table("GroupAssignmentConfig", Schema = "Ldap")]
    public class GroupAssignmentConfig
    {
        [Key]
        public Guid GroupAssignmentKey { get; set; }

        [Display(Name = "Title")]
        [Index("IX_GroupAssignmentConfig_Caption", IsUnique = true)]
        [MaxLength(255)]
        public string Caption { get; set; }

        [Display(Name = "Applies To")]
        public long MembershipScope { get; set; }

        public virtual ICollection<GroupAssignment> GroupAssignments { get; set; }
    }
}
