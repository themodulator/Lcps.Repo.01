using System;
using System.Collections.Generic;
using System.IO;
using System.Data.SqlClient;
using System.Collections.Specialized;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Lcps.DivisionDirectory.Members;

namespace Lcps.DivisionDirectory.ExternalData
{
    public interface IDirectoryMemberCandidate : IDirectoryMember
    {
        List<string> ValidationErrors { get; set; }

        bool ExistsInPersonnel { get; set; }

        bool ExistsInDirectory { get; set; }

        bool IsStudent { get; set; }

        bool IsActive { get; set; }

        long LocationScope { get; set; }

        long PositionScope { get; set; }

        long TitleScope { get; set; }

        BootStrapStatusName BootStrapStatus { get; set; }

        long GradeScope { get; set; }

        string FullSortName { get; set; }

    }
}
