using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaNetCommon.DTOs
{
    public class UserDTO
    {
        public int UserID { get; set; }

        [StringLength(50, MinimumLength = 3)]
        public string Name { get; set; }
        public int Phone { get; set; }

        [Required(ErrorMessage = "Address is required")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(50, MinimumLength = 3)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public int Rights { get; set; }
    }
}
