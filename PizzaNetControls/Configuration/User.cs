using PizzaNetCommon.DTOs;
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
    public class User : IXmlSerializable
    {
        public User()
        {
            UserID = -1;
            Name = "";
            Phone = 0;
            Email = "";
            Address = "";
            Password = "";
            Rights = 0;
        }

        public static User FromUserDTO(UserDTO user)
        {
            return new User()
            {
                UserID = user.UserID,
                Email = user.Email,
                Password = "",
                Name = user.Name,
                Phone = user.Phone,
                Rights = user.Rights,
                Address = user.Address
            };
        }

        public string Email { get; set; }

        [XmlIgnore]
        public int UserID { get; set; }
        [XmlIgnore]
        public string Name { get; set; }
        [XmlIgnore]
        public int Phone { get; set; }
        [XmlIgnore]
        public string Address { get; set; }
        [XmlIgnore]
        public string Password { get; set; }
        [XmlIgnore]
        public int Rights { get; set; }

        public XmlNode getNode()
        {
            XmlSerializer xs = new XmlSerializer(typeof(User));
            StringWriter xout = new StringWriter();
            xs.Serialize(xout, this);
            XmlDocument x = new XmlDocument();
            x.LoadXml(xout.ToString());
            return x.SelectSingleNode("User");
        }

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            if (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "User")
            {
                Email = reader["Email"];
                reader.Read();
            }
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteAttributeString("Email", Email);
        }
    }
}
