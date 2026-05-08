using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SonoBooking.Domain.Entities.Base;

namespace SonoBooking.Domain.Entities.Lookups
{
    public class Attachment : BaseEntity<string>
    {
        public Attachment()
        {
            if (string.IsNullOrEmpty(Id))
            {
                Id = Guid.CreateVersion7().ToString();
            }
        }
        
        [Required, MaxLength(70)]
        public required string FileName { get; set; }
        
        [MaxLength(50)]
        public required string Extension { get; set; }

        [Required, MaxLength(250)]
        public required string Url { get; set; }

        // public bool IsPublic { get; set; }
    }
}
