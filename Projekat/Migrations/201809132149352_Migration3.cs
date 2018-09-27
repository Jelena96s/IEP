namespace Projekat.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migration3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Auction", "userIdCreate", c => c.Int());
            DropColumn("dbo.Auction", "userAdditId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Auction", "userAdditId", c => c.Int());
            DropColumn("dbo.Auction", "userIdCreate");
        }
    }
}
