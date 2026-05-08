using SonoBooking.Common.Core;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace SonoBooking.Common.DTO.Tracker.AccidentOrganization
{
    [ExcludeFromCodeCoverage]

    public class AddAccidentOrganizationDto : IEntityDto<string>
    {
        public string Id { get; set; }

        [Required, MaxLength(50)]
        public required string OrganizationId { get; set; }
        
        [Required, MaxLength(50)]
        public required string AccidentId { get; set; }

    }
}


