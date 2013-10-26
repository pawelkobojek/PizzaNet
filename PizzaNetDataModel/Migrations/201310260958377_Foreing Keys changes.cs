namespace PizzaNetDataModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ForeingKeyschanges : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.Recipes", newName: "Recipies");
            RenameTable(name: "dbo.OrderDetails", newName: "Order_Details");
            RenameTable(name: "dbo.OrderIngredients", newName: "Order_Ingredients");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.Order_Ingredients", newName: "OrderIngredients");
            RenameTable(name: "dbo.Order_Details", newName: "OrderDetails");
            RenameTable(name: "dbo.Recipies", newName: "Recipes");
        }
    }
}
