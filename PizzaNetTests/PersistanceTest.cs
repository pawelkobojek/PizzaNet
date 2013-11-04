using System;
using System.Collections.Generic;
using KellermanSoftware.CompareNetObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PizzaNetDataModel;
using PizzaNetDataModel.Model;
using PizzaNetDataModel.Repository;
using System.Transactions;

namespace PizzaNetTests
{
    [TestClass]
    public class PersistanceTest : DbTest
    {
        /// <summary>
        /// This method tests correctness of setting Order to "In realisation" state.
        /// It checks whether Ingredient.StockQuantity is being substracted properly.
        /// </summary>
        [TestMethod]
        public void Order_RealisationTest()
        {
            int originalQuantity = 10;
            int inOrderQuantity = 6;
            Ingredient ing = new Ingredient { StockQuantity = originalQuantity, PricePerUnit = 0.1M, NormalWeight = 1, Name = "I", ExtraWeight = 2, Recipies = new List<Recipe>() };
            List<OrderIngredient> orings = new List<OrderIngredient>
            {
                new OrderIngredient{ Ingredient=ing, Quantity=inOrderQuantity}
            };
            List<OrderDetail> od = new List<OrderDetail>
            {
                new OrderDetail{ Size = new Size{SizeValue= Size.MEDIUM}, Ingredients = orings}
            };
            Order order = new Order { Address = "A", CustomerPhone = 123, Date = DateTime.Now, State = new State { StateValue = State.NEW }, OrderDetails = od };

            InAutoRollbackTransaction(uof =>
                {
                    uof.Db.Orders.Insert(order);
                    uof.Db.Commit();
                    uof.Db.ObjectContext().DetachAll();

                    Order o = uof.Db.Orders.Get(order.OrderID);
                    Assert.IsTrue(SetToInRealisation(o, uof));
                    Assert.IsTrue(o.State.StateValue == State.IN_REALISATION);

                    Ingredient ingredient = null;
                    foreach (var orderDet in o.OrderDetails)
                    {
                        foreach (var odIng in orderDet.Ingredients)
                        {
                            ingredient = odIng.Ingredient;
                        }
                    }

                    Assert.IsTrue(ingredient.StockQuantity == originalQuantity - inOrderQuantity);

                    o.State.StateValue = State.NEW;
                    Assert.IsFalse(SetToInRealisation(o, uof));
                    foreach (var orderDet in o.OrderDetails)
                    {
                        foreach (var odIng in orderDet.Ingredients)
                        {
                            ingredient = odIng.Ingredient;
                        }
                    }
                    Assert.IsTrue(ingredient.StockQuantity == originalQuantity - inOrderQuantity);

                });
        }
        /// <summary>
        /// Method thested by Order_RealisationTest() test.
        /// It changes order state.
        /// For more details about what is being tested see information about mentioned test method.
        /// </summary>
        /// <param name="o">Order which state is to be changed to In Realisation</param>
        /// <param name="uof">Unit of work associated with the tested database.</param>
        /// <returns></returns>
        private bool SetToInRealisation(Order o, TransactionUnitOfWork uof)
        {
            try
            {
                List<OrderIngredient> orderIngredients = new List<OrderIngredient>();
                foreach (var od in o.OrderDetails)
                {
                    foreach (var odIng in od.Ingredients)
                    {
                        orderIngredients.Add(odIng);
                    }
                }
                foreach (var odIng in orderIngredients)
                {
                    Ingredient i = db.Ingredients.Get(odIng.Ingredient.IngredientID);
                    if (i.StockQuantity - odIng.Quantity < 0)
                    {
                        return false;
                    }
                    i.StockQuantity -= odIng.Quantity;
                }

                db.Commit();
            }
            catch (Exception exc)
            {
                return false;
            }
            o.State.StateValue = State.IN_REALISATION;
            return true;
        }

        /// <summary>
        /// Method testing correctness of Recipe objects persistance logic.
        /// </summary>
        [TestMethod]
        public void Recipe_PersistanceTest()
        {
            var compare = new CompareObjects();
            compare.IgnoreObjectTypes=true;

            InAutoRollbackTransaction(uof =>
                {
                    List<Ingredient> ings = new List<Ingredient>
                    {
                        new Ingredient{ ExtraWeight=3, Name="ING", NormalWeight=1, PricePerUnit=0.1M, StockQuantity=100}
                    };
                    Recipe r = new Recipe { Name = "R", Ingredients = ings };
                    uof.Db.Recipies.Insert(r);
                    uof.Db.Commit();
                    uof.Db.ObjectContext().DetachAll();

                    Recipe r2 = uof.Db.Recipies.Get(r.RecipeID);
                    Assert.IsTrue(r != r2);
                    Assert.IsTrue(compare.Compare(r, r2));
                });
        }

        /// <summary>
        /// Method testing correctness of Ingredient objects persistance logic.
        /// </summary>
        [TestMethod]
        public void Ingredients_PersistanceTest()
        {
            var compare = new CompareObjects();
            compare.IgnoreObjectTypes = true;

            InAutoRollbackTransaction(uof =>
                {
                    Ingredient ing = new Ingredient { StockQuantity = 1, PricePerUnit = 0.1M, NormalWeight = 1, Name = "ING", ExtraWeight = 2, Recipies=new List<Recipe>()};

                    uof.Db.Ingredients.Insert(ing);
                    uof.Db.Commit();
                    uof.Db.ObjectContext().DetachAll();

                    Ingredient ing2 = uof.Db.Ingredients.Get(ing.IngredientID);

                    Assert.IsTrue(ing != ing2);
                    Assert.IsTrue(compare.Compare(ing, ing2));

                    ing2.Name = "ING2";
                    uof.Db.Commit();
                    uof.Db.ObjectContext().DetachAll();

                    ing = uof.Db.Ingredients.Get(ing.IngredientID);
                    Assert.IsTrue(ing != ing2);
                    Assert.IsFalse(compare.Compare(ing, ing2));


                });
        }

    }
}
