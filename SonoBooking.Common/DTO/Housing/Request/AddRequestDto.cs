using SonoBooking.Common.Core;
using SonoBooking.Common.DTO.Housing.RequestAttach;
using SonoBooking.Common.DTO.Housing.RequestParticipant;
using SonoBooking.Common.DTO.Housing.RequestUnit;
using SonoBooking.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace SonoBooking.Common.DTO.Housing.Request
{
    [ExcludeFromCodeCoverage]
    public class AddRequestDto : IEntityDto<string>
    {
        public string Id { get; set; }
        public string RequestNumber { get; set; }
        public DateTime RequestDate { get; set; }
        [Required]
        public required DateOnly StartDate { get; set; }
        [Required]
        public required int Nights { get; set; }
        [Required]
        public required string RequestTypeId { get; set; }
        [Required]
        public required string RequestToId { get; set; }
        [Required]
        public required AllocationType RequestAllocationType { get; set; }
        [Required]
        public required RequestCatagory RequestCatagory { get; set; }
        public string? PreviousRequestId { get; set; }
        public Status? Status { get; set; }
        public string RejectionReason { get; set; }
        [Required]
        public required float Percentage { get; set; }
        public string ApprovedById { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public ICollection<AddRequestUnitDto> RequestUnits { get; set; }
        public ICollection<AddRequestParticipantDto> RequestCompanions { get; set; }
        public List<AddRequestAttachDto> Images { get; set; }
        public List<AddRequestAttachDto> OldImages { get; set; }
    }
}
