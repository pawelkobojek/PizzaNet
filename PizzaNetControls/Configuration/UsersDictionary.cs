using PizzaNetControls.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace PizzaNetControls.Configuration
{
    public class UsersDictionary : Dictionary<string, User>, IXmlSerializable
    {
        public void Add(User user)
        {
            this.Add(user.Email, user);
        }

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            if (reader.Read() && reader.NodeType == System.Xml.XmlNodeType.Element && reader.LocalName == "Users")
            {
                XmlSerializer ser = new XmlSerializer(typeof(User));
                while (reader.Read() && reader.LocalName != "Users")
                {
                    if (reader.NodeType == System.Xml.XmlNodeType.Element && reader.LocalName == "User")
                    {
                        var u = new User();
                        u = (User)ser.Deserialize(reader);
                        this.Add(u);
                    }
                }
            }
            else return;
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteStartElement("Users");
            XmlSerializer ser = new XmlSerializer(typeof(User));
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            foreach(var u in this)
            {
                ser.Serialize(writer, u.Value,ns);
            }
            writer.WriteEndElement();
        }
    }
}
