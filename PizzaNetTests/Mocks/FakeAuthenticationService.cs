using Moq;
using PizzaNetCommon.DTOs;
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
        public List<UserDTO> AccountsCreated { get; set; }

        public void SetAuthCookie(string userName, bool createPersistentCookie)
        {
        }

        public string CreateUserAndAccount(string userName, string password)
        {
            AccountsCreated = new List<UserDTO>();
            AccountsCreated.Add(new UserDTO() { Email = userName, Password = password });
            return null;
        }

        public string CreateAccount(string userName, string password)
        {
            AccountsCreated = new List<UserDTO>();
            AccountsCreated.Add(new UserDTO() { Email = userName, Password = password });
            return null;
        }

        public bool Login(string userName, string password)
        {
            if (userName != "Admin" || password != "123")
                return false;
            return true;
        }

        public int GetUserId(string userName)
        {
            throw new NotImplementedException();
        }

        public bool ChangePassword(string userName, string currentPassword, string newPassword)
        {
            throw new NotImplementedException();
        }


        public void Logout()
        {

        }
    }
}
