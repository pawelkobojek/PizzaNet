using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace PizzaNetTests.Mocks
{
    public class FakeControllerSession : HttpSessionStateBase
    {
        Dictionary<string, object> m_SessionStorage;
        public FakeControllerSession()
        {
            m_SessionStorage = new Dictionary<string, object>();
            m_SessionStorage.Add("LoggedIn", true);
            m_SessionStorage.Add("Email", "Admin");
            m_SessionStorage.Add("Password", "123");
        }

        public override object this[string name]
        {
            get { return m_SessionStorage[name]; }
            set { m_SessionStorage[name] = value; }
        }

        public static ControllerContext GetFakeControllerContext(bool isAjax = false)
        {
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

            var hcontext = new Mock<HttpContextBase>();
            hcontext.SetupGet(x => x.Request).Returns(request.Object);
            hcontext.SetupGet(x => x.Session).Returns(new FakeControllerSession());

            var context = new Mock<ControllerContext>();
            context.SetupGet(x => x.HttpContext).Returns(hcontext.Object);

            return context.Object;
        }
    }
}
