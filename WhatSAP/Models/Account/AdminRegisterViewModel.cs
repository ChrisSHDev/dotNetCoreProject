using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WhatSAP.Models.Account
{
    public class AdminRegisterViewModel
    {
        [Required]
        [Display(Name = "Username")]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(200, MinimumLength = 4, ErrorMessage = "Password must be at least 4 characters long!")]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Passwords must match")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }

    }
}
