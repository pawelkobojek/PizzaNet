using PizzaNetCommon.Requests;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using System.Xml.Serialization;

namespace PizzaNetControls.Common
{
    public static class Utils
    {
        public const string TITLE = "PizzaNet";

        public static void showError(string message)
        {
            MessageBox.Show(message, TITLE, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public static void showExclamation(string message)
        {
            MessageBox.Show(message, TITLE, MessageBoxButton.OK, MessageBoxImage.Exclamation);
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
                showError("Unknown error");
        }
    }
}
