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
            HomeController ctrl = new HomeController(new WorkChannelFactoryMock());
            ctrl.ControllerContext = FakeControllerSession.GetFakeControllerContext();

            ViewResult res = (ViewResult)ctrl.Index();
            Assert.IsNotNull(res);
            TrioResponse<List<RecipeDTO>, List<SizeDTO>, List<OrderIngredientDTO>> resp =
                (TrioResponse<List<RecipeDTO>, List<SizeDTO>, List<OrderIngredientDTO>>)res.Model;
            Assert.IsNotNull(resp);
            Assert.IsTrue(resp.First.Count == 1);
            Assert.IsTrue(resp.Second.Count == 3);
            Assert.IsTrue(resp.Third.Count == 1);
        }
    }
}
