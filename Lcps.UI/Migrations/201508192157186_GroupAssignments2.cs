namespace Lcps.UI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class GroupAssignments2 : DbMigration
    {
        public override void Up()
        {
            DropIndex("Ldap.GroupAssignment", "IX_GroupAssignment_Key");
            AlterColumn("Ldap.GroupAssignment", "GroupDN", c => c.String(nullable: false));
            CreateIndex("Ldap.GroupAssignment", "AssignmentKey");
        }
        
        public override void Down()
        {
            DropIndex("Ldap.GroupAssignment", new[] { "AssignmentKey" });
            AlterColumn("Ldap.GroupAssignment", "GroupDN", c => c.String(nullable: false, maxLength: 1024));
            CreateIndex("Ldap.GroupAssignment", new[] { "AssignmentKey", "GroupDN" }, unique: true, name: "IX_GroupAssignment_Key");
        }
    }
}
