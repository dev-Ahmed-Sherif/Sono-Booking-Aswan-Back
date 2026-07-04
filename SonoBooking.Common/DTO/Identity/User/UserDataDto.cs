using System.Collections.Generic;

namespace SonoBooking.Common.DTO.Identity.User
{
    public record UserDataDto(string Id, 
                              string Name, 
                              string Role, 
                              List<UserPermissionDto> Permissions, 
                              string OrganizationId,
                              string LeaderId,
                              string GovernorateId,
                              string EmployeeId);
}

