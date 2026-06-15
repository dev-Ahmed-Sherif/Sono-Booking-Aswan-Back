using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SonoBooking.Api.Controllers.V1.Base;
using SonoBooking.Application.Services.Housing.Requests;
using SonoBooking.Common.Core;
using SonoBooking.Common.DTO.Base;
using SonoBooking.Common.DTO.Housing.Request;
using SonoBooking.Common.DTO.Housing.Request.Parameters;
using SonoBooking.Common.DTO.Housing.RequestAttach;
using SonoBooking.Common.DTO.Reports.Requests;
using SonoBooking.Common.Helpers;
using System.Net;
using System.Net.Mime;
using System.Reflection.Metadata;
using System.Threading;

namespace SonoBooking.Api.Controllers.V1.Housing
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    public class RequestsController(IRequestService requestService) : BaseController
    {
        [HttpGet("get/{id}")]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status200OK)]
        public async Task<ActionResult<IFinalResult>> GetAsync(string id, CancellationToken cancellationToken = default)
        {
            IFinalResult res = await requestService.GetByIdAsync(id, cancellationToken);
            return Ok(res);
        }

        [HttpGet("getAll")]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status200OK)]
        public async Task<ActionResult<IFinalResult>> GetAllAsync(
            [FromHeader(Name = "UserId")] string userId = null,
            CancellationToken cancellationToken = default)
        {
            IFinalResult res = await requestService.GetAllAsync(cancellationToken: cancellationToken);
            return Ok(res);
        }

        [HttpPost("getPaged")]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status200OK)]
        public async Task<ActionResult<PagingResult>> GetPagedAsync([FromBody] BaseParam<RequestFilter> filter, CancellationToken cancellationToken = default)
        {
            PagingResult res = await requestService.GetAllPagedAsync(filter, cancellationToken);
            return Ok(res);
        }

        [HttpPost("add")]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status201Created)]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<IFinalResult>> AddAsync([FromForm] AddRequestDto dto, CancellationToken cancellationToken = default)
        {
            BindRequestImagesFromFormFiles(dto);
            IFinalResult res = await requestService.AddAsync(dto, cancellationToken);

            if (res.Status == HttpStatusCode.BadRequest) return BadRequest(res);
            if (res.Status == HttpStatusCode.Conflict) return Conflict(res);

            return Created(string.Empty, res);
        }

        [HttpPut("update")]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status202Accepted)]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<IFinalResult>> UpdateAsync([FromForm] AddRequestDto model, CancellationToken cancellationToken = default)
        {
            BindRequestImagesFromFormFiles(model);
            IFinalResult res = await requestService.UpdateAsync(model, cancellationToken);

            if (res.Status == HttpStatusCode.BadRequest) return BadRequest(res);
            if (res.Status == HttpStatusCode.Conflict) return Conflict(res);

            return Accepted(res);
        }

        [HttpDelete("delete/{id}")]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status202Accepted)]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IFinalResult>> DeleteAsync(string id, CancellationToken cancellationToken = default)
        {
            IFinalResult res = await requestService.DeleteAsync(id, cancellationToken);

            if (res.Status == HttpStatusCode.BadRequest) return BadRequest(res);

            return Accepted(res);
        }

        [HttpDelete("deleteSoft/{id}")]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status202Accepted)]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IFinalResult>> DeleteSoftAsync(string id, CancellationToken cancellationToken = default)
        {
            IFinalResult res = await requestService.DeleteSoftAsync(id, cancellationToken);

            if (res.Status == HttpStatusCode.BadRequest) return BadRequest(res);

            return Accepted(res);
        }

        /// <summary>
        /// Generates a project report based on the provided filter.
        /// </summary>
        /// <param name="filter">The filter containing report parameters.</param>
        /// <param name="cancellationToken">Token to observe while waiting for the task to complete.</param>
        /// <returns>A PDF file containing the generated report.</returns>
        
        [HttpGet("GetReport")]
        [ProducesResponseType<Blob>(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetProjectReportData([FromQuery] FilterRequestReportDto filter, CancellationToken cancellationToken = default)
        {
            var report = await requestService.GenerateReportAsync(filter, cancellationToken);
            return File(report, MediaTypeNames.Application.Pdf, ReportHelper.GetReportDetails(filter.ReportName, filter.ReportType));
        }

        /// <summary>
        /// ASP.NET sometimes fails to bind nested <c>Images[n].Image</c> files on large multipart bodies.
        /// Fall back to raw <see cref="IFormFile"/> entries from the form file collection.
        /// </summary>
        private void BindRequestImagesFromFormFiles(AddRequestDto dto)
        {
            if (dto.Images != null && dto.Images.Exists(i => i?.Image != null && i.Image.Length > 0))
                return;

            IFormFileCollection files = Request.Form.Files;
            if (files == null || files.Count == 0)
                return;

            List<AddRequestAttachDto> bound = [];
            foreach (IFormFile file in files)
            {
                if (file == null || file.Length <= 0)
                    continue;

                string name = file.Name ?? string.Empty;
                if (!name.Contains("Image", StringComparison.OrdinalIgnoreCase))
                    continue;

                bound.Add(new AddRequestAttachDto { Image = file });
            }

            if (bound.Count > 0)
                dto.Images = bound;
        }
    }
}
