using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RecordManagementSystemClientSide.DTO
{
    public class LoginDTO
    {
        [EmailAddress(ErrorMessage = "Incorrect")]
        public string email { get; set; }
        
        [Required(ErrorMessage = "Please input your password")]
        public string password { get; set; }
    }
}