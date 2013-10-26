namespace PizzaNetDataModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Ingredients",
                c => new
                    {
                        IngredientID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        StockQuantity = c.Int(nullable: false),
                        NormalWeight = c.Int(nullable: false),
                        ExtraWeight = c.Int(nullable: false),
                        PricePerUnit = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.IngredientID);
            
            CreateTable(
                "dbo.Recipes",
                c => new
                    {
                        RecipeID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.RecipeID);
            
            CreateTable(
                "dbo.OrderDetails",
                c => new
                    {
                        OrderDetailID = c.Int(nullable: false, identity: true),
                        OrderID = c.Int(nullable: false),
                        SizeID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.OrderDetailID)
                .ForeignKey("dbo.Orders", t => t.OrderID, cascadeDelete: true)
                .ForeignKey("dbo.Sizes", t => t.SizeID, cascadeDelete: true)
                .Index(t => t.OrderID)
                .Index(t => t.SizeID);
            
            CreateTable(
                "dbo.Orders",
                c => new
                    {
                        OrderID = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        CustomerPhone = c.Int(nullable: false),
                        StateID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.OrderID)
                .ForeignKey("dbo.States", t => t.StateID, cascadeDelete: true)
                .Index(t => t.StateID);
            
            CreateTable(
                "dbo.States",
                c => new
                    {
                        StateID = c.Int(nullable: false, identity: true),
                        StateValue = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.StateID);
            
            CreateTable(
                "dbo.Sizes",
                c => new
                    {
                        SizeID = c.Int(nullable: false, identity: true),
                        SizeValue = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.SizeID);
            
            CreateTable(
                "dbo.OrderIngredients",
                c => new
                    {
                        IngredientID = c.Int(nullable: false),
                        OrderDetailID = c.Int(nullable: false),
                        Quantity = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.IngredientID, t.OrderDetailID })
                .ForeignKey("dbo.Ingredients", t => t.IngredientID, cascadeDelete: true)
                .ForeignKey("dbo.OrderDetails", t => t.OrderDetailID, cascadeDelete: true)
                .Index(t => t.IngredientID)
                .Index(t => t.OrderDetailID);
            
            CreateTable(
                "dbo.RecipeIngredients",
                c => new
                    {
                        Recipe_RecipeID = c.Int(nullable: false),
                        Ingredient_IngredientID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Recipe_RecipeID, t.Ingredient_IngredientID })
                .ForeignKey("dbo.Recipes", t => t.Recipe_RecipeID, cascadeDelete: true)
                .ForeignKey("dbo.Ingredients", t => t.Ingredient_IngredientID, cascadeDelete: true)
                .Index(t => t.Recipe_RecipeID)
                .Index(t => t.Ingredient_IngredientID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OrderIngredients", "OrderDetailID", "dbo.OrderDetails");
            DropForeignKey("dbo.OrderIngredients", "IngredientID", "dbo.Ingredients");
            DropForeignKey("dbo.OrderDetails", "SizeID", "dbo.Sizes");
            DropForeignKey("dbo.OrderDetails", "OrderID", "dbo.Orders");
            DropForeignKey("dbo.Orders", "StateID", "dbo.States");
            DropForeignKey("dbo.RecipeIngredients", "Ingredient_IngredientID", "dbo.Ingredients");
            DropForeignKey("dbo.RecipeIngredients", "Recipe_RecipeID", "dbo.Recipes");
            DropIndex("dbo.OrderIngredients", new[] { "OrderDetailID" });
            DropIndex("dbo.OrderIngredients", new[] { "IngredientID" });
            DropIndex("dbo.OrderDetails", new[] { "SizeID" });
            DropIndex("dbo.OrderDetails", new[] { "OrderID" });
            DropIndex("dbo.Orders", new[] { "StateID" });
            DropIndex("dbo.RecipeIngredients", new[] { "Ingredient_IngredientID" });
            DropIndex("dbo.RecipeIngredients", new[] { "Recipe_RecipeID" });
            DropTable("dbo.RecipeIngredients");
            DropTable("dbo.OrderIngredients");
            DropTable("dbo.Sizes");
            DropTable("dbo.States");
            DropTable("dbo.Orders");
            DropTable("dbo.OrderDetails");
            DropTable("dbo.Recipes");
            DropTable("dbo.Ingredients");
        }
    }
}
