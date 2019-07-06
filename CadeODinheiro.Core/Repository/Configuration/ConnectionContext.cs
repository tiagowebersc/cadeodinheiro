using CadeODinheiro.Core.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CadeODinheiro.Core.Migrations;

namespace CadeODinheiro.Core.Repository.Configuration
{
    public class ConnectionContext : DbContext
    {
        public ConnectionContext() : base("CadeODinheiroCS") { }

        public DbSet<Account> Accounts { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<ExpenseIncomeReference> ExpenseIncomeReferences { get; set; }

        public DbSet<ExpenseIncome> ExpensesIncomes { get; set; }

        public DbSet<Log> Logs { get; set; }

        public DbSet<Notification> Notifications { get; set; }

        public DbSet<Parameter> Parameters { get; set; }

        public DbSet<User> Users { get; set; }



        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer<ConnectionContext>(new CreateDatabaseIfNotExists<ConnectionContext>());
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<ConnectionContext, CadeODinheiro.Core.Migrations.Configuration>());
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

    }
}
