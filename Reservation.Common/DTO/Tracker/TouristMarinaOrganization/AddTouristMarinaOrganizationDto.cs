using SonoTracker.Common.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SonoTracker.Common.DTO.Tracker.TouristMarinaOrganization
{
    public class AddTouristMarinaOrganizationDto : IEntityDto<string>
    {
        public string Id { get; set; }
        
        [Required ,MaxLength(50)]
        public required string LicenseNumber { get; set; }
        public required string TouristMarinaId { get; set; }
        public required string OrganizationId { get; set; }
        public required DateOnly FromDate { get; set; }
        public required DateOnly ToDate { get; set; }
        public bool IsActive { get; set; }
    }
}
