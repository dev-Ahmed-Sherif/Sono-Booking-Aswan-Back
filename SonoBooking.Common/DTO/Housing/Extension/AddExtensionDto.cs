using SonoBooking.Common.Core;
using SonoBooking.Domain;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace SonoBooking.Common.DTO.Housing.Extension
{
    [ExcludeFromCodeCoverage]
    public class AddExtensionDto : IEntityDto<string>
    {
        public string Id { get; set; }
        [Required]
        public required DateOnly EndDate { get; set; }
        public required Status Status { get; set; } = Status.Pending;
        public required AllocationType ExtensionAllocationType { get; set; }
        public string RejectionReason { get; set; }
        public required string ReservationId { get; set; }
        public required string UserId { get; set; }
        public string ApprovedById { get; set; }
        public DateTime ApprovedAt { get; set; }
    }
}
