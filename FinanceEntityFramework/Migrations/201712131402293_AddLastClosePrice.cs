namespace FinanceEntityFramework.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddLastClosePrice : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Prices", "lastClosePrice", c => c.Double());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Prices", "lastClosePrice");
        }
    }
}
