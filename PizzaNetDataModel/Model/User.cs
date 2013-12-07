using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaNetDataModel.Model
{
    public class User : Entity
    {
        [Key]
        public int UserID { get; set; }
        public string Name { get; set; }
        public int Phone { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public int Rights { get; set; }
    }
}
