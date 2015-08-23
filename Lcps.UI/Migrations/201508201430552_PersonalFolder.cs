namespace Lcps.UI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PersonalFolder : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Ldap.PersonalFolder",
                c => new
                    {
                        PersonalFolderKey = c.Guid(nullable: false),
                        FolderPath = c.String(),
                        MembershipScope = c.Long(nullable: false),
                        NameFormat = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.PersonalFolderKey);
            
        }
        
        public override void Down()
        {
            DropTable("Ldap.PersonalFolder");
        }
    }
}
