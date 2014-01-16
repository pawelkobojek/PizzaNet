using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PizzaWebClient.Models.ViewModels
{
    public class UserViewModel
    {
        public int UserID { get; set; }

        [StringLength(50, MinimumLength = 3)]
        public string Name { get; set; }

        public int Phone { get; set; }

        [Required(ErrorMessage = "Address is required")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [RegularExpression(@"^[\w!#$%&'*+\-/=?\^_`{|}~]+(\.[\w!#$%&'*+\-/=?\^_`{|}~]+)*"
                                   + "@"
                                   + @"((([\-\w]+\.)+[a-zA-Z]{2,4})|(([0-9]{1,3}\.){3}[0-9]{1,3}))$")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(50, MinimumLength = 3)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "This is required")]
        [StringLength(50, MinimumLength = 3)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "This is required")]
        [StringLength(50, MinimumLength = 3)]
        [Compare("NewPassword")]
        [DataType(DataType.Password)]
        [Display(Name = "New password again")]
        public string NewPasswordAgain { get; set; }

    }
}