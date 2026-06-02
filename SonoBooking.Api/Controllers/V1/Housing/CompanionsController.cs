using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SonoBooking.Api.Controllers.V1.Base;
using SonoBooking.Application.Services.Housing.Companions;
using SonoBooking.Common.Core;
using SonoBooking.Common.DTO.Base;
using SonoBooking.Common.DTO.Housing.Companion;
using SonoBooking.Common.DTO.Housing.Companion.Parameters;
using System.Net;
using System.Threading;

namespace SonoBooking.Api.Controllers.V1.Housing
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    public class CompanionsController(ICompanionService companionService) : BaseController
    {
        [HttpGet("get/{id}")]
        [AllowAnonymous]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status200OK)]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IFinalResult>> GetAsync(
            string id,
            [FromHeader(Name = "UserId")] string userId = null,
            CancellationToken cancellationToken = default)
        {
            if (!TryValidateRegistrationUserId(userId, out ActionResult<IFinalResult> unauthorizedResult))
                return unauthorizedResult;

            IFinalResult res = await companionService.GetByIdAsync(id, cancellationToken);

            if (res.Status == HttpStatusCode.Unauthorized) return Unauthorized(res);
            if (res.Status == HttpStatusCode.NotFound) return NotFound(res);

            return Ok(res);
        }

        [HttpGet("getAll")]
        [AllowAnonymous]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status200OK)]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IFinalResult>> GetAllAsync(
            [FromHeader(Name = "UserId")] string userId = null,
            CancellationToken cancellationToken = default)
        {
            if (!TryValidateRegistrationUserId(userId, out ActionResult<IFinalResult> unauthorizedResult))
                return unauthorizedResult;

            IFinalResult res = await companionService.GetAllAsync(cancellationToken: cancellationToken);

            if (res.Status == HttpStatusCode.Unauthorized) return Unauthorized(res);

            return Ok(res);
        }

        [HttpPost("getPaged")]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status200OK)]
        public async Task<ActionResult<PagingResult>> GetPagedAsync([FromBody] BaseParam<CompanionFilter> filter, CancellationToken cancellationToken = default)
        {
            PagingResult res = await companionService.GetAllPagedAsync(filter, cancellationToken);
            return Ok(res);
        }

        [HttpPost("add")]
        [AllowAnonymous]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status201Created)]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<IFinalResult>> AddAsync(
            [FromForm] AddCompanionDto dto,
            [FromHeader(Name = "UserId")] string userId = null,
            CancellationToken cancellationToken = default)
        {
            if (!TryApplyRegistrationUserId(dto, userId, out ActionResult<IFinalResult> unauthorizedResult))
                return unauthorizedResult;

            IFinalResult res = await companionService.AddAsync(dto, cancellationToken);

            if (res.Status == HttpStatusCode.BadRequest) return BadRequest(res);
            if (res.Status == HttpStatusCode.Conflict) return Conflict(res);

            return Created(string.Empty, res);
        }

        [HttpPut("update")]
        [AllowAnonymous]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status202Accepted)]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<IFinalResult>> UpdateAsync(
            [FromForm] AddCompanionDto model,
            [FromHeader(Name = "UserId")] string userId = null,
            CancellationToken cancellationToken = default)
        {
            if (!TryApplyRegistrationUserId(model, userId, out ActionResult<IFinalResult> unauthorizedResult))
                return unauthorizedResult;

            IFinalResult res = await companionService.UpdateAsync(model, cancellationToken);

            if (res.Status == HttpStatusCode.BadRequest) return BadRequest(res);
            if (res.Status == HttpStatusCode.Conflict) return Conflict(res);

            return Accepted(res);
        }

        [HttpDelete("delete/{id}")]
        [AllowAnonymous]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status202Accepted)]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IFinalResult>> DeleteAsync(
            string id,
            [FromHeader(Name = "UserId")] string userId = null,
            CancellationToken cancellationToken = default)
        {
            if (!TryValidateRegistrationUserId(userId, out ActionResult<IFinalResult> unauthorizedResult))
                return unauthorizedResult;

            IFinalResult res = await companionService.DeleteAsync(id, cancellationToken);

            if (res.Status == HttpStatusCode.BadRequest) return BadRequest(res);

            return Accepted(res);
        }

        [HttpDelete("deleteSoft/{id}")]
        [AllowAnonymous]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status202Accepted)]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IFinalResult>> DeleteSoftAsync(
            string id,
            [FromHeader(Name = "UserId")] string userId = null,
            CancellationToken cancellationToken = default)
        {
            if (!TryValidateRegistrationUserId(userId, out ActionResult<IFinalResult> unauthorizedResult))
                return unauthorizedResult;

            IFinalResult res = await companionService.DeleteSoftAsync(id, cancellationToken);

            if (res.Status == HttpStatusCode.BadRequest) return BadRequest(res);

            return Accepted(res);
        }

        /// <summary>
        /// During registration there is no JWT; the client sends the new account id in the UserId header.
        /// </summary>
        private bool TryApplyRegistrationUserId(AddCompanionDto dto, string userId, out ActionResult<IFinalResult> unauthorizedResult)
        {
            unauthorizedResult = null;
            var isAnonymous = !(User?.Identity?.IsAuthenticated ?? false);
            if (!isAnonymous)
                return true;

            if (string.IsNullOrWhiteSpace(userId))
            {
                unauthorizedResult = Unauthorized();
                return false;
            }

            dto.UserId = userId.Trim();
            return true;
        }

        private bool TryValidateRegistrationUserId(string userId, out ActionResult<IFinalResult> unauthorizedResult)
        {
            unauthorizedResult = null;
            var isAnonymous = !(User?.Identity?.IsAuthenticated ?? false);
            if (!isAnonymous)
                return true;

            if (string.IsNullOrWhiteSpace(userId))
            {
                unauthorizedResult = Unauthorized();
                return false;
            }

            return true;
        }
    }
}
