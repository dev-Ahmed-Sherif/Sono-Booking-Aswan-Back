using SonoBooking.Application.Services.Housing.Availability;
using SonoBooking.Domain;
using SonoBooking.Domain.Entities.Housing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SonoBooking.Application.Services.Housing.Dashboard;

/// <summary>
/// Preloaded occupancy data so daily dashboard stats can build blocking indexes in memory.
/// </summary>
internal sealed class DashboardOccupancyCache
{
    private readonly Dictionary<string, DateOnly> _requestEndById;
    private readonly Dictionary<string, DateOnly> _requestStartById;
    private readonly List<RequestUnit> _requestUnits;

    private DashboardOccupancyCache(
        Dictionary<string, DateOnly> requestEndById,
        Dictionary<string, DateOnly> requestStartById,
        List<RequestUnit> requestUnits)
    {
        _requestEndById = requestEndById;
        _requestStartById = requestStartById;
        _requestUnits = requestUnits;
    }

    internal static DashboardOccupancyCache Create(
        List<ReservationEndRow> reservations,
        List<ExtensionEndRow> extensions,
        List<ApprovedRequestRow> approvedRequests,
        Dictionary<string, string?> previousRequestById,
        List<RequestUnit> requestUnits)
    {
        Dictionary<string, string> requestIdByReservationId = reservations
            .Where(r => !string.IsNullOrWhiteSpace(r.ReservationId))
            .ToDictionary(
                r => r.ReservationId!.Trim(),
                r => r.RequestId.Trim(),
                StringComparer.OrdinalIgnoreCase);

        Dictionary<string, DateOnly> requestEndById = [];
        Dictionary<string, DateOnly> requestStartById = approvedRequests
            .ToDictionary(r => r.Id.Trim(), r => r.StartDate, StringComparer.OrdinalIgnoreCase);

        foreach (ApprovedRequestRow request in approvedRequests)
        {
            MergeRequestEnd(requestEndById, request.Id, request.EndDate);
        }

        foreach (ReservationEndRow reservation in reservations
            .Where(r => r.Status != ReservationStatus.Canceled && r.Status != ReservationStatus.NoShow))
        {
            MergeRequestEnd(requestEndById, reservation.RequestId, reservation.EffectiveEndDay);
        }

        foreach (ExtensionEndRow extension in extensions)
        {
            if (string.IsNullOrWhiteSpace(extension.ReservationId))
                continue;

            if (!requestIdByReservationId.TryGetValue(extension.ReservationId.Trim(), out string? requestId))
                continue;

            MergeRequestEnd(requestEndById, requestId, extension.EndDate);
        }

        foreach (ApprovedRequestRow request in approvedRequests
            .Where(r => r.RequestCatagory == RequestCatagory.Extension
                && !string.IsNullOrWhiteSpace(r.PreviousRequestId)))
        {
            string rootId = DashboardStayHelper.ResolveRootStayRequestId(
                request.PreviousRequestId!,
                previousRequestById);
            MergeRequestEnd(requestEndById, rootId, request.EndDate);
            MergeRequestEnd(requestEndById, request.Id, request.EndDate);
        }

        return new DashboardOccupancyCache(requestEndById, requestStartById, requestUnits);
    }

    internal UnitBlockingEndIndex BuildBlockingIndexForDay(DateOnly day)
    {
        UnitBlockingEndIndex index = new();

        foreach (RequestUnit unit in _requestUnits)
        {
            if (string.IsNullOrWhiteSpace(unit.RequestId))
                continue;

            string requestId = unit.RequestId.Trim();
            if (!_requestEndById.TryGetValue(requestId, out DateOnly endYmd))
                continue;

            if (!_requestStartById.TryGetValue(requestId, out DateOnly requestStart))
                continue;

            if (endYmd < day || requestStart > day)
                continue;

            if (!string.IsNullOrWhiteSpace(unit.BedId))
            {
                UnitBlockingEndIndex.SetMax(
                    index.Beds,
                    unit.BedId,
                    endYmd.ToDateTime(new TimeOnly(12, 0, 0)));
                UnitBlockingEndIndex.AddApprovedStart(index.BedApprovedStarts, unit.BedId, requestStart);
                continue;
            }

            if (!string.IsNullOrWhiteSpace(unit.RoomId))
            {
                UnitBlockingEndIndex.SetMax(
                    index.Rooms,
                    unit.RoomId,
                    endYmd.ToDateTime(new TimeOnly(12, 0, 0)));
                UnitBlockingEndIndex.AddApprovedStart(index.RoomApprovedStarts, unit.RoomId, requestStart);
                continue;
            }

            if (!string.IsNullOrWhiteSpace(unit.ApartmentId))
            {
                UnitBlockingEndIndex.SetMax(
                    index.Apartments,
                    unit.ApartmentId,
                    endYmd.ToDateTime(new TimeOnly(12, 0, 0)));
                UnitBlockingEndIndex.AddApprovedStart(index.ApartmentApprovedStarts, unit.ApartmentId, requestStart);
            }
        }

        return index;
    }

    private static void MergeRequestEnd(IDictionary<string, DateOnly> requestEndById, string requestId, DateOnly endYmd)
    {
        if (string.IsNullOrWhiteSpace(requestId))
            return;

        string key = requestId.Trim();
        if (requestEndById.TryGetValue(key, out DateOnly current))
        {
            if (endYmd > current)
                requestEndById[key] = endYmd;
        }
        else
        {
            requestEndById[key] = endYmd;
        }
    }

    internal sealed class ReservationEndRow
    {
        public string? ReservationId { get; init; }
        public required string RequestId { get; init; }
        public required DateOnly EffectiveEndDay { get; init; }
        public required ReservationStatus Status { get; init; }
    }

    internal sealed class ExtensionEndRow
    {
        public required string ReservationId { get; init; }
        public required DateOnly EndDate { get; init; }
    }

    internal sealed class ApprovedRequestRow
    {
        public required string Id { get; init; }
        public required DateOnly StartDate { get; init; }
        public required DateOnly EndDate { get; init; }
        public string? PreviousRequestId { get; init; }
        public required RequestCatagory RequestCatagory { get; init; }
    }
}
