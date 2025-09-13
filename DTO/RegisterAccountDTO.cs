using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace RecordManagementSystemClientSide.DTO
{
    public class RegisterAccountDTO
    {
        [Required(ErrorMessage = "Firstname is required")]
        public string FirstName { get; set; }


        [Required(ErrorMessage = "Middlename is required")]
        public string Middlename { get; set; }


        [Required(ErrorMessage = "Lastname is required")]
        public string LastName { get; set; }


        [Required(ErrorMessage = "Gender is required")]
        public string Gender { get; set; }


        [Required(ErrorMessage = "Year of birth is required")]
        public string YearOfBirth { get; set; }


        [Required(ErrorMessage = "Month of birth is required")]
        public string MonthOfBirth { get; set; }


        [Required(ErrorMessage = "Date of birth is required")]
        public string DateOfBirth { get; set; }


        [Required(ErrorMessage = "Home address is required")]
        public string HomeAddress { get; set; }


        [Required(ErrorMessage = "Mobile number is required")]
        public string MobileNumber { get; set; }


        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public string Email { get; set; }


        [Required(ErrorMessage = "Program is required")]
        public string Program { get; set; }


        [Required(ErrorMessage = "Year level is required")]
        public string YearLevel { get; set; }


        [Required(ErrorMessage = "Student Id is required")]
        public string StudentID { get; set; }


        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }
}