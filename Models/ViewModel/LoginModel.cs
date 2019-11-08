using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityApi.Models.ViewModel
{
    public class LoginModel
    {
        [Required(ErrorMessage = "UserName cannot be empty")]
        [MinLength(8, ErrorMessage = "Minimum 8 character required")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password cannot be empty")]
        [MinLength(8, ErrorMessage = "Minimum 8 character required")]
        public string Password { get; set; }
    }
}
