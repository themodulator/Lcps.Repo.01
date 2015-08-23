namespace Lcps.UI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OuAssignment : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Ldap.OuAssignment",
                c => new
                    {
                        OuAssignmentKey = c.Guid(nullable: false),
                        OuDistinguishedName = c.String(maxLength: 1024),
                        MembershipScope = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.OuAssignmentKey)
                .Index(t => t.OuDistinguishedName, unique: true, name: "IX_OuAssignment_OuDN");
            
        }
        
        public override void Down()
        {
            DropIndex("Ldap.OuAssignment", "IX_OuAssignment_OuDN");
            DropTable("Ldap.OuAssignment");
        }
    }
}
