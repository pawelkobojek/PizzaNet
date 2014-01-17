using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PizzaWebClient.Models.ViewModels
{
    public class PasswordViewModel
    {
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