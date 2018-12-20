namespace FinanceEntityFramework.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CodeLists",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        code = c.Int(nullable: false),
                        name = c.String(),
                        market = c.String(),
                        sector = c.String(),
                        date = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.Prices",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        code = c.Int(nullable: false),
                        date = c.DateTime(nullable: false),
                        closePrice = c.Double(),
                        openPrice = c.Double(),
                        highPrice = c.Double(),
                        lowPrice = c.Double(),
                        volume = c.Long(nullable: false),
                        tradingVolume = c.Long(nullable: false),
                        limitHighPrice = c.Double(nullable: false),
                        limitLowPrice = c.Double(nullable: false),
                        codeList_id = c.Int(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.CodeLists", t => t.codeList_id)
                .Index(t => t.codeList_id);
            
            CreateTable(
                "dbo.TradeIndexes",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        code = c.Int(nullable: false),
                        date = c.DateTime(nullable: false),
                        marketCapitalization = c.Long(nullable: false),
                        outstandingShares = c.Long(nullable: false),
                        minimumPrice = c.Long(nullable: false),
                        minimumUnit = c.Int(nullable: false),
                        yearHighPrice = c.Double(),
                        yearLowPrice = c.Double(),
                        marginBuy = c.Long(nullable: false),
                        WoWMarginBuy = c.Long(nullable: false),
                        marginCell = c.Long(nullable: false),
                        WoWMarginCell = c.Long(nullable: false),
                        ratioMarginBalance = c.Double(),
                        price_id = c.Int(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Prices", t => t.price_id)
                .Index(t => t.price_id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TradeIndexes", "price_id", "dbo.Prices");
            DropForeignKey("dbo.Prices", "codeList_id", "dbo.CodeLists");
            DropIndex("dbo.TradeIndexes", new[] { "price_id" });
            DropIndex("dbo.Prices", new[] { "codeList_id" });
            DropTable("dbo.TradeIndexes");
            DropTable("dbo.Prices");
            DropTable("dbo.CodeLists");
        }
    }
}
