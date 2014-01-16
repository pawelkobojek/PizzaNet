using Microsoft.VisualStudio.TestTools.UnitTesting;
using PizzaNetCommon.DTOs;
using PizzaNetCommon.Requests;
using PizzaNetTests.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebClient.Controllers;

namespace PizzaNetTests
{
    [TestClass]
    public class WebApplicationTests
    {
        [TestMethod]
        public void HomeControllerIndexTest()
        {
            const bool ISAJAX = false;
            HomeController ctrl = new HomeController(new WorkChannelFactoryMock());
            ctrl.ControllerContext = FakeControllerSession.GetFakeControllerContext(ISAJAX);

            ctrl.Session["LoggedIn"] = false;
            var res1 = ctrl.Index() as RedirectToRouteResult;
            Assert.IsNotNull(res1);

            ctrl.Session["LoggedIn"] = true;
            ViewResult res2 = ctrl.Index() as ViewResult;
            Assert.IsNotNull(res2);
            TrioResponse<List<RecipeDTO>, List<SizeDTO>, List<OrderIngredientDTO>> resp =
                (TrioResponse<List<RecipeDTO>, List<SizeDTO>, List<OrderIngredientDTO>>)res2.Model;
            Assert.IsNotNull(resp);
            Assert.IsTrue(resp.First.Count == 1);
            Assert.IsTrue(resp.Second.Count == 3);
            Assert.IsTrue(resp.Third.Count == 1);
        }

        [TestMethod]
        public void HomeControllerMyOrdersTest()
        {
            const bool ISAJAX = false;
            HomeController ctrl = new HomeController(new WorkChannelFactoryMock());
            ctrl.ControllerContext = FakeControllerSession.GetFakeControllerContext(ISAJAX);

            ctrl.Session["LoggedIn"] = false;
            var res1 = ctrl.MyOrders() as RedirectToRouteResult;
            Assert.IsNotNull(res1);

            ctrl.Session["LoggedIn"] = true;
            ViewResult res2 = ctrl.MyOrders() as ViewResult;
            Assert.IsNotNull(res2);

            var list = res2.Model as List<OrderDTO>;
            Assert.IsNotNull(list);
            Assert.IsTrue(list.Count == 1);
        }

        [TestMethod]
        public void HomeControllerGetOrderInfoTest()
        {
            const bool ISAJAX = false;
            int orderId = 1;

            HomeController ctrl = new HomeController(new WorkChannelFactoryMock());
            ctrl.ControllerContext = FakeControllerSession.GetFakeControllerContext(ISAJAX);

            ctrl.Session["LoggedIn"] = false;
            var res1 = ctrl.GetOrderInfo(orderId) as RedirectToRouteResult;
            Assert.IsNotNull(res1);

            ctrl.Session["LoggedIn"] = true;
            var res2 = ctrl.GetOrderInfo(orderId) as PartialViewResult;
            Assert.IsNotNull(res2);

            var order = res2.Model as OrderDTO;
            Assert.IsNotNull(order);
            Assert.IsTrue(order.OrderID == orderId);
        }

        [TestMethod]
        public void HomeControllerAddToOrderTest()
        {
            const bool ISAJAX = true;
            OrderInfoDTO info = new OrderInfoDTO()
            {
                Ingredients = new int[] { 1, 2 },
                Quantities = new string[] { "normal", "extra" },
                Size = "big"
            };

            HomeController ctrl = new HomeController(new WorkChannelFactoryMock());
            ctrl.ControllerContext = FakeControllerSession.GetFakeControllerContext(ISAJAX);

            ctrl.Session["LoggedIn"] = false;
            var res1 = ctrl.AddToOrder(info) as RedirectToRouteResult;
            Assert.IsNotNull(res1);

            ctrl.Session["LoggedIn"] = true;
            var res2 = ctrl.AddToOrder(info) as PartialViewResult;
            Assert.IsNotNull(res2);

            var order = res2.Model as OrderDTO;
            Assert.IsNotNull(order);
            Assert.IsTrue(order.OrderDetailsDTO.Count == 1);
            Assert.IsTrue(order.OrderDetailsDTO[0].Ingredients.Count == 2);
            Assert.IsTrue(order.OrderDetailsDTO[0].Size.SizeValue == SizeDTO.GREAT);
        }

        [TestMethod]
        public void HomeControllerMakeOrderTest()
        {
            const bool ISAJAX = false;
            var info = new List<OrderInfoDTO>()
            {
                new OrderInfoDTO()
                {
                    Ingredients = new int[] { 1, 2 },
                    Quantities = new string[] { "normal", "extra" },
                    Size = "big"
                }
            };

            var factory = new WorkChannelFactoryMock();
            HomeController ctrl = new HomeController(factory);
            ctrl.ControllerContext = FakeControllerSession.GetFakeControllerContext(ISAJAX);

            ctrl.Session["LoggedIn"] = false;
            var res1 = ctrl.MakeOrder(info) as RedirectToRouteResult;
            Assert.IsNotNull(res1);

            ctrl.Session["LoggedIn"] = true;
            ctrl.MakeOrder(info);

            Assert.IsTrue(factory.LastWorkChannel.OrdersMade.Count == 1);
            Assert.IsTrue(factory.LastWorkChannel.OrdersMade[0].Ingredients.Length == 2);
            Assert.IsNull(factory.LastWorkChannel.OrdersRemoved);
        }
    }
}
