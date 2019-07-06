namespace CadeODinheiro.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class v5DB : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.tbCreditCardExpense", "sAccountID", "dbo.tbAccount");
            DropForeignKey("dbo.tbCreditCardExpense", "sCategoryID", "dbo.tbCategory");
            DropForeignKey("dbo.tbCreditCardExpense", "sExpenseIncomeID", "dbo.tbExpenseIncome");
            DropForeignKey("dbo.tbCreditCardExpense", "sUserID", "dbo.tbUser");
            DropIndex("dbo.tbCreditCardExpense", new[] { "sCategoryID" });
            DropIndex("dbo.tbCreditCardExpense", new[] { "sAccountID" });
            DropIndex("dbo.tbCreditCardExpense", new[] { "sExpenseIncomeID" });
            DropIndex("dbo.tbCreditCardExpense", new[] { "sUserID" });
            CreateTable(
                "dbo.tbExpenseIncomeReference",
                c => new
                    {
                        sID = c.String(nullable: false, maxLength: 128),
                        sExpenseIncomeOriginID = c.String(nullable: false, maxLength: 128),
                        sExpenseIncomeDestinyID = c.String(nullable: false, maxLength: 128),
                        dRecord = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.sID)
                .ForeignKey("dbo.tbExpenseIncome", t => t.sExpenseIncomeDestinyID, cascadeDelete: false)
                .ForeignKey("dbo.tbExpenseIncome", t => t.sExpenseIncomeOriginID, cascadeDelete: false)
                .Index(t => t.sExpenseIncomeOriginID)
                .Index(t => t.sExpenseIncomeDestinyID);
            
            DropTable("dbo.tbCreditCardExpense");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.tbCreditCardExpense",
                c => new
                    {
                        sID = c.String(nullable: false, maxLength: 128),
                        sDescricao = c.String(nullable: false),
                        sCategoryID = c.String(nullable: false, maxLength: 128),
                        sAccountID = c.String(maxLength: 128),
                        dValor = c.Double(nullable: false),
                        bPago = c.Boolean(nullable: false),
                        sExpenseIncomeID = c.String(maxLength: 128),
                        sUserID = c.String(nullable: false, maxLength: 128),
                        dRecord = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.sID);
            
            DropForeignKey("dbo.tbExpenseIncomeReference", "sExpenseIncomeOriginID", "dbo.tbExpenseIncome");
            DropForeignKey("dbo.tbExpenseIncomeReference", "sExpenseIncomeDestinyID", "dbo.tbExpenseIncome");
            DropIndex("dbo.tbExpenseIncomeReference", new[] { "sExpenseIncomeDestinyID" });
            DropIndex("dbo.tbExpenseIncomeReference", new[] { "sExpenseIncomeOriginID" });
            DropTable("dbo.tbExpenseIncomeReference");
            CreateIndex("dbo.tbCreditCardExpense", "sUserID");
            CreateIndex("dbo.tbCreditCardExpense", "sExpenseIncomeID");
            CreateIndex("dbo.tbCreditCardExpense", "sAccountID");
            CreateIndex("dbo.tbCreditCardExpense", "sCategoryID");
            AddForeignKey("dbo.tbCreditCardExpense", "sUserID", "dbo.tbUser", "sID", cascadeDelete: true);
            AddForeignKey("dbo.tbCreditCardExpense", "sExpenseIncomeID", "dbo.tbExpenseIncome", "sID");
            AddForeignKey("dbo.tbCreditCardExpense", "sCategoryID", "dbo.tbCategory", "sID", cascadeDelete: true);
            AddForeignKey("dbo.tbCreditCardExpense", "sAccountID", "dbo.tbAccount", "sID");
        }
    }
}
