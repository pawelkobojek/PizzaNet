using System.Net;
using System.Web;
using System.Web.Mvc;

namespace PizzaWebClient
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            ServicePointManager.ServerCertificateValidationCallback = (a, b, c, d) => { return true; };
            filters.Add(new HandleErrorAttribute());
        }
    }
}