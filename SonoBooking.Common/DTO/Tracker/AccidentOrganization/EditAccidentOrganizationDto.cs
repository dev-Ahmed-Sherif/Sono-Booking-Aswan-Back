using SonoBooking.Common.Core;
using System;
using System.Diagnostics.CodeAnalysis;

namespace SonoBooking.Common.DTO.Tracker.AccidentOrganization
{
    [ExcludeFromCodeCoverage]

    public class EditAccidentOrganizationDto : IEntityDto<string>
    {
        public string Id { get; set; }

        public string OrganizationId { get; set; }
        public string OrganizationNameAr { get; set; }
        public string OrganizationNameEn { get; set; }

        public string AccidentId { get; set; }

        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedById { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime ModifiedAt { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedById { get; set; }
    }
}


