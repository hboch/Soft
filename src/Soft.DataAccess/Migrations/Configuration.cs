namespace Soft.DataAccess.Migrations
{
    using Soft.Model;   
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Soft.DataAccess.SoftDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(Soft.DataAccess.SoftDbContext context)
        {
            context.Customers.AddOrUpdate(
                p => p.Name,
                new Customer { CustomerNo = "1", Name = "Bill Gates", CustomerSince = Convert.ToDateTime("01.01.2000"), InternetAdress = "http:\\microsoft.com" }
                , new Customer { CustomerNo = "2", Name = "Mark Zuckerberg", CustomerSince = Convert.ToDateTime("31.12.2010"), InternetAdress = "http:\\facebook.com" }
                , new Customer { CustomerNo = "3", Name = "Willy Wacker", CustomerSince = Convert.ToDateTime("31.01.1975") }
);

            context.SaveChanges();

            context.AccountManagers.AddOrUpdate(
                p => p.Name,
                new Model.AccountManager { Name = "Manager 1" },
                new Model.AccountManager { Name = "Manager 2" }
                );

            context.BankAccounts.AddOrUpdate(
                k => k.AccountNo,
                new BankAccount { AccountNo = "1012345678", CustomerId = context.Customers.Single(k => k.CustomerNo == "1").Id },
                new BankAccount { AccountNo = "1123456789", CustomerId = context.Customers.Single(k => k.CustomerNo == "1").Id },
                new BankAccount { AccountNo = "2012345678", CustomerId = context.Customers.Single(k => k.CustomerNo == "2").Id }
                );

            context.Brokers.AddOrUpdate(kv => kv.CustomerNo,
                new Broker
                {
                    CustomerNo = "1111",
                    Name = "Broker 1",
                    Customers = new List<Customer>
                    {
                        context.Customers.Single(k => k.CustomerNo == "1")
                    }
                });
        }
    }
}
