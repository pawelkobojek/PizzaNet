using PizzaNetCommon.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace PizzaNetControls.Configuration
{
    [XmlRoot(Namespace="")]
    public class User
    {
        [XmlIgnore]
        public int UserID { get; set; }
        [XmlIgnore]
        public string Name { get; set; }
        [XmlIgnore]
        public string Password { get; set; }
        [XmlIgnore]
        public string Address { get; set; }
        [XmlIgnore]
        public int Rights { get; set; }
        [XmlIgnore]
        public int Phone { get; set; }

        [XmlAttribute]
        public string Email { get; set; }
        private int refreshRate=60;
        public int RefreshRate { get { return refreshRate;} set {if(refreshRate>0) refreshRate=value;}}

        public void UpdateWithUserDTO(UserDTO userDto)
        {
            this.UserID = userDto.UserID;
            this.Address = userDto.Address;
            this.Email = userDto.Email;
            this.Phone = userDto.Phone;
            this.Rights = userDto.Rights;
            this.Name = userDto.Name;
            this.Password = userDto.Password;
        }

        public override int GetHashCode()
        {
            return Email.GetHashCode();
        }
    }
}
