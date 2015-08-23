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
    [Table("PersonalFolder", Schema = "Ldap")]
    public class PersonalFolder
    {
        [Key]
        public Guid PersonalFolderKey { get; set; }

        [Display(Name = "Folder Path")]
        public string FolderPath { get; set; }

        [Display(Name = "Applies To")]
        public long MembershipScope { get; set; }

        [Display(Name = "Folder Name")]
        public PersonalFolderIdFormats NameFormat { get; set; }
    }
}
