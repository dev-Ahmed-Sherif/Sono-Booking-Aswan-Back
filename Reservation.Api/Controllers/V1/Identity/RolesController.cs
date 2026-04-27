using Asp.Versioning;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SonoTracker.Api.Controllers.V1.Base;
using SonoTracker.Application.Services.Identity.Role;
using SonoTracker.Common.Core;
using SonoTracker.Common.DTO.Base;
using SonoTracker.Common.DTO.Identity.Role;
using SonoTracker.Domain.Entities.Identity;
using System.Net;

namespace SonoTracker.Api.Controllers.V1.Identity
{
    /// <summary>
    /// Controller for managing roles in the application.
    /// </summary>

    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class RolesController(RoleManager<Role> roleManager, IRoleService roleService) : BaseController
    {
        /// <summary>
        /// Gets all roles in the application.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>A list of roles.</returns>
        
        [HttpGet("getAll")]
        public ActionResult<IFinalResult> GetAllRolesAsync(CancellationToken cancellationToken = default)
        {
            var responseResult = new ResponseResult();

            var roles = roleManager.Roles.Select(r => new RoleDto()
            {
                Id = r.Id,
                NameAr = r.NameAr,
                NameEn = r.Name ?? "",
            }).ToList();

            return Ok(responseResult.PostResult(roles, HttpStatusCode.OK, message: HttpStatusCode.OK.ToString()));
        }
       
        /// <summary>
        /// Retrieves a paginated list of users in the application.
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>
        /// An <see cref="ActionResult"/> containing a paginated list of roles if found, 
        /// or a not found response if no users exist.
        /// </returns>
        
        [HttpPost("getPaged")]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status200OK)]
        public ActionResult<IFinalResult> GetPagedRolesAsync([FromBody] BaseParam<FilterRoleDto> filter, CancellationToken cancellationToken = default)
        {
            var roles = roleService.GetAllPagedAsync(filter);
            return Ok(roles);
        }
        
        /// <summary>
        /// Gets a role by its ID.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The role with the specified ID, or a not found result if the role does not exist.</returns>
        
        [HttpGet("get/{id}")]
        public async Task<ActionResult<IFinalResult>> GetRoleByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            var responseResult = new ResponseResult();

            if (string.IsNullOrEmpty(id))
            {
                return BadRequest(responseResult.PostResult(null, HttpStatusCode.BadRequest,
                                  message: "Role Id Required"));
            }

            var role = await roleManager.FindByIdAsync(id);

            if (role == null)
            {
                return NotFound(responseResult.PostResult(null, HttpStatusCode.NotFound,
                                message: "Role not found."));
            }

            RoleDto roleDto = new()
            {
                Id = role.Id,
                NameAr = role.ConcurrencyStamp,
                NameEn = role.Name,
            };

            return Ok(responseResult.PostResult(roleDto, HttpStatusCode.OK,
                      message: HttpStatusCode.OK.ToString()));
        }

        /// <summary>
        /// Creates a new role in the application.
        /// </summary>
        /// <param name="roleDto">The role data transfer object containing role details.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>An ActionResult indicating the result of the role creation operation.</returns>
        
        [HttpPost("add")]
        public async Task<ActionResult<IFinalResult>> CreateRoleAsync([FromBody] AddRoleDto roleDto, CancellationToken cancellationToken = default)
        {
            var responseResult = new ResponseResult();

            if (roleDto == null || 
                string.IsNullOrWhiteSpace(roleDto.NameAr) || 
                string.IsNullOrWhiteSpace(roleDto.NameEn))
            {
                return BadRequest(responseResult.PostResult(null, HttpStatusCode.BadRequest,
                                  message: "Invalid Role Data."));
            }

            var now = DateTime.UtcNow;
            var role = new Role
            {
                Name = roleDto.NameEn,
                NormalizedName = roleDto.NameEn.ToUpperInvariant(),
                NameAr = roleDto.NameAr,
                CreatedAt = now,
                ModifiedAt = now,
                CreatedById = "System",
                CreatedBy = "System",
                ModifiedById = "System",
                ModifiedBy = "System",
                IsDeleted = false
            };

            var result = await roleManager.CreateAsync(role);

            if (result.Succeeded)
            {
                return Ok(responseResult.PostResult(role.Id, HttpStatusCode.OK,
                          message: HttpStatusCode.OK.ToString()));
            }

            return BadRequest(responseResult.PostResult(null, HttpStatusCode.BadRequest,
                              message: "Failed to create role: " + 
                              string.Join(", ", result.Errors.Select(e => e.Description))));

        }

        /// <summary>
        /// Deletes a role from the application.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>
        /// An IActionResult indicating the result of the delete operation.
        /// </returns>
        
        [HttpDelete("delete/{id}")]
        public async Task<ActionResult<IFinalResult>> DeleteRoleAsync(string id, CancellationToken cancellationToken = default)
        {
            var responseResult = new ResponseResult();

            if (string.IsNullOrEmpty(id))
            {
                return BadRequest(responseResult.PostResult(null, HttpStatusCode.BadRequest,
                                  message: "Role ID cannot be null or empty."));
            }

            var role = await roleManager.FindByIdAsync(id);

            if (role == null)
            {
                return NotFound(responseResult.PostResult(null, HttpStatusCode.NotFound,
                                message: "Role not found."));
            }

            var result = await roleManager.DeleteAsync(role);

            if (result.Succeeded)
            {
                return Ok(responseResult.PostResult(result, HttpStatusCode.OK,
                          message: "Role deleted successfully."));
            }

            return BadRequest(responseResult.PostResult(null, HttpStatusCode.BadRequest,
                              message: "Failed to delete role: " + 
                              string.Join(", ", result.Errors.Select(e => e.Description))));
        }
    }   
}
