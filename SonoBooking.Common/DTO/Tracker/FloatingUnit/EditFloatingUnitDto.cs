using SonoTracker.Common.Core;
using SonoTracker.Common.DTO.Base;
using SonoTracker.Domain.Enum;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace SonoTracker.Common.DTO.Tracker.FloatingUnit
{
    [ExcludeFromCodeCoverage]
    public class EditFloatingUnitDto : LookupDto<string>
    {
        public string LicenseNumber { get; set; }
        public float Length { get; set; }
        public float Width { get; set; }
        public int PassengerNumber { get; set; }
        public int RoomNumber { get; set; }
        public DateOnly ManufactureYear { get; set; }
        public DateOnly LastMaintenanceDate { get; set; }
        public DateOnly NextMaintenanceDate { get; set; }
        public UnitCategory UnitCategory { get; set; }
        public string UnitTypeId { get; set; }
        public string UnitType { get; set; }
        public string ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedById { get; set; }
        public DateTime ModifiedAt { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedById { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsAccepted { get; set; }
    }
}
