﻿using System.ComponentModel.DataAnnotations;

namespace NextStopApp.DTOs
{
    public class RefreshTokenRequest
    {
        [Required]
        public string RefreshToken { get; set; }
    }
}
