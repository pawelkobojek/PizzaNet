using Moq;
using PizzaNetCommon.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace PizzaNetTests.Mocks
{
    public class FakeControllerSession : HttpSessionStateBase
    {
        Dictionary<string, object> m_SessionStorage;
        public FakeControllerSession()
        {
            m_SessionStorage = new Dictionary<string, object>();
            m_SessionStorage.Add("LoggedIn", true);
            m_SessionStorage.Add("User", new UserDTO()
                {
                    Email = "Admin",
                    Password = "123",
                    Address = "Address",
                    Phone = 999,
                    Name = "AdminName"
                });
        }

        public override object this[string name]
        {
            get { return m_SessionStorage[name]; }
            set { m_SessionStorage[name] = value; }
        }

        public override void Clear()
        {
        }

        public static void SetFakeControllerContext(Controller controller, bool isAjax = false)
        {
            var routes = new RouteCollection();

            var request = new Mock<HttpRequestBase>();
            // Not working - IsAjaxRequest() is static extension method and cannot be mocked
            // request.Setup(x => x.IsAjaxRequest()).Returns(true /* or false */);
            // use this
            if (isAjax)
            {
                request.SetupGet(x => x.Headers).Returns(
                    new System.Net.WebHeaderCollection 
                    {
                        {"X-Requested-With", "XMLHttpRequest"}
                    });
            }

            request.SetupGet(r => r.HttpMethod).Returns("GET");
            request.SetupGet(r => r.ApplicationPath).Returns("/");
            request.SetupGet(r => r.Url).Returns(new Uri("http://localhost/app", UriKind.Absolute));
  
            var response = new Mock<HttpResponseBase>(MockBehavior.Strict);
            response.Setup(r => r.ApplyAppPathModifier(It.IsAny<string>())).Returns((String url) => url);

            var hcontext = new Mock<HttpContextBase>();
            hcontext.SetupGet(x => x.Request).Returns(request.Object);
            hcontext.SetupGet(x => x.Response).Returns(response.Object);
            var session = new FakeControllerSession();
            hcontext.SetupGet(x => x.Session).Returns(session);

            var context = new Mock<ControllerContext>();
            context.SetupGet(x => x.HttpContext).Returns(hcontext.Object);
            controller.ControllerContext = context.Object;
            controller.Url = new UrlHelper(new RequestContext(hcontext.Object, new RouteData()), routes);
        }
    }
}
