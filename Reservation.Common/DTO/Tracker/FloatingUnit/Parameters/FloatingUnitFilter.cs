using SonoTracker.Domain.Enum;
using System;
using System.Diagnostics.CodeAnalysis;

namespace SonoTracker.Common.DTO.Tracker.FloatingUnit.Parameters
{
    [ExcludeFromCodeCoverage]

    public class FloatingUnitFilter
    {
        public string NameAr { get; set; }

        public string NameEn { get; set; }

        public string Code { get; set; }

        public string LicenseNumber { get; set; }

        public UnitCategory? unitCategory { get; set; }

        public string? UnitTypeId { get; set; }

        public bool IsDeleted { get; set; } = false;

    }
}
