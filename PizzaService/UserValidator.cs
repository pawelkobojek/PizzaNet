using System;
using System.Collections.Generic;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Web;
using PizzaNetDataModel.Model;
using PizzaNetDataModel.Repository;

namespace PizzaService
{
    public class UserValidator : UserNamePasswordValidator
    {
        public override void Validate(string userName, string password)
        {
            User user = null;
            using (var db = new PizzaUnitOfWork())
            {
                user = db.inTransaction(uow =>
                    {
                        return uow.Db.Users.Find(userName);
                    });
            }
            if (user == null || password != user.Password)
            {
                throw new SecurityTokenException("Wrong username or password");
            }
        }
    }
}