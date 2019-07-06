namespace CadeODinheiro.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class v3DB : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.CreditCardExpense", newName: "tbCreditCardExpense");
            RenameTable(name: "dbo.Notification", newName: "tbNotification");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.tbNotification", newName: "Notification");
            RenameTable(name: "dbo.tbCreditCardExpense", newName: "CreditCardExpense");
        }
    }
}
