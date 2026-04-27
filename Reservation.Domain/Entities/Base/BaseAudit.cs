using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace SonoTracker.Domain.Entities.Base
{
    [ExcludeFromCodeCoverage]
    public class BaseAudit<TKey> : BaseEntity<TKey>
    {   
        public required DateTime CreatedAt { get; set; }
        
        [MaxLength(50)]
        public required string CreatedById { get; set; }
        
        [MaxLength(70)]
        public required string CreatedBy { get; set; }
        
        public required DateTime ModifiedAt { get; set; }
        
        [MaxLength(50)]
        public required string ModifiedById { get; set; }
        
        [MaxLength(70)]
        public required string ModifiedBy { get; set; }
        
        [MaxLength(28)]
        public string? IpAddress { get; set; }
    }
}