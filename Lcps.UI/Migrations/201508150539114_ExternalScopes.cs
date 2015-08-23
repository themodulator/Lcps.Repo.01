namespace Lcps.UI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ExternalScopes : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "[Lcps.Division].ExternalScope",
                c => new
                    {
                        ExternalScopeKey = c.Guid(nullable: false),
                        Provider = c.Int(nullable: false),
                        ExternalCaption = c.String(),
                        InternalCaption = c.String(),
                    })
                .PrimaryKey(t => t.ExternalScopeKey);
            
        }
        
        public override void Down()
        {
            DropTable("[Lcps.Division].ExternalScope");
        }
    }
}
