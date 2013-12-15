using PizzaNetCommon.Requests;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using System.Xml.Serialization;

namespace PizzaNetControls.Common
{
    public static class Utils
    {
        public const string TITLE = "PizzaNet";

        public static bool IsEmailValid(string email)
        {
            const string theEmailPattern = @"^[\w!#$%&'*+\-/=?\^_`{|}~]+(\.[\w!#$%&'*+\-/=?\^_`{|}~]+)*"
                                   + "@"
                                   + @"((([\-\w]+\.)+[a-zA-Z]{2,4})|(([0-9]{1,3}\.){3}[0-9]{1,3}))$";
            return Regex.IsMatch(email, theEmailPattern);
        }

        public static bool IsNumber(string text)
        {
            Regex regex = new Regex("^[0-9]+$");
            return regex.IsMatch(text);
        }

        public static void showError(string message)
        {
            MessageBox.Show(message, TITLE, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public static void showExclamation(string message)
        {
            MessageBox.Show(message, TITLE, MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }

        public static void showInformation(string message)
        {
            MessageBox.Show(message, TITLE, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public static bool showChangesDialog()
        {
            return MessageBox.Show
                    (Messages.DISCARD_CHANGES_QUESTION, TITLE,
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Exclamation,
                        MessageBoxResult.No
                    ) != MessageBoxResult.No;
        }

        public static class Messages
        {
            public const string REGISTRATION_FAILED = "Registration failed!";
            public const string REGISTRATION_COMPLETED = "Registration completed!";
            public const string UNKNOWN_ERROR = "Unknown error!";
            public const string ORDERED_SUCCESSFULLY = "Ordered successfully!";
            public const string ORDERING_ERROR = "Error while ordering!";
            public const string ORDERS_REFRESH_FAILED = "Refreshing orders failed!";
            public const string DISCARD_CHANGES_QUESTION = "You have unsaved changes. Do you want to discard them?";
            public const string RECIPES_REFRESH_FAILED = "Refreshing recipes failed!";
            public const string NO_USER_LOGGED_IN = "No user is logged in!";
            public const string NO_USER_FOUND_FORMAT = "No user {0} found in database!";
            public const string INVALID_PASSWORD = "Inserted password is not valid!";
            public const string PASSWORD_EMPTY = "Password cannot be empty!";
            public const string REPEATED_PASSWORD_DIFF = "Repeated password is diffrent than new password!";
        }

        public static XmlNode SerializeObjectToXmlNode(Object obj)
        {
            if (obj == null)
                throw new ArgumentNullException("Argument cannot be null");

            XmlNode resultNode = null;
            XmlSerializer xmlSerializer = new XmlSerializer(obj.GetType());
            using (MemoryStream memoryStream = new MemoryStream())
            {
                try
                {
                    xmlSerializer.Serialize(memoryStream, obj);
                }
                catch (InvalidOperationException)
                {
                    return null;
                }
                memoryStream.Position = 0;
                XmlDocument doc = new XmlDocument();
                doc.Load(memoryStream);
                resultNode = doc.DocumentElement;
            }
            return resultNode;
        }

        public static void HandleException(Exception exc)
        {
            if (exc is FaultException<PizzaServiceFault>)
            {
                showExclamation((exc as FaultException<PizzaServiceFault>).Detail.Reason);
            }
            else
                showError(Messages.UNKNOWN_ERROR);
        }
    }
}
