using SonoBooking.Common.Core;
using SonoBooking.Common.DTO.Base;
using SonoBooking.Common.DTO.Identity.Role;
using System.Threading.Tasks;

namespace SonoBooking.Application.Services.Identity.Role
{
    public interface IRoleService
    {
        PagingResult GetAllPagedAsync(BaseParam<FilterRoleDto> filter);
    }
}

