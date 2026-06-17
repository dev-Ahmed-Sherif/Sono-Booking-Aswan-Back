using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SonoBooking.Api.Controllers.V1.Base;
using SonoBooking.Application.Services.Housing.Availability;
using SonoBooking.Application.Services.Housing.Rooms;
using SonoBooking.Application.Services.Housing.UnitImages;
using SonoBooking.Application.Services.LookUp.Attachments;
using SonoBooking.Common.Core;
using SonoBooking.Common.DTO.Base;
using SonoBooking.Common.DTO.Housing.Room;
using SonoBooking.Common.DTO.Housing.Room.Parameters;
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
    public class RoomsController(
                 IRoomService roomService,
                 IUnitOccupancyService unitOccupancyService,
                 IUnitImageService unitImageService,
                 IAttachmentService attachmentService) : BaseController
    {
        [HttpGet("get/{id}")]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status200OK)]
        public async Task<ActionResult<IFinalResult>> GetAsync(string id, CancellationToken cancellationToken = default)
        {
            IFinalResult res = await roomService.GetByIdAsync(id, cancellationToken);
            return Ok(res);
        }

        [HttpGet("getAll")]
        [AllowAnonymous]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status200OK)]
        public async Task<ActionResult<IFinalResult>> GetAllAsync(
            [FromHeader(Name = "ApartmentId")] string apartmentId = null,
            [FromHeader(Name = "Status")] string status = null,
            [FromHeader(Name = "StartDate")] string startDate = null,
            [FromHeader(Name = "Nights")] int? nights = null,
            [FromHeader(Name = "Gender")] string gender = null,
            CancellationToken cancellationToken = default)
        {
            var isAnonymous = !(User?.Identity?.IsAuthenticated ?? false);
            var requestedStatus = (status ?? string.Empty).Trim();
            var isAvailableRequested =
                string.Equals(requestedStatus, "متاح", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(requestedStatus, "Available", StringComparison.OrdinalIgnoreCase);

            if (isAnonymous && !isAvailableRequested)
                return Unauthorized();

            var hasInquiryStart = AvailabilityInquiryFilter.TryParseInquiryStart(startDate, out _);
            Expression<Func<Room, bool>> predicate;

            predicate = isAnonymous || isAvailableRequested
                ? AvailabilityCatalogStatus.RoomMatchesInquiry(hasInquiryStart, apartmentId)
                : string.IsNullOrWhiteSpace(apartmentId) ? null : r => r.ApartmentId == apartmentId;

            IFinalResult res = await roomService.GetAllAsync(predicate: predicate, cancellationToken: cancellationToken);

            if (res?.Data is IEnumerable<RoomDto> rooms)
            {
                var filtered = rooms;
                if (AvailabilityInquiryFilter.TryParseInquiryStart(startDate, out var inquiryStart))
                {
                    filtered = await AvailabilityInquiryFilter.FilterRoomsAsync(
                        filtered,
                        unitOccupancyService,
                        inquiryStart,
                        nights,
                        cancellationToken);
                }

                if (AvailabilityInquiryFilter.TryParseGenders(gender, out var genders))
                {
                    filtered = await AvailabilityInquiryFilter.FilterRoomsByGenderAsync(
                        filtered,
                        unitOccupancyService,
                        genders,
                        AvailabilityInquiryFilter.TryParseInquiryStart(startDate, out var parsedStart) ? parsedStart : null,
                        cancellationToken);
                }

                res.Data = filtered;
            }

            return Ok(res);
        }

        [HttpPost("getPaged")]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status200OK)]
        public async Task<ActionResult<PagingResult>> GetPagedAsync([FromBody] BaseParam<RoomFilter> filter, CancellationToken cancellationToken = default)
        {
            PagingResult res = await roomService.GetAllPagedAsync(filter, cancellationToken);
            return Ok(res);
        }

        [HttpPost("add")]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status201Created)]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<IFinalResult>> AddAsync([FromForm] AddRoomDto dto, CancellationToken cancellationToken = default)
        {
            IFinalResult res = await roomService.AddAsync(dto, cancellationToken);

            if (res.Status == HttpStatusCode.BadRequest) return BadRequest(res);
            if (res.Status == HttpStatusCode.Conflict) return Conflict(res);

            return Created(string.Empty, res);
        }

        [HttpPut("update")]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status202Accepted)]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<IFinalResult>> UpdateAsync([FromForm] AddRoomDto model, CancellationToken cancellationToken = default)
        {
            IFinalResult res = await roomService.UpdateAsync(model, cancellationToken);

            if (res.Status == HttpStatusCode.BadRequest) return BadRequest(res);
            if (res.Status == HttpStatusCode.Conflict) return Conflict(res);

            return Accepted(res);
        }

        [HttpDelete("delete/{id}")]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status202Accepted)]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IFinalResult>> DeleteAsync(string id, CancellationToken cancellationToken = default)
        {
            IFinalResult res = await roomService.DeleteAsync(id, cancellationToken);

            if (res.Status == HttpStatusCode.BadRequest) return BadRequest(res);

            return Accepted(res);
        }

        [HttpDelete("deleteSoft/{id}")]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status202Accepted)]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IFinalResult>> DeleteSoftAsync(string id, CancellationToken cancellationToken = default)
        {
            IFinalResult res = await roomService.DeleteSoftAsync(id, cancellationToken);

            if (res.Status == HttpStatusCode.BadRequest) return BadRequest(res);

            return Accepted(res);
        }

        /// <summary>
        /// Deletes a range of attachments by their IDs.
        /// </summary>
        /// <param name="ids">A collection of attachment IDs to delete.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
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
