namespace Lcps.UI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NwUser : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "ExternalData.NwUser",
                c => new
                    {
                        NwUserKey = c.Guid(nullable: false, identity: true),
                        EntityId = c.String(),
                        LN = c.String(),
                        FN = c.String(),
                        MN = c.String(),
                        SocSecNbrFormatted = c.String(),
                        BirthDateStandard = c.DateTime(nullable: false),
                        Gender = c.String(),
                        SchPerDir = c.String(),
                        EmpType = c.String(),
                        JobTitle = c.String(),
                        UserStatus = c.String(),
                        Year = c.String(),
                        UserNameNW = c.String(),
                        EmailLcps = c.String(),
                        Password = c.String(),
                    })
                .PrimaryKey(t => t.NwUserKey);
            
            AlterColumn("dbo.AspNetUsers", "Title", c => c.String(nullable: false, maxLength: 256));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.AspNetUsers", "Title", c => c.String(maxLength: 256));
            DropTable("ExternalData.NwUser");
        }
    }
}
