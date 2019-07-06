namespace CadeODinheiro.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class v8DB : DbMigration
    {
        public override void Up()
        {
            RenameColumn("dbo.tbAccount", "SituationType", "StatusType");
            RenameColumn("dbo.tbNotification", "SituationType", "StatusType");
        }
        
        public override void Down()
        {
            RenameColumn("dbo.tbAccount", "StatusType", "SituationType");
            RenameColumn("dbo.tbNotification", "StatusType", "SituationType");
        }
    }
}
