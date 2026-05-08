using Microsoft.AspNetCore.Http;
using SonoBooking.Domain;
using SonoBooking.Common.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SonoBooking.Common.DTO.Tracker.Accident
{
    [ExcludeFromCodeCoverage]
    public class AddAccidentDto : IEntityDto<string>
    {
        public string Id { get; set; }
        public string Code { get; set; }
        public string CityId { get; set; }
        public string GeoPointId { get; set; }
        public DateOnly AccidentDate { get; set; }
        public DateOnly? ResponseDate { get; set; }
        public string AccidentTypeId { get; set; }
        public string FloatingUnitId { get; set; }
        public Case Case { get; set; }
        public string OrganizationId { get; set; }
        public string Notes { get; set; }
        public List<IFormFile> Attach { get; set; }
        public ICollection<string> Organizations { get; set; }
    }
}


