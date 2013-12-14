using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PizzaNetCommon.DTOs;
using PizzaNetCommon.Queries;
using PizzaNetCommon.Requests;
using PizzaNetDataModel.Model;
using PizzaNetWorkClient.WCFClientInfrastructure;
using PizzaService.Assemblers;

namespace PizzaNetTests
{
    [TestClass]
    public class ConnectionTests : DbTest
    {
        [TestMethod]
        public void GetIngredientsTest()
        {
            using (WorkChannel ch = new WorkChannel())
            {

                InAutoRollbackTransaction(uof =>
                    {
                        IList<Ingredient> ingredients = uof.Db.Ingredients.FindAll();
                        IList<StockIngredientDTO> ingDtos = ch.GetIngredients(empRequest).Data;

                        for (int i = 0; i < ingDtos.Count; i++)
                        {
                            Assert.IsTrue(ingredients[i].ExtraWeight == ingDtos[i].ExtraWeight);
                            Assert.IsTrue(ingredients[i].IngredientID == ingDtos[i].IngredientID);
                            Assert.IsTrue(ingredients[i].Name == ingDtos[i].Name);
                            Assert.IsTrue(ingredients[i].NormalWeight == ingDtos[i].NormalWeight);
                            Assert.IsTrue(ingredients[i].PricePerUnit == ingDtos[i].PricePerUnit);
                            Assert.IsTrue(ingredients[i].StockQuantity == ingDtos[i].StockQuantity);
                            Assert.IsTrue((ingredients[i].Recipies == null && !ingDtos[i].IsPartOfRecipe) ||
                                (ingredients[i].Recipies != null && ingDtos[i].IsPartOfRecipe));
                        }
                    });
            }
        }

        [TestMethod]
        public void GetOrdersTest()
        {
            using (var ch = new WorkChannel())
            {
                InAutoRollbackTransaction(uof =>
                    {
                        IList<Order> orders = uof.Db.Orders.FindAllEagerlyWhere(o => o.State.StateValue == State.NEW ||
                            o.State.StateValue == State.IN_REALISATION);

                        // IList<OrderDTO> dtoOrders = ch.GetOrdersWhere(QueryRequest.New(new OrdersQuery { Predicate = st => st.State.StateValue == State.IN_REALISATION || st.State.StateValue == State.NEW })).Data;
                        IList<OrderDTO> dtoOrders = ch.GetUndoneOrders(empRequest).Data;

                        Assert.IsTrue(orders.Count == dtoOrders.Count);

                        for (int i = 0; i < orders.Count; i++)
                        {
                            Assert.IsTrue(orders[i].Address == dtoOrders[i].Address);
                            Assert.IsTrue(orders[i].CustomerPhone == dtoOrders[i].CustomerPhone);
                            Assert.IsTrue(orders[i].Date == dtoOrders[i].Date);
                            Assert.IsTrue(orders[i].State.StateValue == dtoOrders[i].State.StateValue);

                            Assert.IsTrue(orders[i].OrderDetails.Count == dtoOrders[i].OrderDetailsDTO.Count);


                            List<OrderDetail> orderDetails = new List<OrderDetail>();
                            foreach (var ordDet in orders[i].OrderDetails)
                            {
                                orderDetails.Add(ordDet);
                            }
                            List<OrderIngredient> orderIngs = new List<OrderIngredient>();
                            foreach (var ordIng in orderDetails)
                            {
                                foreach (var item in ordIng.Ingredients)
                                {
                                    orderIngs.Add(item);
                                }
                            }

                            for (int j = 0; j < dtoOrders[i].OrderDetailsDTO.Count; j++)
                            {
                                for (int k = 0; k < dtoOrders[i].OrderDetailsDTO[j].Ingredients.Count; k++)
                                {
                                    Assert.IsTrue(dtoOrders[i].OrderDetailsDTO[j].Ingredients[k].IngredientID == orderIngs[j].IngredientID);
                                    Assert.IsTrue(dtoOrders[i].OrderDetailsDTO[j].Ingredients[k].Name == orderIngs[j].Ingredient.Name);
                                    Assert.IsTrue(dtoOrders[i].OrderDetailsDTO[j].Ingredients[k].Price == orderIngs[j].Ingredient.PricePerUnit);
                                    Assert.IsTrue(dtoOrders[i].OrderDetailsDTO[j].Ingredients[k].Quantity == orderIngs[j].Quantity);
                                }
                            }
                        }
                    });
            }
        }

