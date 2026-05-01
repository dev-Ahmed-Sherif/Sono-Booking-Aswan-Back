using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SonoTracker.Api.Controllers.V1.Base;
using SonoTracker.Application.Services.Tracker.OrganizationStaffStaff;
using SonoTracker.Common.Core;
using SonoTracker.Common.DTO.Base;
using SonoTracker.Common.DTO.Tracker.OrganizationStaff;
using SonoTracker.Common.DTO.Tracker.OrganizationStaff.Parameters;
using System.Net;
using System.Threading;

namespace SonoTracker.Api.Controllers.V1.Tracker.Organization
{
    /// <summary>
    /// Constructor
    /// </summary>
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    public class OrganizationStaffsController(IOrganizationStaffService organizationStaffService) : BaseController
    {
        /// <summary>
        /// Get By Id 
        /// </summary>
        /// <returns></returns>
        [HttpGet("get/{id}")]
        public async Task<ActionResult<IFinalResult>> GetAsync(string id, CancellationToken cancellationToken = default)
        {
            IFinalResult res = await organizationStaffService.GetByIdAsync(id, cancellationToken);
            
            return Ok(res);
        }  

        ///// <summary>
        ///// Get For Edit 
        ///// </summary>
        ///// <returns></returns>
        //[HttpGet("getEdit/{id}")]
        //public async Task<IFinalResult> GetEditAsync(string id) => await organizationStaffService.GetByIdForEditAsync(id);

        /// <summary>
        /// Get All 
        /// </summary>
        /// <param name="organizationId">Optional. Filter by organization ID.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet("getAll")]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status200OK)]
        public async Task<ActionResult<IFinalResult>> GetAllAsync([FromQuery] string organizationId, CancellationToken cancellationToken = default)
        {
            IFinalResult res = await organizationStaffService.GetAllAsync(organizationId, cancellationToken);

            return Ok(res);
        }

        /// <summary>
        /// GetAll Data paged
        /// </summary>
        /// <param name="filter">Filter responsible for search and sort</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost("getPaged")]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status200OK)]
        public async Task<ActionResult<PagingResult>> GetPagedAsync([FromBody] BaseParam<OrganizationStaffFilter> filter, CancellationToken cancellationToken = default)
        {
            PagingResult res = await organizationStaffService.GetAllPagedAsync(filter, cancellationToken);

            return Ok(res);
        }

        /// <summary>
        /// Get All Data paged For Drop Down
        /// </summary>
        /// <param name="filter">Filter responsible for search and sort</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("getDropDown")]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status200OK)]
        public async Task<ActionResult<PagingResult>> GetDropDownAsync([FromBody] BaseParam<SearchCriteriaFilter> filter, CancellationToken cancellationToken = default)
        {
            PagingResult res = await organizationStaffService.GetDropDownAsync(filter, cancellationToken);

            return Ok(res);
        }

        /// <summary>
        /// Add 
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost("add")]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status201Created)]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<IFinalResult>> AddAsync([FromForm] AddOrganizationStaffDto dto, CancellationToken cancellationToken = default)
        {
            IFinalResult res = await organizationStaffService.AddAsync(dto, cancellationToken);

            if (res.Status == HttpStatusCode.BadRequest) return BadRequest(res);

            if (res.Status == HttpStatusCode.Conflict) return Conflict(res); 

            return Created("",res);
        }

        /// <summary>
        /// Update  
        /// </summary>
        /// <param name="model">Object content</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPut("update")]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status202Accepted)]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<IFinalResult>> UpdateAsync([FromForm] AddOrganizationStaffDto model, CancellationToken cancellationToken = default)
        {
            IFinalResult res = await organizationStaffService.UpdateAsync(model, cancellationToken);
            
            if (res.Status == HttpStatusCode.BadRequest) return BadRequest(res);

            if (res.Status == HttpStatusCode.Conflict) return Conflict(res);

            return Accepted("", res);
        }  

        /// <summary>
        /// Remove  by id
        /// </summary>
        /// <param name="id">PK</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpDelete("delete/{id}")]
        public async Task<IFinalResult> DeleteAsync(string id, CancellationToken cancellationToken = default) => await organizationStaffService.DeleteAsync(id, cancellationToken);

        /// <summary>
        /// Soft Remove  by id
        /// </summary>
        /// <param name="id">PK</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpDelete("deleteSoft/{id}")]
        public async Task<IFinalResult> DeleteSoftAsync(string id, CancellationToken cancellationToken = default) => await organizationStaffService.DeleteSoftAsync(id, cancellationToken);
    }
}
