namespace CadeODinheiro.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class v7DB : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.tbAccount", "iDiaFechamentoCC", c => c.Int(nullable: false));
            AddColumn("dbo.tbExpenseIncome", "iNumeroOcorrencia", c => c.Int(nullable: false));
            AddColumn("dbo.tbExpenseIncome", "iTotalOcorrencia", c => c.Int(nullable: false));
            AddColumn("dbo.tbExpenseIncome", "sAgrupadorOcorrencia", c => c.String());
            Sql("update tbAccount set iDiaFechamentoCC = 1");
            Sql("update tbExpenseIncome set iNumeroOcorrencia = 1");
            Sql("update tbExpenseIncome set iTotalOcorrencia = 1");
        }
        
        public override void Down()
        {
            DropColumn("dbo.tbExpenseIncome", "sAgrupadorOcorrencia");
            DropColumn("dbo.tbExpenseIncome", "iTotalOcorrencia");
            DropColumn("dbo.tbExpenseIncome", "iNumeroOcorrencia");
            DropColumn("dbo.tbAccount", "iDiaFechamentoCC");
        }
    }
}
