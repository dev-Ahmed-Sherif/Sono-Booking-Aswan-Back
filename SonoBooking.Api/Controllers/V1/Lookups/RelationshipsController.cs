using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SonoBooking.Api.Controllers.V1.Base;
using SonoBooking.Application.Services.LookUp.Relationship;
using SonoBooking.Common.Core;
using SonoBooking.Common.DTO.Base;
using SonoBooking.Common.DTO.Lookup.Relationship;
using SonoBooking.Common.DTO.Lookup.Relationship.Parameters;
using System.Net;
using System.Threading;

namespace SonoBooking.Api.Controllers.V1.Lookups
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]

    /// <summary>
    /// Controller for managing relationship lookups.
    /// Provides endpoints to retrieve, list, page, add, update and delete relationships.
    /// </summary>
    /// 
    public class RelationshipsController : BaseController
    {
        private readonly IRelationshipService relationshipService;

        /// <summary>
        /// Initializes a new instance of the <see cref="RelationshipsController"/> class.
        /// </summary>
        /// <param name="relationshipService">Service used to perform relationship operations.</param>
        public RelationshipsController(IRelationshipService relationshipService)
        {
            this.relationshipService = relationshipService;
        }

        /// <summary>
        /// Retrieves a relationship by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the relationship.</param>
        /// <param name="cancellationToken">A CancellationToken to cancel the operation.</param>
        /// <returns>An ActionResult wrapping an IFinalResult containing the operation result.</returns>
        [HttpGet("get/{id}")]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status200OK)]
        public async Task<ActionResult<IFinalResult>> GetAsync(string id, CancellationToken cancellationToken = default)
        {
            IFinalResult res = await relationshipService.GetByIdAsync(id, cancellationToken);
            return Ok(res);
        }

        /// <summary>
        /// Retrieves all relationships.
        /// </summary>
        /// <param name="cancellationToken">A CancellationToken to cancel the operation.</param>
        /// <returns>An ActionResult wrapping an IFinalResult containing the operation result.</returns>
        [HttpGet("getAll")]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status200OK)]
        public async Task<ActionResult<IFinalResult>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            IFinalResult res = await relationshipService.GetAllAsync(cancellationToken: cancellationToken);
            return Ok(res);
        }

        [HttpPost("getPaged")]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status200OK)]
        public async Task<ActionResult<PagingResult>> GetPagedAsync([FromBody] BaseParam<RelationshipFilter> filter, CancellationToken cancellationToken = default)
        {
            PagingResult res = await relationshipService.GetAllPagedAsync(filter, cancellationToken);
            return Ok(res);
        }

        [HttpPost("getDropDown")]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status200OK)]
        public async Task<ActionResult<PagingResult>> GetDropDownAsync([FromBody] BaseParam<SearchCriteriaFilter> filter, CancellationToken cancellationToken = default)
        {
            PagingResult res = await relationshipService.GetDropDownAsync(filter, cancellationToken);
            return Ok(res);
        }

        [HttpPost("add")]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status201Created)]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<IFinalResult>> AddAsync([FromBody] AddRelationshipDto dto, CancellationToken cancellationToken = default)
        {
            IFinalResult res = await relationshipService.AddAsync(dto, cancellationToken);

            if (res.Status == HttpStatusCode.BadRequest) return BadRequest(res);
            if (res.Status == HttpStatusCode.Conflict) return Conflict(res);

            return Created(string.Empty, res);
        }

        [HttpPut("update")]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status202Accepted)]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<IFinalResult>> UpdateAsync([FromBody] AddRelationshipDto model, CancellationToken cancellationToken = default)
        {
            IFinalResult res = await relationshipService.UpdateAsync(model, cancellationToken);

            if (res.Status == HttpStatusCode.BadRequest) return BadRequest(res);
            if (res.Status == HttpStatusCode.Conflict) return Conflict(res);

            return Accepted(res);
        }

        [HttpDelete("delete/{id}")]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status202Accepted)]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IFinalResult>> DeleteAsync(string id, CancellationToken cancellationToken = default)
        {
            IFinalResult res = await relationshipService.DeleteAsync(id, cancellationToken);

            if (res.Status == HttpStatusCode.BadRequest) return BadRequest(res);

            return Accepted(res);
        }

        [HttpDelete("deleteSoft/{id}")]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status202Accepted)]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IFinalResult>> DeleteSoftAsync(string id, CancellationToken cancellationToken = default)
        {
            IFinalResult res = await relationshipService.DeleteSoftAsync(id, cancellationToken);

            if (res.Status == HttpStatusCode.BadRequest) return BadRequest(res);

            return Accepted(res);
        }
    }
}
