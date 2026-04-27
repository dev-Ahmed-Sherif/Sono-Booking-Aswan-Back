using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SonoTracker.Api.Controllers.V1.Base;
using SonoTracker.Application.Services.Tracker.FloatingUnits;
using SonoTracker.Application.Services.Tracker.TripInformation;
using SonoTracker.Common.Core;
using SonoTracker.Common.DTO.Base;
using SonoTracker.Common.DTO.Tracker.FloatingUnit;
using SonoTracker.Common.DTO.Tracker.TripInformation;
using SonoTracker.Common.DTO.Tracker.TripInformation.Parameters;
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
    public class TripInformationController(
                    ITripInformationService tripInformationService, 
                    IFloatingUnitService floatingUnitService) : BaseController
    {
        /// <summary>
        /// Get By Id 
        /// </summary>
        /// <returns></returns>

        [HttpGet("get/{id}")]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status200OK)]
        public async Task<IFinalResult> GetAsync(string id, CancellationToken cancellationToken = default)
                                        => await tripInformationService.GetByIdAsync(id, cancellationToken);

        /// <summary>
        /// Get For Edit 
        /// </summary>
        /// <returns></returns>

        [HttpGet("getEdit/{id}")]
        public async Task<IFinalResult> GetEditAsync(string id, CancellationToken cancellationToken = default)
                                        => await tripInformationService.GetByIdForEditAsync(id, cancellationToken);

        /// <summary>
        /// Get All 
        /// </summary>
        /// <returns></returns>

        [HttpGet("getAll")]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status200OK)]
        public async Task<ActionResult<IFinalResult>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            IFinalResult res = await tripInformationService.GetAllAsync(cancellationToken: cancellationToken);

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
        public async Task<ActionResult<PagingResult>> GetPagedAsync([FromBody] BaseParam<TripInformationFilter> filter, CancellationToken cancellationToken = default)
        {
            PagingResult res = await tripInformationService.GetAllPagedAsync(filter, cancellationToken);

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
            PagingResult res = await tripInformationService.GetDropDownAsync(filter, cancellationToken);

            return Ok(res);
        }

        /// <summary>
        /// Add 
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost("add")]
        public async Task<IFinalResult> AddAsync([FromForm] AddTripInformationDto dto, CancellationToken cancellationToken = default)
        {
            string floatingUnitCode, TripCode = "";

            var floatingUnit = await floatingUnitService.GetByIdAsync(dto.FloatingUnitId, cancellationToken);

            // Fix: Cast the floatingUnit.Data to the appropriate type that contains the 'Code' property.
            if (floatingUnit.Data is FloatingUnitDto floatingUnitDto)
            {
                floatingUnitCode = floatingUnitDto.Code;

                TripInformationFilter filter = new()
                {
                    FloatingUnitId = dto.FloatingUnitId,
                    IsDeleted = false
                };

                // Check if the trip information already exists for the given floating unit id
                var exist = await tripInformationService.GetAllFilterAsync(filter, cancellationToken);
                // Fix: Explicitly cast exist.Data to a collection type to access the Count property.
                var existDataCollection = exist.Data as ICollection<TripInformationDto>;

                if (existDataCollection?.Count > 0)
                {
                    // Handle existing trip information logic here (if needed)
                    var lastTrip = existDataCollection.LastOrDefault();
                    if (lastTrip != null && lastTrip.Code.StartsWith(floatingUnitCode))
                    {
                        // Extract the numeric part and increment it
                        var numericPart = lastTrip.Code[floatingUnitCode.Length..];
                        if (int.TryParse(numericPart, out int number))
                        {
                            number++;
                            TripCode = floatingUnitCode + number.ToString("D4"); // Ensure 4 digits
                        }
                    }
                }
                else
                {
                    TripCode = floatingUnitCode + "0001";
                }
            }

            AddTripInformationDto addTripInformation = new()
            {
                Code = TripCode,
                FloatingUnitId = dto.FloatingUnitId,
                RouteId = dto.RouteId,
                SartDate = dto.SartDate,
                EndDate = dto.EndDate,
                StaffNumber = dto.StaffNumber,
                PassengerNumber = dto.PassengerNumber,
                PassengerAttachment = dto.PassengerAttachment
            };

            var res = await tripInformationService.AddAsync(addTripInformation, cancellationToken);

            return res;
        }

        /// <summary>
        /// Update  
        /// </summary>
        /// <param name="model">Object content</param>
        /// <returns></returns>
        [HttpPut("update")]
        public async Task<IFinalResult> UpdateAsync([FromForm] AddTripInformationDto model, CancellationToken cancellationToken = default)
                                        => await tripInformationService.UpdateAsync(model, cancellationToken);

        /// <summary>
        /// Remove  by id
        /// </summary>
        /// <param name="id">PK</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpDelete("delete/{id}")]
        public async Task<IFinalResult> DeleteAsync(string id, CancellationToken cancellationToken = default)
                                        => await tripInformationService.DeleteAsync(id, cancellationToken);

        /// <summary>
        /// Soft Remove by id
        /// </summary>
        /// <param name="id">PK</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpDelete("deleteSoft/{id}")]
        public async Task<IFinalResult> DeleteSoftAsync(string id, CancellationToken cancellationToken = default)
                                        => await tripInformationService.DeleteSoftAsync(id, cancellationToken);

        /// <summary>
        /// Remove Range by Organization Ids
        /// </summary>
        /// <param name="ids">PK</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpDelete("deleteRange")]
        public async Task<IFinalResult> DeleteRangeAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default)
                                        => await tripInformationService.DeleteRangeAsync(ids, cancellationToken);
    }
}
