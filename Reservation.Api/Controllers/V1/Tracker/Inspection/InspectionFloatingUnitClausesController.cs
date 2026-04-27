using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SonoTracker.Api.Controllers.V1.Base;
using SonoTracker.Application.Services.Tracker.InspectionFloatingUnitClause;
using SonoTracker.Common.Core;
using SonoTracker.Common.DTO.Base;
using SonoTracker.Common.DTO.Tracker.InspectionFloatingUnitClause;
using SonoTracker.Common.DTO.Tracker.InspectionFloatingUnitClause.Parameters;
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
    public class InspectionFloatingUnitClausesController(IInspectionFloatingUnitClauseService inspectionFloatingUnitClauseService) : BaseController
    {
        /// <summary>
        /// Get By Id
        /// </summary>
        [HttpGet("get/{id}")]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status200OK)]
        public async Task<IFinalResult> GetAsync(string id, CancellationToken cancellationToken = default)
            => await inspectionFloatingUnitClauseService.GetByIdAsync(id, cancellationToken);

        /// <summary>
        /// Get For Edit
        /// </summary>
        [HttpGet("getEdit/{id}")]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status200OK)]
        public async Task<IFinalResult> GetEditAsync(string id, CancellationToken cancellationToken = default)
            => await inspectionFloatingUnitClauseService.GetByIdForEditAsync(id, cancellationToken);

        /// <summary>
        /// Get All
        /// </summary>
        /// <param name="inspectionId">Optional. Filter by inspection ID.</param>
        /// <param name="cancellationToken"></param>
        [HttpGet("getAll")]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status200OK)]
        public async Task<ActionResult<IFinalResult>> GetAllAsync([FromQuery] string inspectionId, CancellationToken cancellationToken = default)
        {
            IFinalResult res = await inspectionFloatingUnitClauseService.GetAllAsync(inspectionId, cancellationToken);
            return Ok(res);
        }

        /// <summary>
        /// Get All Paged
        /// </summary>
        [HttpPost("getPaged")]
        [ProducesResponseType<PagingResult>(StatusCodes.Status200OK)]
        public async Task<ActionResult<PagingResult>> GetPagedAsync([FromBody] BaseParam<InspectionFloatingUnitClauseFilter> filter, CancellationToken cancellationToken = default)
        {
            PagingResult res = await inspectionFloatingUnitClauseService.GetAllPagedAsync(filter, cancellationToken);
            return Ok(res);
        }

        /// <summary>
        /// Add
        /// </summary>
        [HttpPost("add")]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status201Created)]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<IFinalResult>> AddAsync([FromBody] AddInspectionFloatingUnitClauseDto dto, CancellationToken cancellationToken = default)
        {
            IFinalResult res = await inspectionFloatingUnitClauseService.AddAsync(dto, cancellationToken);

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
        public async Task<ActionResult<IFinalResult>> UpdateAsync([FromBody] AddInspectionFloatingUnitClauseDto model, CancellationToken cancellationToken = default)
        {
            IFinalResult res = await inspectionFloatingUnitClauseService.UpdateAsync(model, cancellationToken);

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
            IFinalResult res = await inspectionFloatingUnitClauseService.DeleteAsync(id, cancellationToken);

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
            IFinalResult res = await inspectionFloatingUnitClauseService.DeleteSoftAsync(id, cancellationToken);

            if (res.Status == HttpStatusCode.BadRequest) return BadRequest(res);

            return Accepted(res);
        }

        /// <summary>
        /// Delete Range by ids
        /// </summary>
        [HttpDelete("deleteRange")]
        public async Task<IFinalResult> DeleteRangeAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default)
            => await inspectionFloatingUnitClauseService.DeleteRangeAsync(ids, cancellationToken);
    }
}
