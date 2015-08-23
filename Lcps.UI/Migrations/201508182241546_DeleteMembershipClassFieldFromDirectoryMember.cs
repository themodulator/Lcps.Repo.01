namespace Lcps.UI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DeleteMembershipClassFieldFromDirectoryMember : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.AspNetUsers", "MemberClass");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "MemberClass", c => c.Int(nullable: false));
        }
    }
}
