namespace CadeODinheiro.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class v6DB : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.tbAccount", "SituationType", c => c.Int(nullable: false));
            AddColumn("dbo.tbExpenseIncome", "dDataBase", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.tbExpenseIncome", "dDataBase");
            DropColumn("dbo.tbAccount", "SituationType");
        }
    }
}
