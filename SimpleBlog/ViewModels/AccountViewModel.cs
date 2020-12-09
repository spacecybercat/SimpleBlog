using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SimpleBlog.ViewModels
{
    public class AccountViewModel
    {
        public class LoginViewModel
        {
            [Required(ErrorMessage = "Musisz podać login")]
            public string UserName { get; set; }

            [Required(ErrorMessage = "Musisz wprowadzić hasło")]
            [DataType(DataType.Password)]
            [Display(Name = "Hasło")]
            public string Password { get; set; }

            [Display(Name = "Zapamiętaj mnie")]
            public bool RememberMe { get; set; }

        }

        public class RegisterViewModel
        {
            public string AvatarFile { get; set; }

            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [StringLength(16, ErrorMessage = "Błąd 1", MinimumLength = 3)]
            public string Nickname { get; set; }

            [Required]
            [StringLength(30, ErrorMessage = "{0} musi mieć co najmniej {2} znaków.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Hasło ")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Potwierdź hasło ")]
            [Compare("Password", ErrorMessage = "Podane hasła muszą być identyczne.")]
            public string ConfirmPassword { get; set; }

        }
    }
}