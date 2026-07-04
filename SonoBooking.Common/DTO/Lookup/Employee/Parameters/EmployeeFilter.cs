using System.Diagnostics.CodeAnalysis;

namespace SonoBooking.Common.DTO.Lookup.Employee.Parameters
{
    [ExcludeFromCodeCoverage]
    public class EmployeeFilter
    {
        public string Name { get; set; }
        public string NationalId { get; set; }
        public string EmployeeOrgId { get; set; }
        public string EmployeeJobId { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
