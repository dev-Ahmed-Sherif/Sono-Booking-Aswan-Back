using SonoBooking.Common.Core;
using SonoBooking.Domain;
using System;
using System.Diagnostics.CodeAnalysis;

namespace SonoBooking.Common.DTO.Housing.Extension
{
    [ExcludeFromCodeCoverage]
    public class AddExtensionDto : IEntityDto<string>
    {
        public string Id { get; set; }
        public DateOnly EndDate { get; set; }
        public Status Status { get; set; } = Status.Pending;
        public AllocationType ExtensionAllocationType { get; set; }
        public string? RejectionReason { get; set; }
        public string ReservationId { get; set; }
        public string UserId { get; set; }
        public string? ApprovedById { get; set; }
        public DateTime? ApprovedAt { get; set; }
    }
}
