using SonoBooking.Domain;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SonoBooking.Application.Services.Housing.Availability;

public interface IUnitOccupancyService
{
    Task<UnitBlockingEndIndex> BuildBlockingEndIndexAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyDictionary<string, string>> GetRoomApartmentIdsAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyDictionary<string, Gender>> GetApartmentGendersAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Unit is bookable when inquiry start is strictly after the latest blocking end.
    /// </summary>
    bool IsUnitFreeOnInquiryStart(DateOnly inquiryStart, DateOnly? blockingEnd);
}
