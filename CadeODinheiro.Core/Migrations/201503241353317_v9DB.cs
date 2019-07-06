namespace CadeODinheiro.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class v9DB : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.tbNotification", "dDataFim", c => c.DateTime(nullable: false));
            Sql("update tbNotification set dDataFim = DATEADD(year, 2, dData)");
        }
        
        public override void Down()
        {
            DropColumn("dbo.tbNotification", "dDataFim");
        }
    }
}
