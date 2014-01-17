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
using PizzaWebClient.Models;
using PizzaWebClient.Models.ViewModels;

namespace PizzaWebClient.Controllers
{
    public class HomeController : Controller
    {
        IWorkChannelFactory factory = new BasicWorkChannelFactory();

        public HomeController()
        {

        }

        public HomeController(IWorkChannelFactory fact)
        {
            factory = fact;
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
                        //Login = (string)this.Session["Email"],
                        //Password = (string)this.Session["Password"]
                        Login = ((UserDTO)this.Session["User"]).Email,
                        Password = ((UserDTO)this.Session["User"]).Password
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
                    Login = ((UserDTO)this.Session["User"]).Email,
                    Password = ((UserDTO)this.Session["User"]).Password
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
                    Login = ((UserDTO)this.Session["User"]).Email,
                    Password = ((UserDTO)this.Session["User"]).Password,
                    Ids = new int[] { id }
                });

                return PartialView("_ExpandedOrderInfo", result.Data[0]);
            }
        }

        [HttpGet]
        public ActionResult EditProfile()
        {
            if (!((bool?)this.Session["LoggedIn"] ?? false))
                return RedirectToAction("Login", "Account");

            UserDTO user = ((UserDTO)this.Session["User"]);
            return View(new UserViewModel
                {
                    Address = user.Address,
                    Phone = user.Phone,
                    Email = user.Email,
                    Name = user.Name
                });
        }

        [HttpPost]
        public ActionResult EditProfile(UserViewModel user)
        {
            using (var proxy = factory.GetWorkChannel())
            {
                UserDTO currUser = ((UserDTO)this.Session["User"]);
                var newUser = proxy.UpdateUser(new UpdateRequest<UserDTO>
                {
                    Login = ((UserDTO)this.Session["User"]).Email,
                    Password = ((UserDTO)this.Session["User"]).Password,
                    Data = new UserDTO
                    {
                        Address = user.Address,
                        Email = user.Email,
                        Name = user.Name,
                        Password = currUser.Password,
                        Phone = user.Phone,
                        UserID = ((UserDTO)this.Session["User"]).UserID,
                        Rights = ((UserDTO)this.Session["User"]).Rights
                    }
                });

                this.Session["User"] = newUser.Data;
                return RedirectToAction("Index");
                //return View();
            }
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
                            Login = ((UserDTO)this.Session["User"]).Email,
                            Password = ((UserDTO)this.Session["User"]).Password,
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
                        Login = ((UserDTO)this.Session["User"]).Email,
                        Password = ((UserDTO)this.Session["User"]).Password,
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

        [HttpGet]
        public ActionResult CreateComplaint()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateComplaint(string body)
        {
            using (var proxy = factory.GetWorkChannel())
            {
                proxy.CreateComplaint(new UpdateRequest<ComplaintDTO>
                {
                    Login = ((UserDTO)this.Session["User"]).Email,
                    Password = ((UserDTO)this.Session["User"]).Password,
                    Data = new ComplaintDTO { Body = body }
                });
                return RedirectToAction("Index");
            }
        }

        // Following code may be used in order to get all complaints in db
        //public string GetComps()
        //{
        //    using (var proxy = factory.GetWorkChannel())
        //    {
        //        string str = "";
        //        var res = proxy.GetComplaints(new EmptyRequest
        //            {
        //                Login = (string)this.Session["Email"],
        //                Password = (string)this.Session["Password"]
        //            }); ;

        //        foreach (var item in res.Data)
        //        {
        //            str += item.Body + "\n";
        //        }

        //        return str;
        //    }
        //}

        [HttpGet]
        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ChangePassword(PasswordViewModel viewModel)
        {
            using (var proxy = factory.GetWorkChannel())
            {
                UserDTO user = ((UserDTO)this.Session["User"]);
                var newUser = proxy.UpdateUser(new UpdateRequest<UserDTO>
                {
                    Login = user.Email,
                    Password = user.Password,
                    Data = new UserDTO
                    {
                        UserID = user.UserID,
                        Email = user.Email,
                        Name = user.Name,
                        Password = viewModel.NewPassword,
                        Phone = user.Phone,
                        Address = user.Address,
                        Rights = user.Rights
                    }
                });

                this.Session["User"] = newUser.Data;
            }

            return RedirectToAction("Index");
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
