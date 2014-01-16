using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using PizzaNetCommon.DTOs;
using PizzaNetCommon.Queries;
using PizzaNetCommon.Requests;
using PizzaNetControls.WCFClientInfrastructure;
using PizzaNetWorkClient.WCFClientInfrastructure;
using WebClient.Models;

namespace WebClient.Controllers
{
    public class HomeController : Controller
    {
        IWorkChannelFactory factory = new BasicWorkChannelFactory();

        public HomeController(IWorkChannelFactory fact)
        {
            factory = fact;
        }

        public HomeController()
        {
        }

        public ActionResult Index()
        {
            if (!((bool?)this.Session["LoggedIn"] ?? false))
                return RedirectToAction("Login", "Account");


            using (var proxy = factory.GetWorkChannel())
            {
                if (Request.IsAjaxRequest())
                {

                    return null;
                }
                else
                {
                    var data = proxy.GetRecipeTabData(new PizzaNetCommon.Requests.EmptyRequest
                    {
                        Login = (string)this.Session["Email"],
                        Password = (string)this.Session["Password"]
                    });

                    return View(data);
                }
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
            if (!((bool?)this.Session["LoggedIn"] ?? false))
                return RedirectToAction("Login", "Account");

            List<OrderDTO> orders = new List<OrderDTO>();
            using (var proxy = factory.GetWorkChannel())
            {
                var result = proxy.GetOrdersForUser(new PizzaNetCommon.Requests.EmptyRequest
                {
                    Login = (string)this.Session["Email"],
                    Password = (string)this.Session["Password"]
                });
                orders = result.Data;
            }
            return View(orders);
        }

        public ActionResult GetOrderInfo(int id)
        {
            if (!((bool?)this.Session["LoggedIn"] ?? false))
                return RedirectToAction("Login", "Account");

            using (var proxy = factory.GetWorkChannel())
            {
                var result = proxy.GetOrderInfo(new OrdersQuery
                {
                    Login = (string)this.Session["Email"],
                    Password = (string)this.Session["Password"],
                    Ids = new int[] { id }
                });

                return PartialView("_ExpandedOrderInfo", result.Data[0]);
            }
        }

        public ActionResult EditProfile()
        {
            return View();
        }

        public ActionResult AddToOrder(OrderInfoDTO info)
        {
            if (!((bool?)this.Session["LoggedIn"] ?? false))
                return RedirectToAction("Login", "Account");

            if (Request.IsAjaxRequest())
            {
                using (var proxy = factory.GetWorkChannel())
                {
                    List<OrderIngredientDTO> ings = proxy.QueryIngredients(new QueryRequest<IngredientsQuery>
                    {
                        Query = new IngredientsQuery
                        {
                            Login = (string)this.Session["Email"],
                            Password = (string)this.Session["Password"],
                            IngredientIds = info.Ingredients
                        }
                    }).Data;

                    for (int i = 0; i < ings.Count; i++)
                    {
                        ings[i].Quantity = (info.Quantities[i] == "normal") ? ings[i].NormalWeight : ings[i].ExtraWeight;
                    }

                    double sizeValue = GetSize(info);

                    SizeDTO size = new SizeDTO { SizeValue = sizeValue };

                    List<OrderDetailDTO> od = new List<OrderDetailDTO>
                    {
                        new OrderDetailDTO
                        { 
                            Ingredients=ings,
                            Size=size
                        }
                    };
                    OrderDTO data = new OrderDTO
                    {
                        OrderDetailsDTO = od
                    };

                    return PartialView("_OrderList", data);
                }
            }
            return View();
        }

        [NonAction]
        private static double GetSize(OrderInfoDTO info)
        {
            double sizeValue = SizeDTO.SMALL;
            switch (info.Size)
            {
                case "normal":
                    sizeValue = SizeDTO.MEDIUM;
                    break;
                case "big":
                    sizeValue = SizeDTO.GREAT;
                    break;
                default:
                    break;
            }
            return sizeValue;
        }

        //[JsonFilter(Param = "widgets", JsonDataType = typeof(List<OrderInfo>))]
        [HttpPost]
        public ActionResult MakeOrder(List<OrderInfoDTO> info)
        {
            if (info == null || info.Count <= 0)
                return View("Error");

            if (!((bool?)this.Session["LoggedIn"] ?? false))
                return RedirectToAction("Login", "Account");

            try
            {
                using (var proxy = factory.GetWorkChannel())
                {
                    proxy.MakeOrderFromWeb(new UpdateOrRemoveRequest<List<OrderInfoDTO>>
                    {
                        Login = (string)this.Session["Email"],
                        Password = (string)this.Session["Password"],
                        DataToRemove = null,
                        Data = info
                    });
                }
            }
            catch (Exception)
            {
                return View("Error");
            }

            return View();
        }

        [HttpGet]
        public ActionResult MakeOrder()
        {
            return View();
        }


        public class JsonFilter : ActionFilterAttribute
        {
            public string Param { get; set; }
            public Type JsonDataType { get; set; }
            public override void OnActionExecuting(ActionExecutingContext filterContext)
            {
                if (filterContext.HttpContext.Request.ContentType.Contains("application/x-www-form-urlencoded"))
                {
                    string inputContent;
                    using (var sr = new StreamReader(filterContext.HttpContext.Request.InputStream))
                    {
                        inputContent = sr.ReadToEnd();
                    }
                    var result = JsonConvert.DeserializeObject(inputContent, JsonDataType);
                    filterContext.ActionParameters[Param] = result;
                }
            }
        }
    }
}
