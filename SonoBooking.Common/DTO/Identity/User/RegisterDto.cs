using Microsoft.AspNetCore.Http;
using SonoBooking.Domain;
using System;
using System.ComponentModel.DataAnnotations;

namespace SonoBooking.Common.DTO.Identity.User
{
    public class RegisterDto
    {
        [Required, MaxLength(70)]
        public required string Username { get; set; }
        [Required, MaxLength(20)]
        public required string NationalId { get; set; }
        [Required]
        public required IDType DocumentType { get; set; }
        [Required]
        public Gender Gender { get; set; }
        [Required]
        public required DateOnly BirthDate { get; set; }
        public required string Phone { get; set; }
        [Required, EmailAddress]
        public required string Email { get; set; }
        [Required]
        public required string Password { get; set; }
        [Required]
        public required IFormFile DocumentImage { get; set; }
        public string RoleId { get; set; }
    }
}

