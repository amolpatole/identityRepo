using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityApi.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage ="Name cannot be empty")]
        public string Name { get; set; }

        [Required(ErrorMessage = "UserName cannot be empty")]
        [MinLength(8, ErrorMessage = "Minimum 8 character required")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password cannot be empty")]
        [MinLength(8, ErrorMessage = "Minimum 8 character required")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Email cannot be empty")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        public string Role { get; set; }

        public string Status { get; set; }
    }
}
