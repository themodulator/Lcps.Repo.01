namespace Lcps.UI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class GroupAssignments : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Ldap.GroupAssignmentConfig",
                c => new
                    {
                        GroupAssignmentKey = c.Guid(nullable: false),
                        Caption = c.String(maxLength: 255),
                        MembershipScope = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.GroupAssignmentKey)
                .Index(t => t.Caption, unique: true, name: "IX_GroupAssignmentConfig_Caption");
            
            CreateTable(
                "Ldap.GroupAssignment",
                c => new
                    {
                        GroupAssignmentKey = c.Guid(nullable: false),
                        AssignmentKey = c.Guid(nullable: false),
                        GroupDN = c.String(nullable: false, maxLength: 1024),
                    })
                .PrimaryKey(t => t.GroupAssignmentKey)
                .ForeignKey("Ldap.GroupAssignmentConfig", t => t.AssignmentKey, cascadeDelete: true)
                .Index(t => new { t.AssignmentKey, t.GroupDN }, unique: true, name: "IX_GroupAssignment_Key");
            
        }
        
        public override void Down()
        {
            DropForeignKey("Ldap.GroupAssignment", "AssignmentKey", "Ldap.GroupAssignmentConfig");
            DropIndex("Ldap.GroupAssignment", "IX_GroupAssignment_Key");
            DropIndex("Ldap.GroupAssignmentConfig", "IX_GroupAssignmentConfig_Caption");
            DropTable("Ldap.GroupAssignment");
            DropTable("Ldap.GroupAssignmentConfig");
        }
    }
}
