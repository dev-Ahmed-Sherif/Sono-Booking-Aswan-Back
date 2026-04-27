using Microsoft.AspNetCore.Http;
using SonoTracker.Common.Core;
using SonoTracker.Domain.Entities.Lookups;
using SonoTracker.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SonoTracker.Common.DTO.Tracker.Maintenance
{
    [ExcludeFromCodeCoverage]

    public class AddMaintenanceDto : IEntityDto<string>
    {
        public required string Id { get; set; }
        public required string Number { get; set; }
        public required DateOnly MaintenanceDate { get; set; }
        public required DateOnly? NextMaintenanceDate { get; set; }
        public required string MaintenanceTypeId { get; set; }
        public required string FloatingUnitId { get; set; }
        public required IFormFile MaintenanceReport { get; set; }
        public List<IFormFile> Other { get; set; }
        public string Notes { get; set; }
       
    }
}
