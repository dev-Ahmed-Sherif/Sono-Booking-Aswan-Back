using SonoTracker.Common.Core;
using SonoTracker.Domain.Enum;
using System;
using System.Diagnostics.CodeAnalysis;

namespace SonoTracker.Common.DTO.Tracker.Accident
{
    [ExcludeFromCodeCoverage]
    public class EditAccidentDto : IEntityDto<string>
    {
        public string? Id { get; set; }
        public string Code { get; set; }
        public string City { get; set; }
        public string GeoPointId { get; set; }
        public DateTime AccidentDate { get; set; }
        public DateTime ResponseDate { get; set; }
        public string AccidentType { get; set; }
        public string AccidentTypeId { get; set; }
        public string FloatingUnit { get; set; }
        public string FloatingUnitId { get; set; }
        public Case Case { get; set; }
        public string Organization { get; set; }
        public string OrganizationId { get; set; }
        public string Notes { get; set; }
        public string Attach { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedById { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime ModifiedAt { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedById { get; set; }
    }
}
