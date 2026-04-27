using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SonoTracker.Api.Controllers.V1.Base;
using SonoTracker.Application.Services.Tracker.TripGeo;
using SonoTracker.Common.Core;
using SonoTracker.Common.DTO.Base;
using SonoTracker.Common.DTO.Tracker.TripGeo;
using SonoTracker.Common.DTO.Tracker.TripGeo.Parameters;
using System.Net;
using System.Threading;

namespace SonoTracker.Api.Controllers.V1.Tracker.Trip
{
    /// <summary>
    /// Constructor
    /// </summary>
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    public class TripGeoController(ITripGeoService tripGeoService) : BaseController
    {
        /// <summary>
        /// Get By Id 
        /// </summary>
        /// <returns></returns>

        [HttpGet("get/{id}")]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status200OK)]
        public async Task<IFinalResult> GetAsync(string id, CancellationToken cancellationToken = default)
                                        => await tripGeoService.GetByIdAsync(id, cancellationToken);

        /// <summary>
        /// Get For Edit 
        /// </summary>
        /// <returns></returns>

        [HttpGet("getEdit/{id}")]
        public async Task<IFinalResult> GetEditAsync(string id, CancellationToken cancellationToken = default)
                                        => await tripGeoService.GetByIdForEditAsync(id, cancellationToken);

        /// <summary>
        /// Get All 
        /// </summary>
        /// <returns></returns>

        [HttpGet("getAll")]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status200OK)]
        public async Task<ActionResult<IFinalResult>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            IFinalResult res = await tripGeoService.GetAllAsync(cancellationToken: cancellationToken);

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
        public async Task<ActionResult<PagingResult>> GetPagedAsync([FromBody] BaseParam<TripGeoFilter> filter, CancellationToken cancellationToken = default)
        {
            PagingResult res = await tripGeoService.GetAllPagedAsync(filter, cancellationToken);

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
            PagingResult res = await tripGeoService.GetDropDownAsync(filter, cancellationToken);

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
        public async Task<ActionResult<IFinalResult>> AddAsync([FromBody] AddTripGeoDto dto, CancellationToken cancellationToken = default)
        {
            IFinalResult res = await tripGeoService.AddAsync(dto, cancellationToken);

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
        public async Task<ActionResult<IFinalResult>> UpdateAsync([FromBody] AddTripGeoDto model, CancellationToken cancellationToken = default)
        {
            IFinalResult res = await tripGeoService.UpdateAsync(model, cancellationToken);

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
            IFinalResult res = await tripGeoService.DeleteAsync(id, cancellationToken);

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
            IFinalResult res = await tripGeoService.DeleteSoftAsync(id, cancellationToken);

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
                                        => await tripGeoService.DeleteRangeAsync(ids, cancellationToken);
    }
}
