using Microsoft.AspNetCore.Http;
using SonoTracker.Common.Core;
using SonoTracker.Common.DTO.Tracker.InspectionFloatingUnitClause;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SonoTracker.Common.DTO.Tracker.Inspection
{
    [ExcludeFromCodeCoverage]
    public class AddInspectionDto : IEntityDto<string>
    {
        public string Id { get; set; }
        public DateOnly InspectionDate { get; set; }
        public string InspectionTypeId { get; set; }
        public bool SaftyPetroleumWaste { get; set; }
        public bool RightWasteDisposal { get; set; }
        public string Note { get; set; }
        public required string FloatingUnitId { get; set; }
        public required string OrganizationId { get; set; }
        public string GovernorateId { get; set; }
        public List<IFormFile> InspectionAttachment { get; set; }
        public ICollection<AddInspectionFloatingUnitClauseDto> InspectionFloatingUnitClauses { get; set; }
    }
}
