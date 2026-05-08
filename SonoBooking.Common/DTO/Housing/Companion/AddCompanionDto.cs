using SonoBooking.Common.Core;
using SonoBooking.Domain;
using System;
using System.Diagnostics.CodeAnalysis;

namespace SonoBooking.Common.DTO.Housing.Companion
{
    [ExcludeFromCodeCoverage]
    public class AddCompanionDto : IEntityDto<string>
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
    }
}
