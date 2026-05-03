using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Identity;
using SonoTracker.Common.Core;
using SonoTracker.Common.DTO.Base;
using SonoTracker.Common.DTO.Identity.Role;
using SonoTracker.Domain;
namespace SonoTracker.Application.Services.Identity.Role
{

    public class RoleService(RoleManager<SonoBooking.Domain.Entities.Identity.Role> roleManager) : IRoleService
    {
        public PagingResult GetAllPagedAsync(BaseParam<FilterRoleDto> filter)
        {
            var limit = filter.PageSize;

            var offset = --filter.PageNumber * filter.PageSize;

            var filterRoles = roleManager.Roles.Where
                                (r =>
                                    r.Name.Equals(filter.Filter.NameEn) ||
                                    r.NormalizedName.Equals(filter.Filter.NameAr) ||
                                    r.Id.Equals(filter.Filter.Id)
                                );

            var roles = filterRoles.ToList().Count > 0 ? filterRoles : roleManager.Roles;

            var dataRes = roles.Select(r => new RoleDto
            {
                Id = r.Id,
                NameAr = r.NameAr ?? "",
                NameEn = r.Name,
            });

            return new PagingResult(filter.PageNumber, filter.PageSize, dataRes.ToList().Count, dataRes, status: HttpStatusCode.OK, MessagesConstants.Success);
        }
    }
}
