using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PizzaNetCommon.DTOs;
using PizzaNetCommon.Queries;
using PizzaNetCommon.Requests;
using PizzaNetDataModel.Model;
using PizzaNetWorkClient.WCFClientInfrastructure;

namespace PizzaNetTests
{
    [TestClass]
    public class ConnectionTests : DbTest
    {
        private const string ADDRESS = "http://localhost:60499/PizzaService.svc";

        [TestMethod]
        public void GetIngredientsTest()
        {
            using (WorkChannel ch = new WorkChannel(ADDRESS))
            {

                InAutoRollbackTransaction(uof =>
                    {
                        IList<Ingredient> ingredients = uof.Db.Ingredients.FindAll();
                        IList<StockIngredientDTO> ingDtos = ch.GetIngredients().Data;

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
            using (var ch = new WorkChannel(ADDRESS))
            {
                InAutoRollbackTransaction(uof =>
                    {
                        IList<Order> orders = uof.Db.Orders.FindAllEagerlyWhere(o => o.State.StateValue == State.NEW ||
                            o.State.StateValue == State.IN_REALISATION);

                        // IList<OrderDTO> dtoOrders = ch.GetOrdersWhere(QueryRequest.New(new OrdersQuery { Predicate = st => st.State.StateValue == State.IN_REALISATION || st.State.StateValue == State.NEW })).Data;
                        IList<OrderDTO> dtoOrders = ch.GetUndoneOrders().Data;

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
    }
}
