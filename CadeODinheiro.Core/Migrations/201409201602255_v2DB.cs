namespace CadeODinheiro.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class v2DB : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Notification", "SituationType", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Notification", "SituationType");
        }
    }
}
