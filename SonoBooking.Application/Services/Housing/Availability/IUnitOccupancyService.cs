using SonoBooking.Domain;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SonoBooking.Application.Services.Housing.Availability;

public interface IUnitOccupancyService
{
    Task<UnitBlockingEndIndex> BuildBlockingEndIndexAsync(
        DateOnly? inquiryStart = null,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyDictionary<string, string>> GetRoomApartmentIdsAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyDictionary<string, Gender>> GetApartmentGendersAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Apartment ids that have at least one room or bed free for the inquiry window.
    /// </summary>
    Task<HashSet<string>> GetApartmentIdsWithAvailableChildrenAsync(
        DateOnly inquiryStart,
        int nights,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// For flexible apartments, returns extra allowed genders based on child-unit state/occupancy.
    /// </summary>
    Task<IReadOnlyDictionary<string, IReadOnlySet<Gender>>> GetFlexibleApartmentAllowedGendersAsync(
        IEnumerable<string> apartmentIds,
        DateOnly inquiryStart,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Unit is bookable when inquiry start is strictly after the latest blocking end.
    /// </summary>
    bool IsUnitFreeOnInquiryStart(DateOnly inquiryStart, DateOnly? blockingEnd);

    /// <summary>
    /// Inquiry end date (`inquiryStart + nights`) must be strictly before the next approved request start.
    /// </summary>
    bool IsUnitFreeForInquiryWindow(
        DateOnly inquiryStart,
        int nights,
        DateOnly? blockingEnd,
        DateOnly? nextApprovedStart);
}
