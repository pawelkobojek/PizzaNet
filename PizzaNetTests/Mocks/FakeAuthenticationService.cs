using Moq;
using PizzaWebClient.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaNetTests.Mocks
{
    public class FakeAuthenticationService : IAuthenticationService
    {
        public void SetAuthCookie(string userName, bool createPersistentCookie)
        {
        }

        public string CreateUserAndAccount(string userName, string password)
        {
            throw new NotImplementedException();
        }

        public string CreateAccount(string userName, string password)
        {
            throw new NotImplementedException();
        }

        public bool Login(string userName, string password)
        {
            throw new NotImplementedException();
        }

        public int GetUserId(string userName)
        {
            throw new NotImplementedException();
        }

        public bool ChangePassword(string userName, string currentPassword, string newPassword)
        {
            throw new NotImplementedException();
        }
    }
}
