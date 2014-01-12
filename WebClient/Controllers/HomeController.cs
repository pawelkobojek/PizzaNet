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
using PizzaNetWorkClient.WCFClientInfrastructure;
using WebClient.Models;

namespace WebClient.Controllers
{
    public class HomeController : Controller
    {

        public ActionResult Index()
        {
            using (var proxy = new WorkChannel())
            {
                if (Request.IsAjaxRequest())
                {

                    return null;
                }
                else
                {

                    var data = proxy.GetRecipeTabData(new PizzaNetCommon.Requests.EmptyRequest
                    {
                        Login = "Admin",
                        Password = "123"
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
            List<OrderDTO> orders = new List<OrderDTO>();
            using (var proxy = new WorkChannel())
            {
                var result = proxy.GetOrders(new PizzaNetCommon.Requests.EmptyRequest { Login = "Admin", Password = "123" });
                orders = result.Data;
            }
            return View(orders);
        }

        public ActionResult GetOrderInfo(int id)
        {
            using (var proxy = new WorkChannel())
            {
                var result = proxy.GetOrderInfo(new OrdersQuery
                {
                    Login = "Admin",
                    Password = "123",
                    Ids = new int[] { id }
                });

                return PartialView("_ExpandedOrderInfo", result.Data[0]);
            }
        }

        public ActionResult ProfileSettings()
        {
            return View();
        }

        public ActionResult AddToOrder(OrderInfoDTO info)
        {
            if (Request.IsAjaxRequest())
            {
                using (var proxy = new WorkChannel())
                {
                    List<OrderIngredientDTO> ings = proxy.QueryIngredients(new QueryRequest<IngredientsQuery>
                    {
                        Query = new IngredientsQuery
                        {
                            Login = "Admin",
                            Password = "123",
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

            try
            {
                using (var proxy = new WorkChannel())
                {
                    proxy.MakeOrderFromWeb(new UpdateOrRemoveRequest<List<OrderInfoDTO>>
                    {
                        Login = "Admin",
                        Password = "123",
                        DataToRemove = null,
                        Data = info
                    });
                }
            }
            catch (Exception exc)
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
