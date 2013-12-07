using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaNetCommon.DTOs
{
    public class UserDTO
    {
        public int UserID { get; set; }
        public string Name { get; set; }
        public int Phone { get; set; }
        public string Email { get; set; }
        public int Rights { get; set; }
    }
}
