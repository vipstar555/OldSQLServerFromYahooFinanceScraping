namespace FinanceEntityFramework.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPricesFourDate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Prices", "closePriceDate", c => c.String());
            AddColumn("dbo.Prices", "openPriceDate", c => c.String());
            AddColumn("dbo.Prices", "highPriceDate", c => c.String());
            AddColumn("dbo.Prices", "lowPriceDate", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Prices", "lowPriceDate");
            DropColumn("dbo.Prices", "highPriceDate");
            DropColumn("dbo.Prices", "openPriceDate");
            DropColumn("dbo.Prices", "closePriceDate");
        }
    }
}
