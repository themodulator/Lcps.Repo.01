using System;
using Lcps.DivisionDirectory.Members;


namespace Lcps.DivisionDirectory.ExternalData
{
    
    
    
    

    partial class LdapStaffCandidate : IDirectoryMember
    {


        public DirectoryMemberGender Gender
        {
            get { return (DirectoryMemberGender)GenderValue; }
            set { GenderValue = (int)value; }
        }
    }

    public partial class StaffCandidate : IDirectoryMember
    {


        public DirectoryMemberGender Gender
        {
            get { return (DirectoryMemberGender)Enum.Parse(typeof(DirectoryMemberGender), GenderValue); }
            set { GenderValue = value.ToString(); }
        }

    }
}
