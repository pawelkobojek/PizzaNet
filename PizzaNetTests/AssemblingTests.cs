using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PizzaNetCommon.DTOs;
using PizzaNetDataModel.Model;
using PizzaService.Assemblers;

namespace PizzaNetTests
{
    [TestClass]
    public class AssemblingTests
    {
        [TestMethod]
        public void Order_Assembling_test()
        {
            List<OrderIngredient> orderIng = new List<OrderIngredient>
            {
                new OrderIngredient{ Ingredient = new Ingredient{ ExtraWeight=20,
                    IngredientID=1,
                    Name="TestIngredient",
                    NormalWeight = 10,
                    PricePerUnit=0.1M,
                    StockQuantity=200},
                    Quantity= 10 }
            };

            List<OrderDetail> od = new List<OrderDetail>
            {
                new OrderDetail{ Ingredients = orderIng, Size=new Size{ SizeID=1, SizeValue=Size.MEDIUM}}
            };

            Order o =
                new Order
                {
                    Address = "asd",
                    CustomerPhone = 123,
                    Date = DateTime.Now,
                    State = new State { StateValue = State.NEW },
                    OrderDetails = od
                };
            OrderDTO oDto = (new OrderAssembler()).ToSimpleDto(o);

            Assert.IsTrue(oDto.Address == o.Address);
            Assert.IsTrue(oDto.CustomerPhone == o.CustomerPhone);
            Assert.IsTrue(oDto.Date == o.Date);
            Assert.IsTrue(oDto.State.StateValue == o.State.StateValue);
            Assert.IsTrue(oDto.State.StateID == o.State.StateID);
            Assert.IsTrue(oDto.OrderDetailsDTO.Count == o.OrderDetails.Count);


            List<OrderDetail> orderDetails = new List<OrderDetail>();
            foreach (var ordDet in o.OrderDetails)
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


            for (int i = 0; i < oDto.OrderDetailsDTO.Count; i++)
            {
                for (int j = 0; j < oDto.OrderDetailsDTO[i].Ingredients.Count; j++)
                {
                    Assert.IsTrue(oDto.OrderDetailsDTO[i].Ingredients[j].IngredientID == orderIngs[i].IngredientID);
                    Assert.IsTrue(oDto.OrderDetailsDTO[i].Ingredients[j].Name == orderIngs[i].Ingredient.Name);
                    Assert.IsTrue(oDto.OrderDetailsDTO[i].Ingredients[j].Price == orderIngs[i].Ingredient.PricePerUnit);
                    Assert.IsTrue(oDto.OrderDetailsDTO[i].Ingredients[j].Quantity == orderIngs[i].Quantity);
                }
            }
        }
    }
}
