using SonoBooking.Common.Core;
using SonoBooking.Domain;
using System;
using System.Diagnostics.CodeAnalysis;

namespace SonoBooking.Common.DTO.Housing.Companion
{
    [ExcludeFromCodeCoverage]
    public class CompanionDto : IEntityDto<string>
    {
        public string Id { get; set; }
        public string Code { get; set; }
        public string FullName { get; set; }
        public Gender Gender { get; set; }
        public DateOnly BirthDate { get; set; }
        public string DocumentImageUrl { get; set; }
        public string DocumentNumber { get; set; }
        public IDType DocumentType { get; set; }
        public string UserId { get; set; }
        public string RelationshipId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedById { get; set; }
        public string CreatedBy { get; set; }
        public DateTime ModifiedAt { get; set; }
        public string ModifiedById { get; set; }
        public string ModifiedBy { get; set; }
        public bool IsDeleted { get; set; }
    }
}
