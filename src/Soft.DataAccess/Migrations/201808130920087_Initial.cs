namespace Soft.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AccountManager",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.BankAccount",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AccountNo = c.String(nullable: false, maxLength: 50),
                        CustomerId = c.Int(nullable: false),
                        IsForeignAccount = c.Boolean(nullable: false),
                        Notes = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Customer", t => t.CustomerId, cascadeDelete: true)
                .Index(t => t.CustomerId);
            
            CreateTable(
                "dbo.Customer",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CustomerNo = c.String(nullable: false, maxLength: 20),
                        Name = c.String(nullable: false, maxLength: 50),
                        CustomerSince = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        InternetAdress = c.String(maxLength: 50),
                        AccountManagerId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AccountManager", t => t.AccountManagerId)
                .Index(t => t.AccountManagerId);
            
            CreateTable(
                "dbo.Broker",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CustomerNo = c.String(nullable: false, maxLength: 20),
                        Name = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.BrokerCustomer",
                c => new
                    {
                        Broker_Id = c.Int(nullable: false),
                        Customer_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Broker_Id, t.Customer_Id })
                .ForeignKey("dbo.Broker", t => t.Broker_Id, cascadeDelete: true)
                .ForeignKey("dbo.Customer", t => t.Customer_Id, cascadeDelete: true)
                .Index(t => t.Broker_Id)
                .Index(t => t.Customer_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.BrokerCustomer", "Customer_Id", "dbo.Customer");
            DropForeignKey("dbo.BrokerCustomer", "Broker_Id", "dbo.Broker");
            DropForeignKey("dbo.BankAccount", "CustomerId", "dbo.Customer");
            DropForeignKey("dbo.Customer", "AccountManagerId", "dbo.AccountManager");
            DropIndex("dbo.BrokerCustomer", new[] { "Customer_Id" });
            DropIndex("dbo.BrokerCustomer", new[] { "Broker_Id" });
            DropIndex("dbo.Customer", new[] { "AccountManagerId" });
            DropIndex("dbo.BankAccount", new[] { "CustomerId" });
            DropTable("dbo.BrokerCustomer");
            DropTable("dbo.Broker");
            DropTable("dbo.Customer");
            DropTable("dbo.BankAccount");
            DropTable("dbo.AccountManager");
        }
    }
}
