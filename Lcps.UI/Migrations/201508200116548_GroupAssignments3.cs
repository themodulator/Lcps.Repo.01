namespace Lcps.UI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class GroupAssignments3 : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "Ldap.GroupAssignment", name: "AssignmentKey", newName: "GroupConfigKey");
            RenameIndex(table: "Ldap.GroupAssignment", name: "IX_AssignmentKey", newName: "IX_GroupConfigKey");
        }
        
        public override void Down()
        {
            RenameIndex(table: "Ldap.GroupAssignment", name: "IX_GroupConfigKey", newName: "IX_AssignmentKey");
            RenameColumn(table: "Ldap.GroupAssignment", name: "GroupConfigKey", newName: "AssignmentKey");
        }
    }
}
