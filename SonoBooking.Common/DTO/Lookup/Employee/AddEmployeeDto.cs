using SonoBooking.Common.Core;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace SonoBooking.Common.DTO.Lookup.Employee
{
    [ExcludeFromCodeCoverage]
    public class AddEmployeeDto : IEntityDto<string>
    {
        public string Id { get; set; }

        [Required, MaxLength(20)]
        public string NationalId { get; set; }
    }
}
