namespace Lcps.UI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MSScope001 : DbMigration
    {
        public override void Up()
        {
            AddColumn("DivisionDirectory.MembershipScope", "PersonnelCaption", c => c.String());
            AddColumn("DivisionDirectory.MembershipScope", "NWUserCaption", c => c.String());
            DropColumn("DivisionDirectory.MembershipScope", "LiteralName");
        }
        
        public override void Down()
        {
            AddColumn("DivisionDirectory.MembershipScope", "LiteralName", c => c.String());
            DropColumn("DivisionDirectory.MembershipScope", "NWUserCaption");
            DropColumn("DivisionDirectory.MembershipScope", "PersonnelCaption");
        }
    }
}
