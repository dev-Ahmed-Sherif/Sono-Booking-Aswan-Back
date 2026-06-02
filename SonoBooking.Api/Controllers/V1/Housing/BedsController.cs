using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SonoBooking.Api.Controllers.V1.Base;
using SonoBooking.Application.Services.Housing.Availability;
using SonoBooking.Application.Services.Housing.Beds;
using SonoBooking.Application.Services.Housing.UnitImages;
using SonoBooking.Application.Services.LookUp.Attachments;
using SonoBooking.Common.Core;
using SonoBooking.Common.DTO.Base;
using SonoBooking.Common.DTO.Housing.Bed;
using SonoBooking.Common.DTO.Housing.Bed.Parameters;
using SonoBooking.Domain;
using SonoBooking.Domain.Entities.Housing;
using System;
using System.Linq.Expressions;
using System.Net;
using System.Threading;

namespace SonoBooking.Api.Controllers.V1.Housing
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    public class BedsController(
                 IBedService bedService,
                 IUnitOccupancyService unitOccupancyService,
                 IUnitImageService unitImageService,
                 IAttachmentService attachmentService) : BaseController
    {
        [HttpGet("get/{id}")]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status200OK)]
        public async Task<ActionResult<IFinalResult>> GetAsync(string id, CancellationToken cancellationToken = default)
        {
            IFinalResult res = await bedService.GetByIdAsync(id, cancellationToken);
            return Ok(res);
        }

        [HttpGet("getAll")]
        [AllowAnonymous]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status200OK)]
        public async Task<ActionResult<IFinalResult>> GetAllAsync(
            [FromHeader(Name = "RoomId")] string roomId = null,
            [FromHeader(Name = "Status")] string status = null,
            [FromHeader(Name = "StartDate")] string startDate = null,
            [FromHeader(Name = "Nights")] int? nights = null,
            CancellationToken cancellationToken = default)
        {
            var isAnonymous = !(User?.Identity?.IsAuthenticated ?? false);
            var requestedStatus = (status ?? string.Empty).Trim();
            var isAvailableRequested =
                string.Equals(requestedStatus, "متاح", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(requestedStatus, "Available", StringComparison.OrdinalIgnoreCase);

            if (isAnonymous && !isAvailableRequested)
                return Unauthorized();

            Expression<Func<Bed, bool>> predicate;

            if (string.IsNullOrWhiteSpace(roomId))
            {
                predicate = isAnonymous || isAvailableRequested
                    ? b => b.Status == UnitStatus.Available
                    : null;
            }
            else
            {
                predicate = isAnonymous || isAvailableRequested
                    ? b => b.RoomId == roomId && b.Status == UnitStatus.Available
                    : b => b.RoomId == roomId;
            }

            IFinalResult res = await bedService.GetAllAsync(predicate: predicate, cancellationToken: cancellationToken);

            if (AvailabilityInquiryFilter.TryParseInquiryStart(startDate, out var inquiryStart) &&
                res?.Data is IEnumerable<BedDto> beds)
            {
                var filtered = await AvailabilityInquiryFilter.FilterBedsAsync(
                    beds,
                    unitOccupancyService,
                    inquiryStart,
                    cancellationToken);
                res.Data = filtered;
            }

            return Ok(res);
        }

        [HttpPost("getPaged")]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status200OK)]
        public async Task<ActionResult<PagingResult>> GetPagedAsync([FromBody] BaseParam<BedFilter> filter, CancellationToken cancellationToken = default)
        {
            PagingResult res = await bedService.GetAllPagedAsync(filter, cancellationToken);
            return Ok(res);
        }

        [HttpPost("add")]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status201Created)]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<IFinalResult>> AddAsync([FromForm] AddBedDto dto, CancellationToken cancellationToken = default)
        {
            IFinalResult res = await bedService.AddAsync(dto, cancellationToken);

            if (res.Status == HttpStatusCode.BadRequest) return BadRequest(res);
            if (res.Status == HttpStatusCode.Conflict) return Conflict(res);

            return Created(string.Empty, res);
        }

        [HttpPut("update")]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status202Accepted)]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<IFinalResult>> UpdateAsync([FromForm] AddBedDto model, CancellationToken cancellationToken = default)
        {
            IFinalResult res = await bedService.UpdateAsync(model, cancellationToken);

            if (res.Status == HttpStatusCode.BadRequest) return BadRequest(res);
            if (res.Status == HttpStatusCode.Conflict) return Conflict(res);

            return Accepted(res);
        }

        [HttpDelete("delete/{id}")]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status202Accepted)]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IFinalResult>> DeleteAsync(string id, CancellationToken cancellationToken = default)
        {
            IFinalResult res = await bedService.DeleteAsync(id, cancellationToken);

            if (res.Status == HttpStatusCode.BadRequest) return BadRequest(res);

            return Accepted(res);
        }

        [HttpDelete("deleteSoft/{id}")]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status202Accepted)]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IFinalResult>> DeleteSoftAsync(string id, CancellationToken cancellationToken = default)
        {
            IFinalResult res = await bedService.DeleteSoftAsync(id, cancellationToken);

            if (res.Status == HttpStatusCode.BadRequest) return BadRequest(res);

            return Accepted(res);
        }

        /// <summary>
        /// Deletes a range of attachments by their IDs.
        /// </summary>
        /// <param name="ids">A collection of attachment IDs to delete.</param>
        /// <returns>An <see cref="IFinalResult"/> indicating the result of the operation.</returns>
        [HttpDelete("deleteRange/attachments")]
        public async Task<ActionResult<IFinalResult>> DeleteRangeAsync([FromBody] IEnumerable<string> ids, CancellationToken cancellationToken = default)
        {
            // Call the service to delete the attachments
            // and return the result
            IFinalResult resultUnitImageService = await unitImageService.DeleteRangeWithAttachIdRangeAsync(ids, cancellationToken);
            IFinalResult resultAttach = await attachmentService.DeleteRangeAsync(ids, cancellationToken);
            if (resultAttach.Status == HttpStatusCode.BadRequest && resultUnitImageService.Status == HttpStatusCode.BadRequest)
            {
                return BadRequest(resultAttach);
            }
            return Ok(resultAttach);
        }
    }
}
