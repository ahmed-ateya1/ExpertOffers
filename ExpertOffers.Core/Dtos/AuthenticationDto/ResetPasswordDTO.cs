﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertOffers.Core.DTOS.AuthenticationDTO
{
    public class ResetPasswordDTO
    {
        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&#])[A-Za-z\d@$!%*?&#]{8,}$",
            ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character.")]
        public string? Password { get; set; }

        [Compare("Password", ErrorMessage = "Password and confirmation password do not match.")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long.")]
        public string? ConfirmPassword { get; set; }

        public string? Email { get; set; }

        [Required]
        public string? Otp { get; set; }
    }
}

