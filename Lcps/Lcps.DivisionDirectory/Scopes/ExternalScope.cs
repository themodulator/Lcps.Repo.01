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

namespace Lcps.DivisionDirectory.Scopes
{
    [Table("ExternalScope", Schema = "DivisionDirectory")]
    public class ExternalScope
    {
        [Key]
        public Guid ExternalScopeKey { get; set; }

        public ExternalScopeProvider Provider { get; set; }

        public string ExternalCaption { get; set; }

        public string InternalCaption { get; set; }


    }
}
