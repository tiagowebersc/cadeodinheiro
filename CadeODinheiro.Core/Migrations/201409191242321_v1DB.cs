namespace CadeODinheiro.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class v1DB : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.tbAccount",
                c => new
                    {
                        sID = c.String(nullable: false, maxLength: 128),
                        sNome = c.String(nullable: false, maxLength: 30),
                        sDescricao = c.String(nullable: false, maxLength: 255),
                        AccountType = c.Int(nullable: false),
                        dSaldo = c.Double(nullable: false),
                        sUserID = c.String(nullable: false, maxLength: 128),
                        dRecord = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.sID)
                .ForeignKey("dbo.tbUser", t => t.sUserID, cascadeDelete: true)
                .Index(t => t.sUserID);
            
            CreateTable(
                "dbo.tbUser",
                c => new
                    {
                        sID = c.String(nullable: false, maxLength: 128),
                        login = c.String(nullable: false, maxLength: 10),
                        senha = c.String(nullable: false, maxLength: 255),
                        nome = c.String(nullable: false, maxLength: 80),
                        dRecord = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.sID);
            
            CreateTable(
                "dbo.tbCategory",
                c => new
                    {
                        sID = c.String(nullable: false, maxLength: 128),
                        descricao = c.String(nullable: false, maxLength: 50),
                        CategoryType = c.Int(nullable: false),
                        sUserID = c.String(nullable: false, maxLength: 128),
                        dRecord = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.sID)
                .ForeignKey("dbo.tbUser", t => t.sUserID, cascadeDelete: true)
                .Index(t => t.sUserID);
            
            CreateTable(
                "dbo.CreditCardExpense",
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
                .PrimaryKey(t => t.sID)
                .ForeignKey("dbo.tbAccount", t => t.sAccountID)
                .ForeignKey("dbo.tbCategory", t => t.sCategoryID, cascadeDelete: true)
                .ForeignKey("dbo.tbExpenseIncome", t => t.sExpenseIncomeID)
                .ForeignKey("dbo.tbUser", t => t.sUserID, cascadeDelete: false)
                .Index(t => t.sCategoryID)
                .Index(t => t.sAccountID)
                .Index(t => t.sExpenseIncomeID)
                .Index(t => t.sUserID);
            
            CreateTable(
                "dbo.tbExpenseIncome",
                c => new
                    {
                        sID = c.String(nullable: false, maxLength: 128),
                        sDescricao = c.String(nullable: false),
                        CategoryType = c.Int(nullable: false),
                        sCategoryID = c.String(nullable: false, maxLength: 128),
                        sAccountID = c.String(nullable: false, maxLength: 128),
                        dValor = c.Double(nullable: false),
                        dData = c.DateTime(nullable: false),
                        bTransfer = c.Boolean(nullable: false),
                        sExpenseIncomeTransferID = c.String(maxLength: 128),
                        sNotificationID = c.String(maxLength: 128),
                        sUserID = c.String(nullable: false, maxLength: 128),
                        dRecord = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.sID)
                .ForeignKey("dbo.tbAccount", t => t.sAccountID, cascadeDelete: true)
                .ForeignKey("dbo.tbCategory", t => t.sCategoryID, cascadeDelete: false)
                .ForeignKey("dbo.tbExpenseIncome", t => t.sExpenseIncomeTransferID)
                .ForeignKey("dbo.Notification", t => t.sNotificationID)
                .ForeignKey("dbo.tbUser", t => t.sUserID, cascadeDelete: false)
                .Index(t => t.sCategoryID)
                .Index(t => t.sAccountID)
                .Index(t => t.sExpenseIncomeTransferID)
                .Index(t => t.sNotificationID)
                .Index(t => t.sUserID);
            
            CreateTable(
                "dbo.Notification",
                c => new
                    {
                        sID = c.String(nullable: false, maxLength: 128),
                        sDescricao = c.String(nullable: false),
                        CategoryType = c.Int(nullable: false),
                        sCategoryID = c.String(nullable: false, maxLength: 128),
                        dValor = c.Double(nullable: false),
                        NotificationType = c.Int(nullable: false),
                        dData = c.DateTime(nullable: false),
                        sUserID = c.String(nullable: false, maxLength: 128),
                        dRecord = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.sID)
                .ForeignKey("dbo.tbCategory", t => t.sCategoryID, cascadeDelete: true)
                .ForeignKey("dbo.tbUser", t => t.sUserID, cascadeDelete: false)
                .Index(t => t.sCategoryID)
                .Index(t => t.sUserID);
            
            CreateTable(
                "dbo.tbLog",
                c => new
                    {
                        sID = c.String(nullable: false, maxLength: 128),
                        sOperacao = c.String(maxLength: 25),
                        sAgent = c.String(),
                        sAction = c.String(),
                        sController = c.String(),
                        sSession = c.String(),
                        sIP = c.String(maxLength: 20),
                        dRecord = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.sID);
            
            CreateTable(
                "dbo.tbParameter",
                c => new
                    {
                        sID = c.String(nullable: false, maxLength: 128),
                        sParametro = c.String(nullable: false, maxLength: 50),
                        sValor = c.String(nullable: false, maxLength: 200),
                        sDescricao = c.String(maxLength: 250),
                        dRecord = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.sID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CreditCardExpense", "sUserID", "dbo.tbUser");
            DropForeignKey("dbo.CreditCardExpense", "sExpenseIncomeID", "dbo.tbExpenseIncome");
            DropForeignKey("dbo.tbExpenseIncome", "sUserID", "dbo.tbUser");
            DropForeignKey("dbo.tbExpenseIncome", "sNotificationID", "dbo.Notification");
            DropForeignKey("dbo.Notification", "sUserID", "dbo.tbUser");
            DropForeignKey("dbo.Notification", "sCategoryID", "dbo.tbCategory");
            DropForeignKey("dbo.tbExpenseIncome", "sExpenseIncomeTransferID", "dbo.tbExpenseIncome");
            DropForeignKey("dbo.tbExpenseIncome", "sCategoryID", "dbo.tbCategory");
            DropForeignKey("dbo.tbExpenseIncome", "sAccountID", "dbo.tbAccount");
            DropForeignKey("dbo.CreditCardExpense", "sCategoryID", "dbo.tbCategory");
            DropForeignKey("dbo.CreditCardExpense", "sAccountID", "dbo.tbAccount");
            DropForeignKey("dbo.tbCategory", "sUserID", "dbo.tbUser");
            DropForeignKey("dbo.tbAccount", "sUserID", "dbo.tbUser");
            DropIndex("dbo.Notification", new[] { "sUserID" });
            DropIndex("dbo.Notification", new[] { "sCategoryID" });
            DropIndex("dbo.tbExpenseIncome", new[] { "sUserID" });
            DropIndex("dbo.tbExpenseIncome", new[] { "sNotificationID" });
            DropIndex("dbo.tbExpenseIncome", new[] { "sExpenseIncomeTransferID" });
            DropIndex("dbo.tbExpenseIncome", new[] { "sAccountID" });
            DropIndex("dbo.tbExpenseIncome", new[] { "sCategoryID" });
            DropIndex("dbo.CreditCardExpense", new[] { "sUserID" });
            DropIndex("dbo.CreditCardExpense", new[] { "sExpenseIncomeID" });
            DropIndex("dbo.CreditCardExpense", new[] { "sAccountID" });
            DropIndex("dbo.CreditCardExpense", new[] { "sCategoryID" });
            DropIndex("dbo.tbCategory", new[] { "sUserID" });
            DropIndex("dbo.tbAccount", new[] { "sUserID" });
            DropTable("dbo.tbParameter");
            DropTable("dbo.tbLog");
            DropTable("dbo.Notification");
            DropTable("dbo.tbExpenseIncome");
            DropTable("dbo.CreditCardExpense");
            DropTable("dbo.tbCategory");
            DropTable("dbo.tbUser");
            DropTable("dbo.tbAccount");
        }
    }
}
