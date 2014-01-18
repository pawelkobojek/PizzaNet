using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaWebClient.Common
{
    public interface IAuthenticationService
    {
        void SetAuthCookie(string userName, bool createPersistentCookie);
        string CreateUserAndAccount(string userName, string password);

        string CreateAccount(string userName, string password);

        bool Login(string userName, string password);
        
        int GetUserId(string userName);
        
        bool ChangePassword(string userName, string currentPassword, string newPassword);
    }
}
