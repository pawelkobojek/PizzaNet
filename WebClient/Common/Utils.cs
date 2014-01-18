using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PizzaWebClient.Common
{
    public static class Utils
    {
        public static ActionResult HandleFaults(Exception e)
        {
            return new HttpNotFoundResult();
        }
    }
}