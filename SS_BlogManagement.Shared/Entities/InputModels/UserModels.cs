using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SS_BlogManagement.Shared.Entities.InputModels
{
    public partial class RegisteredUser
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public string Password { get; set; }

    }

    public partial class LoginUser
    {
        [Required]
        public string EmailOrPhone { get; set; }
        [Required]
        public string Password { get; set; }

    }
}
