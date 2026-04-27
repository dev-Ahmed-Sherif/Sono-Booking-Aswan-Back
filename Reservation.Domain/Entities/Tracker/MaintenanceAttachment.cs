using SonoTracker.Domain.Entities.Attachments;
using SonoTracker.Domain.Entities.Base;
using SonoTracker.Domain.Entities.Lookups;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace SonoTracker.Domain.Entities.Tracker
{
    [ExcludeFromCodeCoverage]
    public class MaintenanceAttachment : BaseEntity<string>
    {
        public MaintenanceAttachment()
        {
            if (string.IsNullOrEmpty(Id))
            {
                Id = Guid.CreateVersion7().ToString();
            }
        }

        [Required, MaxLength(50)]
        [ForeignKey(nameof(Attachment))]
        public required string AttachmentId { get; set; }
        public virtual Attachment? Attachment { get; set; }
        
        [Required, MaxLength(50)]
        [ForeignKey(nameof(Maintenance))]
        public required string MaintenanceId { get; set; }
        public virtual Maintenance? Maintenance { get; set; }
    }
}
