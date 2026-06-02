using SonoBooking.Common.Core;
using SonoBooking.Common.DTO.Base;
using SonoBooking.Common.DTO.Identity.Roles;

namespace SonoBooking.Application.Services.Identity.Roles
{
    public interface IRoleService
    {
        PagingResult GetAllPagedAsync(BaseParam<FilterRoleDto> filter);
    }
}