        [TestMethod]
        public void SetOrderStateTest()
        {
            using (var ch = new WorkChannel())
            {
                InAutoRollbackTransaction(uof =>
                    {
                        //Order order = new Order
                        //{
                        //    Address = "TestAddress",
                        //    CustomerPhone = 123,
                        //    Date = DateTime.Now,
                        //    OrderDetails = null,
                        //    User = new User { Rights = 1, Phone = 1, Password = "asd", Name = "asD", Email = "2332", Address = "lol" }
                        //};

                        //order.State = uof.Db.States.Find(State.NEW);

                        //uof.Db.Orders.Insert(order);
                        //uof.Db.Commit();
                        //uof.Db.ObjectContext().DetachAll();

                        OrderDTO dtoOrder = ch.GetOrders(empRequest).Data[0];
                        dtoOrder.State.StateValue = State.IN_REALISATION;

                        ch.SetOrderState(new UpdateRequest<OrderDTO> { Data = dtoOrder, Login=emp.Email, Password=emp.Password });

                        OrderDTO o = ch.GetOrders(empRequest).Data[0];

                        Assert.IsTrue(o.State.StateValue == State.IN_REALISATION);

                        dtoOrder.State.StateValue = State.NEW;
                        ch.SetOrderState(new UpdateRequest<OrderDTO> { Data = dtoOrder, Login=emp.Email, Password=emp.Password });
                        Assert.IsTrue(dtoOrder.State.StateValue == State.NEW);
                    });
            }
        }

        [TestMethod]
        public void UpdateIngredientTest()
        {
            using (var ch = new WorkChannel())
            {
                IList<StockIngredientDTO> ings = ch.GetIngredients(empRequest).Data;
                List<StockIngredientDTO> toUpdate = new List<StockIngredientDTO>();
                string Name1 = ings[0].Name;
                string Name2 = ings[1].Name;
                ings[0].Name = "UPDATED1";
                ings[1].Name = "UPDATED2";
                toUpdate.Add(ings[0]);
                toUpdate.Add(ings[1]);
                ch.UpdateIngredient(new UpdateRequest<IList<StockIngredientDTO>> { Data = toUpdate, Login=emp.Email, Password=emp.Password });

                IList<StockIngredientDTO> newIngs = ch.GetIngredients(empRequest).Data;
                Assert.IsTrue(newIngs[0].Name == ings[0].Name);
                Assert.IsTrue(newIngs[1].Name == ings[1].Name);

                toUpdate.Clear();

                newIngs[0].Name = Name1;
                newIngs[1].Name = Name2;
                toUpdate.Add(newIngs[0]);
                toUpdate.Add(newIngs[1]);

                ch.UpdateIngredient(new UpdateRequest<IList<StockIngredientDTO>> { Data = toUpdate, Login=emp.Email, Password=emp.Password });
            }
            //ServiceMock mock = new ServiceMock();

            //IList<StockIngredientDTO> ings = mock.GetIngredients().Data;
            //List<StockIngredientDTO> toUpdate = new List<StockIngredientDTO>();
            //ings[0].Name = "UPDATED1";
            //ings[1].Name = "UPDATED2";
            //toUpdate.Add(ings[0]);
            //toUpdate.Add(ings[1]);

            //mock.UpdateIngredient(new UpdateRequest<IList<StockIngredientDTO>>
            //{
            //    Data = toUpdate
            //});


        }
    }
}
