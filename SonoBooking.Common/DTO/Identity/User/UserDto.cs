using SonoBooking.Domain;
using System;
using System.ComponentModel.DataAnnotations;

namespace SonoBooking.Common.DTO.Identity.User
{
    public class UserDto
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Phone { get; set; }
        public Gender Gender { get; set; }
        public DateOnly BirthDate { get; set; }
        public string DocumentNumber { get; set; }
        public IDType DocumentType { get; set; }
        public string DocumentImageUrl { get; set; }
        public string LeaderId { get; set; }
        public string LeaderName { get; set; }
        public string JobTitle { get; set; }
        public string Organization { get; set; }
        public string RoleId { get; set; }
        public string Role { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedById { get; set; }
        public DateTime ModifiedAt { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedById { get; set; }
    }
}

