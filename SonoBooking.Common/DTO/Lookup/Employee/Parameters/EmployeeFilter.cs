using System.Diagnostics.CodeAnalysis;

namespace SonoBooking.Common.DTO.Lookup.Employee.Parameters
{
    [ExcludeFromCodeCoverage]
    public class EmployeeFilter
    {
        public string NationalId { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
