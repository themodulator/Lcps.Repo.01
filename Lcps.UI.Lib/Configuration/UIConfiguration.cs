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

namespace Lcps.UI.Configuration
{
    [Table("UIConfiguration", Schema = "Lcps.UI")]
    public class UIConfiguration
    {
        [Key]
        public Guid UIConfigurationKey { get; set; }

        [Display(Name = "Bootstrap Theme")]
        public string DefaultTheme { get; set; }
    }
}
