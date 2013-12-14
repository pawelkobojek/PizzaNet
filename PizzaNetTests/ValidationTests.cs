using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PizzaNetControls.Common;

namespace PizzaNetTests
{
    [TestClass]
    public class ValidationTests
    {
        [TestMethod]
        public void EmailValidationTest()
        {
            string email = "malpa";
            Assert.IsFalse(Utils.IsEmailValid(email));

            email = "malpa@";
            Assert.IsFalse(Utils.IsEmailValid(email));

            email = "malpa@malpa";
            Assert.IsFalse(Utils.IsEmailValid(email));

            email = "malpa@";
            Assert.IsFalse(Utils.IsEmailValid(email));

            email = "malpa@malpa.pl";
            Assert.IsTrue(Utils.IsEmailValid(email));

            email = "malpa@malpa..pl";
            Assert.IsFalse(Utils.IsEmailValid(email));

            email = ".malpa@malpa.pl";
            Assert.IsFalse(Utils.IsEmailValid(email));

            email = "malpa.malpa@malpa.com.pl";
            Assert.IsTrue(Utils.IsEmailValid(email));

            email = "malpa.malpa@malpa@malpa.com.pl";
            Assert.IsFalse(Utils.IsEmailValid(email));

            email = "malpa.malpa@@malpa.com.pl";
            Assert.IsFalse(Utils.IsEmailValid(email));

            email = "malpa.malpa.@.malpa.com.pl";
            Assert.IsFalse(Utils.IsEmailValid(email));

            email = "malpa.malpa@malpa.com..pl";
            Assert.IsFalse(Utils.IsEmailValid(email));

            email = "malpa.malpa@malpa.com. .pl";
            Assert.IsFalse(Utils.IsEmailValid(email));
        }
    }
}
