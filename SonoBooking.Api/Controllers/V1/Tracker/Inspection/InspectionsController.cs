using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SonoTracker.Api.Controllers.V1.Base;
using SonoTracker.Application.Services.LookUp.Attach;
using SonoTracker.Application.Services.Tracker.Inspection;
using SonoTracker.Application.Services.Tracker.InspectionAttach;
using SonoTracker.Common.Core;
using SonoTracker.Common.DTO.Base;
using SonoTracker.Common.DTO.Tracker.GeneralInspection;
using SonoTracker.Common.DTO.Tracker.GeneralInspection.Parameters;
using SonoTracker.Common.DTO.Tracker.Inspection;
using SonoTracker.Common.DTO.Tracker.Inspection.Parameters;
using SonoTracker.Common.DTO.Tracker.InspectionFloatingUnitClause;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading;

namespace SonoTracker.Api.Controllers.V1.Tracker.Inspection
{
    /// <summary>
    /// Constructor
    /// </summary>
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    public class InspectionsController(IInspectionService inspectionService,
                                       IInspectionAttachService inspectionAttachService,
                                       IAttachService attachService) : BaseController
    {
        /// <summary>
        /// Get By Id 
        /// </summary>
        /// <returns></returns>

        [HttpGet("get/{id}")]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status200OK)]
        public async Task<IFinalResult> GetAsync(string id, CancellationToken cancellationToken = default)
                                        => await inspectionService.GetByIdAsync(id, cancellationToken);

        /// <summary>
        /// Get For Edit 
        /// </summary>
        /// <returns></returns>

        [HttpGet("getEdit/{id}")]
        public async Task<IFinalResult> GetEditAsync(string id, CancellationToken cancellationToken = default)
                                        => await inspectionService.GetByIdForEditAsync(id, cancellationToken);

        /// <summary>
        /// Get All 
        /// </summary>
        /// <param name="inspectionTypeId">Optional. Filter by floating unit ID.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>

        [HttpGet("getAll")]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status200OK)]
        public async Task<ActionResult<IFinalResult>> GetAllAsync([FromQuery] string? inspectionTypeId, CancellationToken cancellationToken = default)
        {
            IFinalResult res = await inspectionService.GetAllAsync(inspectionTypeId, cancellationToken);

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
        public async Task<ActionResult<PagingResult>> GetPagedAsync([FromBody] BaseParam<InspectionFilter> filter, CancellationToken cancellationToken = default)
        {
            PagingResult res = await inspectionService.GetAllPagedAsync(filter, cancellationToken);

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
        public async Task<ActionResult<IFinalResult>> AddAsync([FromForm] AddInspectionDto dto, CancellationToken cancellationToken = default)
        {
            // [FromForm] cannot automatically deserialize a JSON-encoded string into a collection.
            // The frontend sends InspectionFloatingUnitClauses as a JSON string inside FormData,
            // so we deserialize it manually here before passing to the service.
            var clausesJson = Request.Form["InspectionFloatingUnitClauses"].FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(clausesJson))
            {
                dto.InspectionFloatingUnitClauses = JsonSerializer.Deserialize<ICollection<AddInspectionFloatingUnitClauseDto>>(
                    clausesJson, new JsonSerializerOptions(JsonSerializerDefaults.Web));
            }

            IFinalResult res = await inspectionService.AddAsync(dto, cancellationToken);

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
        public async Task<ActionResult<IFinalResult>> UpdateAsync([FromForm] AddInspectionDto model, CancellationToken cancellationToken = default)
        {
            var clausesJson = Request.Form["InspectionFloatingUnitClauses"].FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(clausesJson))
            {
                model.InspectionFloatingUnitClauses = JsonSerializer.Deserialize<ICollection<AddInspectionFloatingUnitClauseDto>>(
                    clausesJson, new JsonSerializerOptions(JsonSerializerDefaults.Web));
            }

            IFinalResult res = await inspectionService.UpdateAsync(model, cancellationToken);

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
            IFinalResult res = await inspectionService.DeleteAsync(id, cancellationToken);

            if (res.Status == HttpStatusCode.BadRequest) return BadRequest(res);

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
            IFinalResult res = await inspectionService.DeleteSoftAsync(id, cancellationToken);

            if (res.Status == HttpStatusCode.BadRequest) return BadRequest(res);

            return Accepted(res);
        }

        /// <summary>
        /// Remove Range by Organization Ids
        /// </summary>
        /// <param name="ids">PK</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpDelete("deleteRange")]
        public async Task<IFinalResult> DeleteRangeAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default)
                                        => await inspectionService.DeleteRangeAsync(ids, cancellationToken);

        /// <summary>
        /// Deletes a range of attachments by their IDs.
        /// </summary>
        /// <param name="ids">A collection of attachment IDs to delete.</param>
        /// <returns>An <see cref="IFinalResult"/> indicating the result of the operation.</returns>
        [HttpDelete("deleteRange/attachments")]
        public async Task<ActionResult<IFinalResult>> DeleteRangeAttachmentsAsync([FromBody] IEnumerable<string> ids, CancellationToken cancellationToken = default)
        {
            if (ids == null || !ids.Any())
            {
                return new FinalResult
                {
                    Status = HttpStatusCode.BadRequest,
                    Message = "No IDs provided for deletion."
                };
            }

            IFinalResult resultInspectionAttach = await inspectionAttachService.DeleteRangeWithAttachIdRangeAsync(ids, cancellationToken);
            IFinalResult resultAttach = await attachService.DeleteRangeAsync(ids, cancellationToken);
            if (resultAttach.Status == HttpStatusCode.BadRequest && resultInspectionAttach.Status == HttpStatusCode.BadRequest)
            {
                return BadRequest(resultAttach);
            }

            return Ok(resultAttach);
        }
    }
}
