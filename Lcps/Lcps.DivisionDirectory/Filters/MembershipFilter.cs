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

using Lcps.DivisionDirectory.Members;

namespace Lcps.DivisionDirectory.Filters
{
    [Table("MembershipFilter", Schema = "DivisionDirectory")]
    public class MembershipFilter
    {
        [Key]
        public Guid MembershipFilterKey { get; set; }

        [Display(Name = "Schema")]
        public MembershipFilterSchema Schema { get; set; }

        [Display(Name = "Filter")]
        public long MembershipScope { get; set; }

        [Display(Name = "Member Classification")]
        public DirectoryMemberClass MemberClass { get; set; }

        [Display(Name = "Caption")]
        [Index("IX_MembershipFilter_Caption", IsUnique = true)]
        [Required]
        [MaxLength(128)]
        public string Caption { get; set; }

        [Display(Name = "Assignment")]
        [Required]
        [MaxLength(128)]
        public string Assignment { get; set; }

        [Display(Name = "Owner")]
        public Guid AntecedentId { get; set; }

    }
}
