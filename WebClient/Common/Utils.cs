using PizzaNetCommon.Requests;
using PizzaNetControls.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;
using System.Web.Mvc;

namespace PizzaWebClient.Common
{
    public static class Utils
    {
        public static ActionResult HandleFaults(Exception exc)
        {
            if (exc is FaultException<PizzaServiceFault>)
                return new ViewResult() { ViewName = (exc as FaultException<PizzaServiceFault>).Detail.Reason };
            else if (exc is PizzaNetException)
                return new ViewResult() { ViewName = (exc as PizzaNetException).Message };
            else if (exc is System.TimeoutException)
                return new ViewResult() { ViewName = PizzaNetControls.Common.Utils.Messages.TIMED_OUT };
            else
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.InternalServerError);
        }
    }
}