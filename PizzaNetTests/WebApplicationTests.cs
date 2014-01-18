using Microsoft.VisualStudio.TestTools.UnitTesting;
using PizzaNetCommon.DTOs;
using PizzaNetCommon.Requests;
using PizzaNetTests.Mocks;
using PizzaWebClient.Controllers;
using PizzaWebClient.Models;
using PizzaWebClient.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace PizzaNetTests
{
    [TestClass]
    public class WebApplicationTests
    {
        [TestMethod]
        public void HomeControllerIndex()
        {
            const bool ISAJAX = false;
            HomeController ctrl = new HomeController(new WorkChannelFactoryMock());
            FakeControllerSession.SetFakeControllerContext(ctrl, ISAJAX);

            // Redirect when not logged in
            ctrl.Session["LoggedIn"] = false;
            var res1 = ctrl.Index() as RedirectToRouteResult;
            Assert.IsNotNull(res1);
            Assert.AreEqual(res1.RouteValues["action"], "Login");
            Assert.AreEqual(res1.RouteValues["controller"], "Account");

            // Pass when everything is OK
            ctrl.Session["LoggedIn"] = true;
            ViewResult res2 = ctrl.Index() as ViewResult;
            Assert.IsNotNull(res2);
            TrioResponse<List<RecipeDTO>, List<SizeDTO>, List<OrderIngredientDTO>> resp =
                (TrioResponse<List<RecipeDTO>, List<SizeDTO>, List<OrderIngredientDTO>>)res2.Model;
            Assert.IsNotNull(resp);
            Assert.IsTrue(resp.First.Count == 1);
            Assert.IsTrue(resp.Second.Count == 3);
            Assert.IsTrue(resp.Third.Count == 1);

            // Fault when wrong user
            ((UserDTO)ctrl.Session["User"]).Email = "Unknown";
            var res3 = ctrl.Index() as HttpNotFoundResult;
            Assert.IsNotNull(res3);
        }

        [TestMethod]
        public void HomeControllerMyOrders()
        {
            const bool ISAJAX = false;
            HomeController ctrl = new HomeController(new WorkChannelFactoryMock());
            FakeControllerSession.SetFakeControllerContext(ctrl, ISAJAX);

            // Redirect when not logged in
            ctrl.Session["LoggedIn"] = false;
            var res1 = ctrl.MyOrders() as RedirectToRouteResult;
            Assert.IsNotNull(res1);
            Assert.AreEqual(res1.RouteValues["action"], "Login");
            Assert.AreEqual(res1.RouteValues["controller"], "Account");

            // Pass when everything is OK
            ctrl.Session["LoggedIn"] = true;
            ViewResult res2 = ctrl.MyOrders() as ViewResult;
            Assert.IsNotNull(res2);

            var list = res2.Model as List<OrderDTO>;
            Assert.IsNotNull(list);
            Assert.IsTrue(list.Count == 1);

            // Fault when wrong user
            ((UserDTO)ctrl.Session["User"]).Email = "Unknown";
            var res3 = ctrl.MyOrders() as HttpNotFoundResult;
            Assert.IsNotNull(res3);
        }

        [TestMethod]
        public void HomeControllerGetOrderInfo()
        {
            const bool ISAJAX = false;
            int orderId = 1;

            HomeController ctrl = new HomeController(new WorkChannelFactoryMock());
            FakeControllerSession.SetFakeControllerContext(ctrl, ISAJAX);

            // Redirect when not logged in
            ctrl.Session["LoggedIn"] = false;
            var res1 = ctrl.GetOrderInfo(orderId) as RedirectToRouteResult;
            Assert.IsNotNull(res1);
            Assert.AreEqual(res1.RouteValues["action"], "Login");
            Assert.AreEqual(res1.RouteValues["controller"], "Account");

            // Pass when everything is OK
            ctrl.Session["LoggedIn"] = true;
            var res2 = ctrl.GetOrderInfo(orderId) as PartialViewResult;
            Assert.IsNotNull(res2);

            var order = res2.Model as OrderDTO;
            Assert.IsNotNull(order);
            Assert.IsTrue(order.OrderID == orderId);

            // Fault when wrong user
            ((UserDTO)ctrl.Session["User"]).Email = "Unknown";
            var res3 = ctrl.GetOrderInfo(orderId) as HttpNotFoundResult;
            Assert.IsNotNull(res3);
        }

        [TestMethod]
        public void HomeControllerAddToOrder()
        {
            const bool ISAJAX = true;
            OrderInfoDTO info = new OrderInfoDTO()
            {
                Ingredients = new int[] { 1, 2 },
                Quantities = new string[] { "normal", "extra" },
                Size = "big"
            };

            HomeController ctrl = new HomeController(new WorkChannelFactoryMock());
            FakeControllerSession.SetFakeControllerContext(ctrl, ISAJAX);

            //Redirect if not logged
            ctrl.Session["LoggedIn"] = false;
            var res1 = ctrl.AddToOrder(info) as RedirectToRouteResult;
            Assert.IsNotNull(res1);
            Assert.AreEqual(res1.RouteValues["action"], "Login");
            Assert.AreEqual(res1.RouteValues["controller"], "Account");

            //Pass if everything ok
            ctrl.Session["LoggedIn"] = true;
            var res2 = ctrl.AddToOrder(info) as PartialViewResult;
            Assert.IsNotNull(res2);

            var order = res2.Model as OrderDTO;
            Assert.IsNotNull(order);
            Assert.IsTrue(order.OrderDetailsDTO.Count == 1);
            Assert.IsTrue(order.OrderDetailsDTO[0].Ingredients.Count == 2);
            Assert.IsTrue(order.OrderDetailsDTO[0].Size.SizeValue == SizeDTO.GREAT);

            //Fault when wrong user
            ((UserDTO)ctrl.Session["User"]).Email = "Unknown";
            var res3 = ctrl.AddToOrder(info) as HttpNotFoundResult;
            Assert.IsNotNull(res3);
        }

        [TestMethod]
        public void HomeControllerMakeOrder()
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
            FakeControllerSession.SetFakeControllerContext(ctrl, ISAJAX);

            //Redirect if not logged
            ctrl.Session["LoggedIn"] = false;
            var res1 = ctrl.MakeOrder(info) as RedirectToRouteResult;
            Assert.IsNotNull(res1);
            Assert.AreEqual(res1.RouteValues["action"], "Login");
            Assert.AreEqual(res1.RouteValues["controller"], "Account");

            //Pass if everything ok
            ctrl.Session["LoggedIn"] = true;
            ctrl.MakeOrder(info);

            Assert.IsTrue(factory.LastWorkChannel.OrdersMade.Count == 1);
            Assert.IsTrue(factory.LastWorkChannel.OrdersMade[0].Ingredients.Length == 2);
            Assert.IsNull(factory.LastWorkChannel.OrdersRemoved);

            //Fault when wrong user
            ((UserDTO)ctrl.Session["User"]).Email = "Unknown";
            var res3 = ctrl.MakeOrder(info) as HttpNotFoundResult;
            Assert.IsNotNull(res3);
        }

        [TestMethod]
        public void HomeControllerEditProfileGET()
        {
            const bool ISAJAX = false;
            var factory = new WorkChannelFactoryMock();
            HomeController ctrl = new HomeController(factory);
            FakeControllerSession.SetFakeControllerContext(ctrl, ISAJAX);

            //Redirect if not logged
            ctrl.Session["LoggedIn"] = false;
            var res1 = ctrl.EditProfile() as RedirectToRouteResult;
            Assert.IsNotNull(res1);
            Assert.AreEqual(res1.RouteValues["action"], "Login");
            Assert.AreEqual(res1.RouteValues["controller"], "Account");

            //Pass if everything ok
            ctrl.Session["LoggedIn"] = true;
            ctrl.EditProfile();
            var res2 = ctrl.EditProfile() as ViewResult;
            Assert.IsNotNull(res2);
            Assert.IsNotNull(res2.Model);

            var user = res2.Model as UserViewModel;
            Assert.IsNotNull(user);
            Assert.AreEqual(user.Email, "Admin");
        }

        [TestMethod]
        public void HomeControllerEditProfilePOST()
        {
            const bool ISAJAX = true;
            var info = new UserViewModel()
            {
                Email = "Admin",
                Password = "123",
                Address = "Address",
                Phone = 999,
                Name = "AdminName2"
            };

            var factory = new WorkChannelFactoryMock();
            HomeController ctrl = new HomeController(factory);
            FakeControllerSession.SetFakeControllerContext(ctrl, ISAJAX);

            //Redirect if not logged
            // TODO - maybe we need to remove this
            ctrl.Session["LoggedIn"] = false;
            var res1 = ctrl.EditProfile(info) as RedirectToRouteResult;
            Assert.IsNotNull(res1);
            Assert.AreEqual(res1.RouteValues["action"], "Login");
            Assert.AreEqual(res1.RouteValues["controller"], "Account");
            
            //Pass if everything ok
            ctrl.Session["LoggedIn"] = true;
            var res2 = ctrl.EditProfile(info) as RedirectToRouteResult;
            Assert.IsNotNull(res2);
            Assert.AreEqual(res2.RouteValues["action"], "Index");

            var upd = factory.LastWorkChannel.UsersUpdated;
            Assert.AreEqual(upd.Count, 1);
            Assert.AreEqual(upd[0].Name, ((UserDTO)ctrl.Session["User"]).Name);
            Assert.AreEqual(upd[0].Name, "AdminName2");

            //Fault when wrong user
            ((UserDTO)ctrl.Session["User"]).Email = "Unknown";
            var res3 = ctrl.EditProfile(info) as HttpNotFoundResult;
            Assert.IsNotNull(res3);
        }

        [TestMethod]
        public void HomeControllerCreateComplaint()
        {
            const bool ISAJAX = true;
            string body = "complaint body";

            var factory = new WorkChannelFactoryMock();
            HomeController ctrl = new HomeController(factory);
            FakeControllerSession.SetFakeControllerContext(ctrl, ISAJAX);

            //Redirect if not logged
            // TODO - maybe we need to remove this
            ctrl.Session["LoggedIn"] = false;
            var res1 = ctrl.CreateComplaint(body) as RedirectToRouteResult;
            Assert.IsNotNull(res1);
            Assert.AreEqual(res1.RouteValues["action"], "Login");
            Assert.AreEqual(res1.RouteValues["controller"], "Account");
            
            //Pass if everything ok
            ctrl.Session["LoggedIn"] = true;
            var res2 = ctrl.CreateComplaint(body) as RedirectToRouteResult;
            Assert.IsNotNull(res2);
            Assert.AreEqual(res2.RouteValues["action"], "Index");

            var upd = factory.LastWorkChannel.ComplaintsMade;
            Assert.AreEqual(upd.Count, 1);
            Assert.AreEqual(upd[0].Body, "complaint body");

            //Fault when wrong user
            ((UserDTO)ctrl.Session["User"]).Email = "Unknown";
            var res3 = ctrl.CreateComplaint(body) as HttpNotFoundResult;
            Assert.IsNotNull(res3);
        }

        [TestMethod]
        public void HomeControllerChangePassword()
        {
            const bool ISAJAX = true;
            var info = new PasswordViewModel()
            {
                NewPassword = "Password2",
                NewPasswordAgain = "Password2",
                Password = "Password"
            };

            var factory = new WorkChannelFactoryMock();
            HomeController ctrl = new HomeController(factory);
            FakeControllerSession.SetFakeControllerContext(ctrl, ISAJAX);

            //Redirect if not logged
            // TODO - maybe we need to remove this
            ctrl.Session["LoggedIn"] = false;
            var res1 = ctrl.ChangePassword(info) as RedirectToRouteResult;
            Assert.IsNotNull(res1);
            Assert.AreEqual(res1.RouteValues["action"], "Login");
            Assert.AreEqual(res1.RouteValues["controller"], "Account");
            
            //Pass if everything ok
            ctrl.Session["LoggedIn"] = true;
            var res2 = ctrl.ChangePassword(info) as RedirectToRouteResult;
            Assert.IsNotNull(res2);
            Assert.AreEqual(res2.RouteValues["action"], "Index");

            var upd = factory.LastWorkChannel.UsersUpdated;
            Assert.AreEqual(upd.Count, 1);
            Assert.AreEqual(upd[0].Password, ((UserDTO)ctrl.Session["User"]).Password);
            Assert.AreEqual(upd[0].Password, "Password2");

            //Fault when wrong user
            ((UserDTO)ctrl.Session["User"]).Email = "Unknown";
            var res3 = ctrl.ChangePassword(info) as HttpNotFoundResult;
            Assert.IsNotNull(res3);
        }

        [TestMethod]
        public void AccountControllerLogin()
        {
            const bool ISAJAX = true;
            var info = new LoginModel()
            {
                Email = "Admin",
                Password = "123"
            };
            string returnUrl = "returnUrl";

            var factory = new WorkChannelFactoryMock();
            var auth = new FakeAuthenticationService();
            var ctrl = new AccountController(factory, auth);
            FakeControllerSession.SetFakeControllerContext(ctrl, ISAJAX);

            //Pass if everything ok
            var res2 = ctrl.Login(info, returnUrl) as RedirectToRouteResult;
            Assert.IsNotNull(res2);
            Assert.AreEqual(res2.RouteValues["action"], "Index");

            var user = ctrl.Session["User"] as UserDTO;
            Assert.IsNotNull(user);
            Assert.AreEqual(user.Email, "Admin");
            Assert.AreEqual(user.Password, "123");

            //Fault when wrong user
            info.Email = "unknown";
            var res3 = ctrl.Login(info, returnUrl) as HttpNotFoundResult;
            Assert.IsNotNull(res3);
        }
    }
}
