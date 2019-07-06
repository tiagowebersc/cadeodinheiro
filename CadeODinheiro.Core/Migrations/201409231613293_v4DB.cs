namespace CadeODinheiro.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class v4DB : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.tbExpenseIncome", "sExpenseIncomeTransferID", "dbo.tbExpenseIncome");
            DropIndex("dbo.tbExpenseIncome", new[] { "sExpenseIncomeTransferID" });
            AddColumn("dbo.tbExpenseIncome", "bTransferOrigem", c => c.Boolean(nullable: false));
            AddColumn("dbo.tbExpenseIncome", "bPagoCartaoCredito", c => c.Boolean(nullable: false));
            DropColumn("dbo.tbExpenseIncome", "bTransfer");
            DropColumn("dbo.tbExpenseIncome", "sExpenseIncomeTransferID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.tbExpenseIncome", "sExpenseIncomeTransferID", c => c.String(maxLength: 128));
            AddColumn("dbo.tbExpenseIncome", "bTransfer", c => c.Boolean(nullable: false));
            DropColumn("dbo.tbExpenseIncome", "bPagoCartaoCredito");
            DropColumn("dbo.tbExpenseIncome", "bTransferOrigem");
            CreateIndex("dbo.tbExpenseIncome", "sExpenseIncomeTransferID");
            AddForeignKey("dbo.tbExpenseIncome", "sExpenseIncomeTransferID", "dbo.tbExpenseIncome", "sID");
        }
    }
}
