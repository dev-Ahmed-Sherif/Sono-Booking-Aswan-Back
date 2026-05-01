using System.Collections.Generic;

namespace SonoTracker.Common.DTO.Identity.User
{
    public record UserDataDto(string Id, 
                              string Name, 
                              string Role, 
                              List<UserPermissionDto> Permissions, 
                              string OrganizationId,
                              string FloatingUnitId,
                              string GovernorateId);
}
