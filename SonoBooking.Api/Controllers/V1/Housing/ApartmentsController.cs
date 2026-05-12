using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SonoBooking.Api.Controllers.V1.Base;
using SonoBooking.Application.Services.Housing.Apartments;
using SonoBooking.Application.Services.Housing.UnitImages;
using SonoBooking.Application.Services.LookUp.Attachments;
using SonoBooking.Common.Core;
using SonoBooking.Common.DTO.Base;
using SonoBooking.Common.DTO.Housing.Apartment;
using SonoBooking.Common.DTO.Housing.Apartment.Parameters;
using System.Net;
using System.Threading;

namespace SonoBooking.Api.Controllers.V1.Housing
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    public class ApartmentsController(
                 IApartmentService apartmentService,
                 IUnitImageService unitImageService,
                 IAttachmentService attachmentService) : BaseController
    {
        [HttpGet("get/{id}")]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status200OK)]
        public async Task<ActionResult<IFinalResult>> GetAsync(string id, CancellationToken cancellationToken = default)
        {
            IFinalResult res = await apartmentService.GetByIdAsync(id, cancellationToken);
            
            return Ok(res);
        }

        [HttpGet("getAll")]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status200OK)]
        public async Task<ActionResult<IFinalResult>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            IFinalResult res = await apartmentService.GetAllAsync(cancellationToken: cancellationToken);
            
            return Ok(res);
        }

        [HttpPost("getPaged")]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status200OK)]
        public async Task<ActionResult<PagingResult>> GetPagedAsync([FromBody] BaseParam<ApartmentFilter> filter, CancellationToken cancellationToken = default)
        {
            PagingResult res = await apartmentService.GetAllPagedAsync(filter, cancellationToken);
            
            return Ok(res);
        }

        [HttpPost("add")]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status201Created)]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<IFinalResult>> AddAsync([FromForm] AddApartmentDto dto, CancellationToken cancellationToken = default)
        {
            IFinalResult res = await apartmentService.AddAsync(dto, cancellationToken);

            if (res.Status == HttpStatusCode.BadRequest) return BadRequest(res);
            
            if (res.Status == HttpStatusCode.Conflict) return Conflict(res);

            return Created(string.Empty, res);
        }

        [HttpPut("update")]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status202Accepted)]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<IFinalResult>> UpdateAsync([FromForm] AddApartmentDto model, CancellationToken cancellationToken = default)
        {
            IFinalResult res = await apartmentService.UpdateAsync(model, cancellationToken);

            if (res.Status == HttpStatusCode.BadRequest) return BadRequest(res);
            
            if (res.Status == HttpStatusCode.Conflict) return Conflict(res);

            return Accepted(res);
        }

        [HttpDelete("delete/{id}")]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status202Accepted)]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IFinalResult>> DeleteAsync(string id, CancellationToken cancellationToken = default)
        {
            IFinalResult res = await apartmentService.DeleteAsync(id, cancellationToken);

            if (res.Status == HttpStatusCode.BadRequest) return BadRequest(res);

            return Accepted(res);
        }

        [HttpDelete("deleteSoft/{id}")]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status202Accepted)]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IFinalResult>> DeleteSoftAsync(string id, CancellationToken cancellationToken = default)
        {
            IFinalResult res = await apartmentService.DeleteSoftAsync(id, cancellationToken);

            if (res.Status == HttpStatusCode.BadRequest) return BadRequest(res);

            return Accepted(res);
        }

        /// <summary>
        /// Deletes a range of attachments by their IDs.
        /// </summary>
        /// <param name="ids">A collection of attachment IDs to delete.</param>
        /// <param name="cancellationToken">A collection of attachment IDs to delete.</param>
        /// <returns>An <see cref="IFinalResult"/> indicating the result of the operation.</returns>
        
        [HttpDelete("deleteRange/attachments")]
        public async Task<ActionResult<IFinalResult>> DeleteRangeAsync([FromBody] IEnumerable<string> ids, CancellationToken cancellationToken = default)
        {
            // Call the service to delete the attachments
            // and return the result
            IFinalResult resultUnitImageService = await unitImageService.DeleteRangeWithAttachIdRangeAsync(ids, cancellationToken);
            
            IFinalResult resultAttach = await attachmentService.DeleteRangeAsync(ids, cancellationToken);
            
            if (resultAttach.Status == HttpStatusCode.BadRequest || resultUnitImageService.Status == HttpStatusCode.BadRequest) return BadRequest(resultAttach);
                        
            return Ok(resultAttach);
        }
    }
}
