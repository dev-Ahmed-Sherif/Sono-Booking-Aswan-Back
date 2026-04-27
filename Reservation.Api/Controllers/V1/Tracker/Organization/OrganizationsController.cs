using Asp.Versioning;
using DeputyOffice.Common.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SonoTracker.Api.Controllers.V1.Base;
using SonoTracker.Application.Services.Tracker.Organizations;
using SonoTracker.Common.Core;
using SonoTracker.Common.DTO.Base;
using SonoTracker.Common.DTO.Reports.Org;
using SonoTracker.Common.DTO.Tracker.Organization;
using SonoTracker.Common.DTO.Tracker.Organization.Parameters;
using SonoTracker.Domain.Entities.Lookups;
using SonoTracker.Domain.Enum;
using System.Net;
using System.Net.Mime;
using System.Reflection.Metadata;
using System.Threading;

namespace SonoTracker.Api.Controllers.V1.Tracker.Organization
{
    /// <summary>
    /// Constructor
    /// </summary>
    
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    
    public class OrganizationsController(IOrganizationService organizationService) : BaseController
    {
        /// <summary>
        /// Get By Id 
        /// </summary>
        /// <returns></returns>

        [HttpGet("get/{id}")]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status200OK)]
        public async Task<ActionResult<IFinalResult>> GetAsync(string id, CancellationToken cancellationToken = default)
        {
            IFinalResult res = await organizationService.GetByIdForEditAsync(id, cancellationToken);

            return Ok(res);
        }
                                         

        // <summary>
        // Get For Edit
        // </summary>
        // <returns></returns>

        //[HttpGet("getEdit/{id}")]
        //public async Task<IFinalResult> GetEditAsync(string id, CancellationToken cancellationToken = default)
        //                                => await organizationService.GetByIdForEditAsync(id, cancellationToken);

        /// <summary>
        /// Get All 
        /// </summary>
        /// <returns></returns>

        [HttpGet("getAll")]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status200OK)]
        public async Task<ActionResult<IFinalResult>> GetAllAsync([FromQuery] OrganizationType? organizationType, [FromQuery] string? organizationCategoryId, CancellationToken cancellationToken = default)
        {
            IFinalResult res = await organizationService.GetAllAsync(organizationType ,organizationCategoryId ,cancellationToken);

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
        public async Task<ActionResult<PagingResult>> GetPagedAsync([FromBody] BaseParam<OrganizationFilter> filter, CancellationToken cancellationToken = default)
        {
            PagingResult res = await organizationService.GetAllPagedAsync(filter, cancellationToken);

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
            PagingResult res = await organizationService.GetDropDownAsync(filter, cancellationToken);

            return Ok(res);
        }

        /// <summary>
        /// Add 
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>

        [HttpPost("add")]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status201Created)]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<IFinalResult>> AddAsync([FromForm] AddOrganizationDto dto, CancellationToken cancellationToken = default)
        {
            IFinalResult res = await organizationService.AddAsync(dto, cancellationToken);

            if (res.Status == HttpStatusCode.BadRequest) return BadRequest(res);

            if (res.Status == HttpStatusCode.Conflict) return Conflict(res);

            return Created("", res);
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
        public async Task<ActionResult<IFinalResult>> UpdateAsync([FromForm] AddOrganizationDto model, CancellationToken cancellationToken = default)
        {
            IFinalResult res = await organizationService.UpdateAsync(model, cancellationToken);

            if (res.Status == HttpStatusCode.BadRequest) return BadRequest(res);

            if (res.Status == HttpStatusCode.Conflict) return Conflict(res);

            return Accepted(res);
        }

        /// <summary>
        /// Remove  by id
        /// </summary>
        /// <param name="id">PK</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>

        [HttpDelete("delete/{id}")]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status202Accepted)]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IFinalResult>> DeleteAsync(string id, CancellationToken cancellationToken = default)
        {
            IFinalResult res = await organizationService.DeleteAsync(id, cancellationToken);

            if (res.Status == HttpStatusCode.NotFound) return BadRequest("هذا البيان غير موجود");

            return Accepted(res);
        }

        /// <summary>
        /// Soft Remove by id
        /// </summary>
        /// <param name="id">PK</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>

        [HttpDelete("deleteSoft/{id}")]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status202Accepted)]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IFinalResult>> DeleteSoftAsync(string id, CancellationToken cancellationToken = default)
        {
            IFinalResult res = await organizationService.DeleteSoftAsync(id, cancellationToken);

            if (res.Status == HttpStatusCode.BadRequest) return BadRequest(res);

            return Accepted(res);
        }

        // <summary>
        // Remove Range by Organization Ids
        // </summary>
        // <param name="ids">PK</param>
        // <param name="cancellationToken"></param>
        // <returns></returns>
        
        //[HttpDelete("deleteRange")]
        //public async Task<IFinalResult> DeleteRangeAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default)
        //                                => await organizationService.DeleteRangeAsync(ids, cancellationToken);

        /// <summary>
        /// Generates a project report based on the provided filter.
        /// </summary>
        /// <param name="filter">The filter containing report parameters.</param>
        /// <param name="cancellationToken">Token to observe while waiting for the task to complete.</param>
        /// <returns>A PDF file containing the generated report.</returns>
        
        [HttpGet("GetReport")]
        [ProducesResponseType<Blob>(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetProjectReportData([FromQuery] FilterOrgReportDTO filter, CancellationToken cancellationToken = default)
        {
            var report = await organizationService.GenerateReportAsync(filter, cancellationToken);
            return File(report, MediaTypeNames.Application.Pdf, ReportHelper.GetReportDetails(filter.ReportName, filter.ReportType));
        }
    }
}
