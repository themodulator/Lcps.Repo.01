namespace Lcps.UI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "[Lcps.Division].MembershipFilter",
                c => new
                    {
                        MembershipFilterKey = c.Guid(nullable: false),
                        Schema = c.Int(nullable: false),
                        MembershipScope = c.Long(nullable: false),
                        MemberClass = c.Int(nullable: false),
                        Caption = c.String(nullable: false, maxLength: 128),
                        Assignment = c.String(nullable: false, maxLength: 128),
                        AntecedentId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.MembershipFilterKey)
                .Index(t => t.Caption, unique: true, name: "IX_MembershipFilter_Caption");
            
            CreateTable(
                "[Lcps.Division].MembershipScope",
                c => new
                    {
                        MembershipScopeId = c.Guid(nullable: false),
                        BinaryValue = c.Long(nullable: false),
                        Caption = c.String(maxLength: 128),
                        ScopeQualifier = c.Int(nullable: false),
                        LiteralName = c.String(),
                    })
                .PrimaryKey(t => t.MembershipScopeId)
                .Index(t => t.BinaryValue, unique: true, name: "IX_MembershipScope_ID")
                .Index(t => t.Caption, unique: true, name: "IX_MembershipScope_Caption");
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        InternalId = c.String(nullable: false, maxLength: 128),
                        GivenName = c.String(nullable: false, maxLength: 75),
                        MiddleName = c.String(maxLength: 75),
                        SurName = c.String(nullable: false, maxLength: 75),
                        DOB = c.DateTime(nullable: false),
                        Gender = c.Int(nullable: false),
                        MembershipScope = c.Long(nullable: false),
                        MemberClass = c.Int(nullable: false),
                        Title = c.String(maxLength: 256),
                        InitialPassword = c.String(nullable: false, maxLength: 256),
                        ConfirmPassword = c.String(maxLength: 256),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.InternalId, unique: true, name: "IX_User_InternalId")
                .Index(t => new { t.GivenName, t.MiddleName, t.SurName, t.DOB }, unique: true, name: "IX_User_ID")
                .Index(t => t.MembershipScope)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUsers", new[] { "MembershipScope" });
            DropIndex("dbo.AspNetUsers", "IX_User_ID");
            DropIndex("dbo.AspNetUsers", "IX_User_InternalId");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("[Lcps.Division].MembershipScope", "IX_MembershipScope_Caption");
            DropIndex("[Lcps.Division].MembershipScope", "IX_MembershipScope_ID");
            DropIndex("[Lcps.Division].MembershipFilter", "IX_MembershipFilter_Caption");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
            DropTable("[Lcps.Division].MembershipScope");
            DropTable("[Lcps.Division].MembershipFilter");
        }
    }
}
