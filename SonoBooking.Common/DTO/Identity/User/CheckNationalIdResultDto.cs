namespace SonoBooking.Common.DTO.Identity.User
{
    public class CheckNationalIdResultDto
    {
        public bool Exists { get; set; }

        public bool IsEmployee { get; set; }

        public string? EmployeeId { get; set; }
    }
}
