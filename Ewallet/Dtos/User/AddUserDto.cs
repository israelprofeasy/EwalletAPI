using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Ewallet.Dtos.User
{
    public class AddUserDto
    {
        [Required(ErrorMessage = "Firstname is required"),
        MaxLength(50, ErrorMessage = "The First name is too long"),
        Display(Name = "First name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Lastname is required"),
        MaxLength(50, ErrorMessage = "The Last name is too long"),
        Display(Name = "Last name")]
        public string LastName { get; set; }

        
        [Required(ErrorMessage = "Username is required"),
        MaxLength(50, ErrorMessage = "The username is too long"),
        Display(Name = "Username")]
        public string UserName { get; set; }
        

        [Required(ErrorMessage = "Email is required"),
        MaxLength(60, ErrorMessage = "The email is too long"),
        EmailAddress(ErrorMessage = "The email address is invalid"),
        Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required"),
        RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[^\\da-zA-Z]).{8,15}$",
        ErrorMessage = "The password must be between 8 and 15 characters long and contain at " +
        "least one uppercase, one lowercase, one number and one symbol"),
        Display(Name = "Password")]
        public string Password { get; set; }

        /*
        [Required(ErrorMessage = "Confirm Password is required"),
        Compare(nameof(Password), ErrorMessage = "Passwords do not match"),
        Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }
        */
        public string RoleName { get; set; }
    }
}
