namespace PizzaNetDataModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initializer : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Ingredients", "PricePerUnit", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.Sizes", "SizeValue", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Sizes", "SizeValue", c => c.Int(nullable: false));
            AlterColumn("dbo.Ingredients", "PricePerUnit", c => c.Int(nullable: false));
        }
    }
}
