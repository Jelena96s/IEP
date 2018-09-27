namespace Projekat.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migration1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            CreateTable(
                "dbo.AdminSettings",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        N = c.Int(nullable: false),
                        D = c.Int(nullable: false),
                        S = c.Int(nullable: false),
                        G = c.Int(nullable: false),
                        P = c.Int(nullable: false),
                        C = c.String(maxLength: 3),
                        T = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.Auction",
                c => new
                    {
                        id = c.Guid(nullable: false),
                        name = c.String(nullable: false),
                        image = c.Binary(nullable: false),
                        duration = c.Int(nullable: false),
                        startPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        currPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        tokenPrice = c.Int(nullable: false),
                        tokenValue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        currency = c.String(nullable: false, maxLength: 3),
                        timeCreated = c.DateTime(nullable: false),
                        timeOpened = c.DateTime(),
                        timeClosed = c.DateTime(),
                        status = c.String(nullable: false, maxLength: 50),
                        user_id = c.Int(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.User", t => t.user_id)
                .Index(t => t.user_id);
            
            CreateTable(
                "dbo.Bid",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        auctionId = c.Guid(nullable: false),
                        userId = c.Int(nullable: false),
                        timeSent = c.DateTime(nullable: false),
                        numOfTokens = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Auction", t => t.auctionId, cascadeDelete: true)
                .ForeignKey("dbo.User", t => t.userId, cascadeDelete: true)
                .Index(t => t.auctionId)
                .Index(t => t.userId);
            
            CreateTable(
                "dbo.User",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        name = c.String(nullable: false, maxLength: 50),
                        lastname = c.String(nullable: false, maxLength: 50),
                        mail = c.String(nullable: false, maxLength: 50),
                        password = c.String(nullable: false, maxLength: 50),
                        numOfTokens = c.Int(nullable: false),
                        isAdmin = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.TokenOrder",
                c => new
                    {
                        id = c.Guid(nullable: false),
                        userId = c.Int(nullable: false),
                        numOfTokens = c.Int(nullable: false),
                        price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        status = c.String(nullable: false, maxLength: 20),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.User", t => t.userId, cascadeDelete: true)
                .Index(t => t.userId);
            
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUserLogins");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId });
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
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
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId });
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id);
            
            DropForeignKey("dbo.TokenOrder", "userId", "dbo.User");
            DropForeignKey("dbo.Bid", "userId", "dbo.User");
            DropForeignKey("dbo.Auction", "user_id", "dbo.User");
            DropForeignKey("dbo.Bid", "auctionId", "dbo.Auction");
            DropIndex("dbo.TokenOrder", new[] { "userId" });
            DropIndex("dbo.Bid", new[] { "userId" });
            DropIndex("dbo.Bid", new[] { "auctionId" });
            DropIndex("dbo.Auction", new[] { "user_id" });
            DropTable("dbo.TokenOrder");
            DropTable("dbo.User");
            DropTable("dbo.Bid");
            DropTable("dbo.Auction");
            DropTable("dbo.AdminSettings");
            CreateIndex("dbo.AspNetUserLogins", "UserId");
            CreateIndex("dbo.AspNetUserClaims", "UserId");
            CreateIndex("dbo.AspNetUsers", "UserName", unique: true, name: "UserNameIndex");
            CreateIndex("dbo.AspNetUserRoles", "RoleId");
            CreateIndex("dbo.AspNetUserRoles", "UserId");
            CreateIndex("dbo.AspNetRoles", "Name", unique: true, name: "RoleNameIndex");
            AddForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers", "Id", cascadeDelete: true);
            AddForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers", "Id", cascadeDelete: true);
            AddForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers", "Id", cascadeDelete: true);
            AddForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles", "Id", cascadeDelete: true);
        }
    }
}
