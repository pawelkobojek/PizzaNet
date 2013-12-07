using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PizzaNetDataModel.Model;

namespace PizzaNetDataModel
{
    /// <summary>
    /// Initializer for the appliaction's database.
    /// It prefills the database.
    /// </summary>
    internal class PizzaContextInitializer : DropCreateDatabaseIfModelChanges<PizzaContext>
    {
        /// <summary>
        /// Populates the database with default data.
        /// </summary>
        /// <param name="context">Context of the database to prefill.</param>
        protected override void Seed(PizzaContext context)
        {
            const String TOMATOS = "Pomidory";
            const String MOZARELLA = "Ser mozarella";
            const String MUSHROOMS = "Pieczarki";
            const String HAM = "Szynka";
            const String GARLIC = "Czosnek";
            const String PARMESAN = "Ser parmezan";
            const String OREGANO = "Oregano";
            const String BASIL = "Bazylia";
            const String ONION = "Cebula";
            const String PEPERONI_SAUSAGE = "Peperoni";
            const String PEPERONI_PEPPER = "Papryczka peperoni";

            const String MARGHERITA = "Margherita";
            const String CAPRI = "Capriciosa";
            const String CALZONE = "Calzone";
            const String PEPERONI = "Peperoni";

            const int NORMAL_W = 350;
            const int EXTRA_W = 700;
            const int STOCK_QUANTITY = 10000;

            User admin = new User { Address = "", Email = "Admin@admin.a", Name = "Admin", Phone = 1, Rights = 3 };
            context.Users.Add(admin);

            List<Ingredient> ingredients = new List<Ingredient>
            {
                new Ingredient { Name = TOMATOS, NormalWeight = NORMAL_W, ExtraWeight = EXTRA_W,
                    StockQuantity = STOCK_QUANTITY, PricePerUnit = 0.003M},
                new Ingredient { Name = MOZARELLA, NormalWeight = NORMAL_W, ExtraWeight = EXTRA_W,
                    StockQuantity = STOCK_QUANTITY, PricePerUnit = 0.005M},
                new Ingredient { Name = MUSHROOMS, NormalWeight = NORMAL_W, ExtraWeight = EXTRA_W,
                    StockQuantity = STOCK_QUANTITY, PricePerUnit = 0.0075M},
                new Ingredient { Name = HAM, NormalWeight = NORMAL_W, ExtraWeight = EXTRA_W,
                    StockQuantity = STOCK_QUANTITY, PricePerUnit = 0.02M},
                new Ingredient { Name = GARLIC, NormalWeight = 5, ExtraWeight = 15,
                    StockQuantity = STOCK_QUANTITY, PricePerUnit = 0.0035M},
                new Ingredient { Name = PARMESAN, NormalWeight = NORMAL_W, ExtraWeight = EXTRA_W,
                    StockQuantity = STOCK_QUANTITY, PricePerUnit = 0.1M},
                new Ingredient { Name = OREGANO, NormalWeight = 1, ExtraWeight = 3,
                    StockQuantity = STOCK_QUANTITY, PricePerUnit = 0.22M},
                new Ingredient { Name = BASIL, NormalWeight = 1, ExtraWeight = 3,
                    StockQuantity = STOCK_QUANTITY, PricePerUnit = 0.06M},
                new Ingredient { Name = ONION, NormalWeight = 70, ExtraWeight = 100,
                    StockQuantity = STOCK_QUANTITY, PricePerUnit = 0.0011M},
                new Ingredient { Name = PEPERONI_SAUSAGE, NormalWeight = NORMAL_W, ExtraWeight = EXTRA_W,
                    StockQuantity = STOCK_QUANTITY, PricePerUnit = 0.06M},
                new Ingredient { Name = PEPERONI_PEPPER, NormalWeight = 30, ExtraWeight = 60,
                    StockQuantity = STOCK_QUANTITY, PricePerUnit = 0.024M}
                            
            };

            List<Recipe> recipies = new List<Recipe>
                {
                    new Recipe { Name = MARGHERITA },
                    new Recipe { Name = CAPRI },
                    new Recipe { Name = CALZONE },
                    new Recipe { Name = PEPERONI }
                };

            recipies.Find(r => r.Name == MARGHERITA).Ingredients = new List<Ingredient>(
                ingredients.FindAll(ing => ing.Name == TOMATOS || ing.Name == MOZARELLA));

            recipies.Find(r => r.Name == CAPRI).Ingredients = new List<Ingredient>(
                ingredients.FindAll(ing => ing.Name == TOMATOS || ing.Name == MUSHROOMS || ing.Name == HAM));

            recipies.Find(r => r.Name == CALZONE).Ingredients = new List<Ingredient>(
                ingredients.FindAll(ing => ing.Name == TOMATOS || ing.Name == MUSHROOMS || ing.Name == MOZARELLA
                || ing.Name == PARMESAN));

            recipies.Find(r => r.Name == PEPERONI).Ingredients = new List<Ingredient>(
                ingredients.FindAll(ing => ing.Name == TOMATOS || ing.Name == ONION || ing.Name == GARLIC
                || ing.Name == MOZARELLA || ing.Name == PEPERONI_PEPPER || ing.Name == PEPERONI_SAUSAGE));


            List<Size> sizes = new List<Size>
            {
                new Size{ SizeValue = 1},
                new Size{ SizeValue = 1.5},
                new Size{ SizeValue = 2}
            };

            List<State> states = new List<State>
            {
                new State{ StateValue = State.NEW},
                new State{ StateValue = State.IN_REALISATION},
                new State{ StateValue = State.DONE}
            };

            List<OrderIngredient> orderIng = new List<OrderIngredient>
            {
                new OrderIngredient{ Ingredient = ingredients.Where(ing => ing.Name==PEPERONI).First(),
                    Quantity= ingredients.Where(ing => ing.Name==PEPERONI).First().NormalWeight }
            };

            List<OrderDetail> od = new List<OrderDetail>
            {
                new OrderDetail{ Ingredients = orderIng, Size=sizes[0]}
            };

            List<Order> orders = new List<Order>
            {
                new Order { Address="asd", CustomerPhone=123, Date=DateTime.Now, State = states[0],
                     OrderDetails = od, User = admin}
            };

            foreach (var item in sizes)
            {
                context.Sizes.Add(item);
            }

            foreach (var item in states)
            {
                context.States.Add(item);
            }
            foreach (var item in ingredients)
            {
                context.Ingredients.Add(item);
            }
            foreach (var item in recipies)
            {
                context.Recipies.Add(item);
            }
            foreach (var item in orders)
            {
                context.Orders.Add(item);
            }
            context.SaveChanges();
        }
    }
}
