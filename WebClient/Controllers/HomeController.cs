using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PizzaNetCommon.DTOs;
using PizzaNetWorkClient.WCFClientInfrastructure;

namespace WebClient.Controllers
{
    public class HomeController : Controller
    {

        public ActionResult Index()
        {
            using (var proxy = new WorkChannel())
            {
                var data = proxy.GetRecipeTabData(new PizzaNetCommon.Requests.EmptyRequest
                {
                    Login = "Admin",
                    Password = "123"
                });

                return View(data);
            }
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult MyOrders()
        {
            List<OrderDTO> orders = new List<OrderDTO>();
            using (var proxy = new WorkChannel())
            {
                var result = proxy.GetOrders(new PizzaNetCommon.Requests.EmptyRequest { Login = "Admin", Password = "123" });
                orders = result.Data;
            }
            return View(orders);
        }

        public ActionResult Profile()
        {
            return View();
        }

    }
}
