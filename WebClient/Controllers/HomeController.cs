using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PizzaNetWorkClient.WCFClientInfrastructure;

namespace WebClient.Controllers
{
    public class HomeController : Controller
    {

        public string Index()
        {
            //ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";

            // ŻEBY SPRAWDZIC CZY DZIAŁA
            string wtf;
            using (var proxy = new WorkChannel())
            {
                var result = proxy.GetOrders(new PizzaNetCommon.Requests.EmptyRequest { Login = "Admin", Password = "123" });
                wtf = result.Data[0].Address;
            }

            return wtf;
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
            return View();
        }

        public ActionResult Profile()
        {
            return View();
        }

    }
}
