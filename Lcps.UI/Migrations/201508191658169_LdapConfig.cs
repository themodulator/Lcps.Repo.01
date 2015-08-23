namespace Lcps.UI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LdapConfig : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Ldap.LdapConfig",
                c => new
                    {
                        LdapConfigKey = c.Guid(nullable: false),
                        DomainPrincipalname = c.String(maxLength: 256),
                        UserName = c.String(),
                        Password = c.String(),
                    })
                .PrimaryKey(t => t.LdapConfigKey)
                .Index(t => t.DomainPrincipalname, unique: true, name: "IX_ConfigDomain_DomainName");
            
        }
        
        public override void Down()
        {
            DropIndex("Ldap.LdapConfig", "IX_ConfigDomain_DomainName");
            DropTable("Ldap.LdapConfig");
        }
    }
}
