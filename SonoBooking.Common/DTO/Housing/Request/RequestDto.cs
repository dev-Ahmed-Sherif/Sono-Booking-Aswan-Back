using SonoBooking.Common.Core;
using SonoBooking.Domain;
using System;
using System.Diagnostics.CodeAnalysis;

namespace SonoBooking.Common.DTO.Housing.Request
{
    [ExcludeFromCodeCoverage]
    public class RequestDto : IEntityDto<string>
    {
        public string Id { get; set; }
        public string RequestNumber { get; set; }
        public DateTime RequestDate { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public Status Status { get; set; }
        public AllocationType RequestAllocationType { get; set; }
        public RequestCatagory RequestCatagory { get; set; }
        public string? ReservationId { get; set; }
        public string RejectionReason { get; set; }
        public string RequestTypeId { get; set; }
        public string RequestType { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string? ApprovedById { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedById { get; set; }
        public string CreatedBy { get; set; }
        public DateTime ModifiedAt { get; set; }
        public string ModifiedById { get; set; }
        public string ModifiedBy { get; set; }
        public bool IsDeleted { get; set; }
    }
}
