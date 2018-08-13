using Soft.Model;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace Soft.DataAccess
{
    /// <summary>
    /// Entity Framework Database Context of the application
    /// </summary>
    public class SoftDbContext : DbContext
    {
        public SoftDbContext()
            : base("SoftDb")
        //CUSTOMIZE Connection String reference in :base()
        {
        }

        //In NuGet-Package-Manager-Console:
        //0. Enable-Migrations {set project to Soft.DataAccess first}
        //1. Add-Migration {a migration name}
        //2. Update-Databae
        //repeat 1. and 2. when data(base) model changes
        //Remark: Evtl. 1. Add-Migration {a migration name} -Force, when migration has to be repeated because of errors

        //CUSTOMIZE DbSet and DataAnnotations in Soft.Model classes
        public DbSet<Customer> Customers { get; set; }
        public DbSet<AccountManager> AccountManagers { get; set; }
        public DbSet<BankAccount> BankAccounts { get; set; }
        public DbSet<Broker> Brokers { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //De-activate that EF generates pluralized table names
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            //map C# System.DateTime type to SQL Server datetime2 type
            modelBuilder.Properties<System.DateTime>().Configure(c => c.HasColumnType("datetime2"));
        }
    }
}
