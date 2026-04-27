using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SonoTracker.Domain.Entities.Base;
using SonoTracker.Domain.Entities.Tracker;

namespace SonoTracker.Domain.Entities.Attachments
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
        
        [Required, MaxLength(250)]
        public required string FileName { get; set; }
        
        [MaxLength(50)]
        public required string Extension { get; set; }
        
        [MaxLength(50)]   
        public required string Size { get; set; }

        [Required, MaxLength(250)]
        public required string Url { get; set; }

        public bool IsPublic { get; set; }
    }
}
