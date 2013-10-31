namespace PizzaNetDataModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
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
                        PricePerUnit = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.IngredientID);
            
            CreateTable(
                "dbo.Recipies",
                c => new
                    {
                        RecipeID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.RecipeID);
            
            CreateTable(
                "dbo.Order_Details",
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
                "dbo.Order_Ingredients",
                c => new
                    {
                        IngredientID = c.Int(nullable: false),
                        OrderDetailID = c.Int(nullable: false),
                        Quantity = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.IngredientID, t.OrderDetailID })
                .ForeignKey("dbo.Ingredients", t => t.IngredientID, cascadeDelete: true)
                .ForeignKey("dbo.Order_Details", t => t.OrderDetailID, cascadeDelete: true)
                .Index(t => t.IngredientID)
                .Index(t => t.OrderDetailID);
            
            CreateTable(
                "dbo.Orders",
                c => new
                    {
                        OrderID = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        CustomerPhone = c.Int(nullable: false),
                        Address = c.String(),
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
                        SizeValue = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.SizeID);
            
            CreateTable(
                "dbo.RecipeIngredients",
                c => new
                    {
                        Recipe_RecipeID = c.Int(nullable: false),
                        Ingredient_IngredientID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Recipe_RecipeID, t.Ingredient_IngredientID })
                .ForeignKey("dbo.Recipies", t => t.Recipe_RecipeID, cascadeDelete: true)
                .ForeignKey("dbo.Ingredients", t => t.Ingredient_IngredientID, cascadeDelete: true)
                .Index(t => t.Recipe_RecipeID)
                .Index(t => t.Ingredient_IngredientID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Order_Details", "SizeID", "dbo.Sizes");
            DropForeignKey("dbo.Order_Details", "OrderID", "dbo.Orders");
            DropForeignKey("dbo.Orders", "StateID", "dbo.States");
            DropForeignKey("dbo.Order_Ingredients", "OrderDetailID", "dbo.Order_Details");
            DropForeignKey("dbo.Order_Ingredients", "IngredientID", "dbo.Ingredients");
            DropForeignKey("dbo.RecipeIngredients", "Ingredient_IngredientID", "dbo.Ingredients");
            DropForeignKey("dbo.RecipeIngredients", "Recipe_RecipeID", "dbo.Recipies");
            DropIndex("dbo.Order_Details", new[] { "SizeID" });
            DropIndex("dbo.Order_Details", new[] { "OrderID" });
            DropIndex("dbo.Orders", new[] { "StateID" });
            DropIndex("dbo.Order_Ingredients", new[] { "OrderDetailID" });
            DropIndex("dbo.Order_Ingredients", new[] { "IngredientID" });
            DropIndex("dbo.RecipeIngredients", new[] { "Ingredient_IngredientID" });
            DropIndex("dbo.RecipeIngredients", new[] { "Recipe_RecipeID" });
            DropTable("dbo.RecipeIngredients");
            DropTable("dbo.Sizes");
            DropTable("dbo.States");
            DropTable("dbo.Orders");
            DropTable("dbo.Order_Ingredients");
            DropTable("dbo.Order_Details");
            DropTable("dbo.Recipies");
            DropTable("dbo.Ingredients");
        }
    }
}
