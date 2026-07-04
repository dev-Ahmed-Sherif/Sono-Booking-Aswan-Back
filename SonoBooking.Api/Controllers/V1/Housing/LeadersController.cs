using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SonoBooking.Api.Controllers.V1.Base;
using SonoBooking.Application.Services.Housing.Leaders;
using SonoBooking.Common.Core;
using SonoBooking.Common.DTO.Base;
using SonoBooking.Common.DTO.Housing.Leader;
using SonoBooking.Common.DTO.Housing.Leader.Parameters;
using System.Net;
using System.Threading;

namespace SonoBooking.Api.Controllers.V1.Housing
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    public class LeadersController(ILeaderService leaderService) : BaseController
    {
        [HttpGet("get/{id}")]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status200OK)]
        public async Task<ActionResult<IFinalResult>> GetAsync(string id, CancellationToken cancellationToken = default)
        {
            IFinalResult res = await leaderService.GetByIdAsync(id, cancellationToken);
            return Ok(res);
        }

        [HttpGet("getAll")]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status200OK)]
        public async Task<ActionResult<IFinalResult>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            IFinalResult res = await leaderService.GetAllAsync(cancellationToken: cancellationToken);
            return Ok(res);
        }

        [HttpPost("getPaged")]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status200OK)]
        public async Task<ActionResult<PagingResult>> GetPagedAsync([FromBody] BaseParam<LeaderFilter> filter, CancellationToken cancellationToken = default)
        {
            PagingResult res = await leaderService.GetAllPagedAsync(filter, cancellationToken);
            return Ok(res);
        }

        [HttpPost("add")]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status201Created)]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<IFinalResult>> AddAsync([FromForm] AddLeaderDto dto, CancellationToken cancellationToken = default)
        {
            IFinalResult res = await leaderService.AddAsync(dto, cancellationToken);

            if (res.Status == HttpStatusCode.BadRequest) return BadRequest(res);
            if (res.Status == HttpStatusCode.Conflict) return Conflict(res);

            return Created(string.Empty, res);
        }

        [HttpPut("update")]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status202Accepted)]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<IFinalResult>> UpdateAsync([FromForm] AddLeaderDto model, CancellationToken cancellationToken = default)
        {
            IFinalResult res = await leaderService.UpdateAsync(model, cancellationToken);

            if (res.Status == HttpStatusCode.BadRequest) return BadRequest(res);
            if (res.Status == HttpStatusCode.Conflict) return Conflict(res);

            return Accepted(res);
        }

        [HttpDelete("delete/{id}")]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status202Accepted)]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IFinalResult>> DeleteAsync(string id, CancellationToken cancellationToken = default)
        {
            IFinalResult res = await leaderService.DeleteAsync(id, cancellationToken);

            if (res.Status == HttpStatusCode.BadRequest) return BadRequest(res);

            return Accepted(res);
        }

        [HttpDelete("deleteSoft/{id}")]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status202Accepted)]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IFinalResult>> DeleteSoftAsync(string id, CancellationToken cancellationToken = default)
        {
            IFinalResult res = await leaderService.DeleteSoftAsync(id, cancellationToken);

            if (res.Status == HttpStatusCode.BadRequest) return BadRequest(res);

            return Accepted(res);
        }
    }
}
