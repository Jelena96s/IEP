namespace Projekat.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migration4 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TokenOrder", "type", c => c.String(nullable: false));
            AddColumn("dbo.TokenOrder", "dateSubmitted", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TokenOrder", "dateSubmitted");
            DropColumn("dbo.TokenOrder", "type");
        }
    }
}
