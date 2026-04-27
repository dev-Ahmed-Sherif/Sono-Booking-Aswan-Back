using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SonoTracker.Api.Controllers.V1.Base;
using SonoTracker.Application.Services.Tracker.InspectionClause;
using SonoTracker.Common.Core;
using SonoTracker.Common.DTO.Base;
using SonoTracker.Common.DTO.Tracker.InspectionClause;
using SonoTracker.Common.DTO.Tracker.InspectionClause.Parameters;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SonoTracker.Api.Controllers.V1.Tracker.Inspection
{
    /// <summary>
    /// Constructor
    /// </summary>
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    public class InspectionClausesController(IInspectionClauseService inspectionClauseService) : BaseController
    {
        /// <summary>
        /// Get By Id
        /// </summary>
        [HttpGet("get/{id}")]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status200OK)]
        public async Task<IFinalResult> GetAsync(string id, CancellationToken cancellationToken = default)
            => await inspectionClauseService.GetByIdAsync(id, cancellationToken);

        /// <summary>
        /// Get For Edit
        /// </summary>
        [HttpGet("getEdit/{id}")]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status200OK)]
        public async Task<IFinalResult> GetEditAsync(string id, CancellationToken cancellationToken = default)
            => await inspectionClauseService.GetByIdForEditAsync(id, cancellationToken);

        /// <summary>
        /// Get All
        /// </summary>
        /// <param name="inspectionTypeId">Optional. Filter by inspection type ID.</param>
        /// <param name="cancellationToken"></param>
        [HttpGet("getAll")]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status200OK)]
        public async Task<ActionResult<IFinalResult>> GetAllAsync([FromQuery] string? inspectionTypeId, CancellationToken cancellationToken = default)
        {
            IFinalResult res = await inspectionClauseService.GetAllAsync(inspectionTypeId, cancellationToken);
            return Ok(res);
        }

        /// <summary>
        /// Get All Paged
        /// </summary>
        [HttpPost("getPaged")]
        [ProducesResponseType<PagingResult>(StatusCodes.Status200OK)]
        public async Task<ActionResult<PagingResult>> GetPagedAsync([FromBody] BaseParam<InspectionClauseFilter> filter, CancellationToken cancellationToken = default)
        {
            PagingResult res = await inspectionClauseService.GetAllPagedAsync(filter, cancellationToken);
            return Ok(res);
        }

        /// <summary>
        /// Add
        /// </summary>
        [HttpPost("add")]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status201Created)]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<IFinalResult>> AddAsync([FromBody] AddInspectionClauseDto dto, CancellationToken cancellationToken = default)
        {
            IFinalResult res = await inspectionClauseService.AddAsync(dto, cancellationToken);

            if (res.Status == HttpStatusCode.BadRequest) return BadRequest(res);
            if (res.Status == HttpStatusCode.Conflict) return Conflict(res);

            return Created("", res);
        }

        /// <summary>
        /// Update
        /// </summary>
        [HttpPut("update")]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status202Accepted)]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<IFinalResult>> UpdateAsync([FromBody] AddInspectionClauseDto model, CancellationToken cancellationToken = default)
        {
            IFinalResult res = await inspectionClauseService.UpdateAsync(model, cancellationToken);

            if (res.Status == HttpStatusCode.BadRequest) return BadRequest(res);
            if (res.Status == HttpStatusCode.Conflict) return Conflict(res);

            return Accepted(res);
        }

        /// <summary>
        /// Delete by id
        /// </summary>
        [HttpDelete("delete/{id}")]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status202Accepted)]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IFinalResult>> DeleteAsync(string id, CancellationToken cancellationToken = default)
        {
            IFinalResult res = await inspectionClauseService.DeleteAsync(id, cancellationToken);

            if (res.Status == HttpStatusCode.BadRequest) return BadRequest(res);

            return Accepted(res);
        }

        /// <summary>
        /// Soft Delete by id
        /// </summary>
        [HttpDelete("deleteSoft/{id}")]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status202Accepted)]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IFinalResult>> DeleteSoftAsync(string id, CancellationToken cancellationToken = default)
        {
            IFinalResult res = await inspectionClauseService.DeleteSoftAsync(id, cancellationToken);

            if (res.Status == HttpStatusCode.BadRequest) return BadRequest(res);

            return Accepted(res);
        }

        /// <summary>
        /// Delete Range by ids
        /// </summary>
        [HttpDelete("deleteRange")]
        public async Task<IFinalResult> DeleteRangeAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default)
            => await inspectionClauseService.DeleteRangeAsync(ids, cancellationToken);
    }
}
