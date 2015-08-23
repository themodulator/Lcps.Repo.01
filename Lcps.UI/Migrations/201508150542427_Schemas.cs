namespace Lcps.UI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Schemas : DbMigration
    {
        public override void Up()
        {
            MoveTable(name: "[Lcps.Division].ExternalScope", newSchema: "DivisionDirectory");
            MoveTable(name: "[Lcps.Division].MembershipFilter", newSchema: "DivisionDirectory");
            MoveTable(name: "[Lcps.Division].MembershipScope", newSchema: "DivisionDirectory");
        }
        
        public override void Down()
        {
            MoveTable(name: "DivisionDirectory.MembershipScope", newSchema: "Lcps.Division");
            MoveTable(name: "DivisionDirectory.MembershipFilter", newSchema: "Lcps.Division");
            MoveTable(name: "DivisionDirectory.ExternalScope", newSchema: "Lcps.Division");
        }
    }
}
