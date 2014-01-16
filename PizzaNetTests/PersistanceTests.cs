using System;
using System.Collections.Generic;
using KellermanSoftware.CompareNetObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PizzaNetDataModel;
using PizzaNetDataModel.Model;
using PizzaNetDataModel.Repository;
using PizzaNetWorkClient;
using System.Transactions;
using PizzaNetControls;
using PizzaNetControls.Controls;
using PizzaNetCommon.DTOs;

namespace PizzaNetTests
{
    [TestClass]
    public class PersistanceTests : DbTest
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
            Order order = new Order
            {
                Address = "A",
                CustomerPhone = 123,
                Date = DateTime.Now,
                State = new State { StateValue = State.NEW },
                OrderDetails = od
                ,
                User = new User { Address = "A", Email = "A", Name = "B", Password = "B", Phone = 2, Rights = 2 }
            };

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
            catch (Exception)
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
            compare.IgnoreObjectTypes = true;

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
                    Ingredient ing = new Ingredient { StockQuantity = 1, PricePerUnit = 0.1M, NormalWeight = 1, Name = "ING", ExtraWeight = 2, Recipies = new List<Recipe>() };

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

        [TestMethod]
        public void Ingredient_DeleteTest()
        {
            StockIngredientDTO st = new StockIngredientDTO { StockQuantity = 1, PricePerUnit = 0.1M, NormalWeight = 1, Name = "ING", ExtraWeight = 2, IsPartOfRecipe=false };
            Assert.IsNotNull(removeStockItemIngredient(st));
            st.IsPartOfRecipe = true;
            Assert.IsNull(removeStockItemIngredient(st));

        }

        public StockIngredientDTO removeStockItemIngredient(params object[] args)
        {
            StockIngredientDTO toRemove = args[0] as StockIngredientDTO;

            if (toRemove == null) return null;
            if (toRemove.IsPartOfRecipe)
            {
                return null;
            }
            return toRemove;
        }

        [TestMethod]
        public void Order_PersistanceTest()
        {
            var compare = new CompareObjects();
            compare.IgnoreObjectTypes = true;

            InAutoRollbackTransaction(uof =>
            {
                Order o = new Order
                {
                    Address = "A",
                    CustomerPhone = 123,
                    Date = DateTime.Now,
                    OrderDetails = new List<OrderDetail>(),
                    State = new State { StateValue = State.DONE },
                    User = new User { Address = "A", Email = "A", Name = "A", Password = "A", Phone = 1, Rights = 1 }
                };
                int x = ((List<Order>)uof.Db.Orders.FindAll()).Count;

                uof.Db.Orders.Insert(o);
                uof.Db.Commit();
                uof.Db.ObjectContext().DetachAll();

                int y = ((List<Order>)uof.Db.Orders.FindAll()).Count;

                Assert.IsTrue(y == x + 1);
            });
        }

        [TestMethod]
        public void Size_PersistanceTest()
        {
            var compare = new CompareObjects();
            compare.IgnoreObjectTypes = true;

            InAutoRollbackTransaction(uof =>
            {
                Size s = new Size { SizeValue = Size.MEDIUM };
                int x = ((List<Size>)uof.Db.Sizes.FindAll()).Count;

                uof.Db.Sizes.Insert(s);
                uof.Db.Commit();
                uof.Db.ObjectContext().DetachAll();

                int y = ((List<Size>)uof.Db.Sizes.FindAll()).Count;

                Assert.IsTrue(y == x + 1);
            });
        }
    }
}
