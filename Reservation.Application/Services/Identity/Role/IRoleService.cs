using SonoTracker.Common.Core;
using SonoTracker.Common.DTO.Base;
using SonoTracker.Common.DTO.Identity.Role;
using System.Threading.Tasks;

namespace SonoTracker.Application.Services.Identity.Role
{
    public interface IRoleService
    {
        PagingResult GetAllPagedAsync(BaseParam<FilterRoleDto> filter);
    }
}
