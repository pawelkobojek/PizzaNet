using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using WebMatrix.WebData;

namespace PizzaWebClient.Common
{
    public class AuthenticationService : IAuthenticationService
    {
        public void SetAuthCookie(string userName, bool createPersistentCookie)
        {
            FormsAuthentication.SetAuthCookie(userName, createPersistentCookie);
        }

        public string CreateUserAndAccount(string userName, string password)
        {
            return WebSecurity.CreateUserAndAccount(userName, password);
        }

        public bool Login(string userName, string password)
        {
            return WebSecurity.Login(userName, password);
        }

        public int GetUserId(string userName)
        {
            return WebSecurity.GetUserId(userName);
        }
        
        public bool ChangePassword(string userName, string currentPassword, string newPassword)
        {
            return WebSecurity.ChangePassword(userName, currentPassword, newPassword);
        }

        public string CreateAccount(string userName, string password)
        {
            return WebSecurity.CreateAccount(userName, password);
        }
    }
}