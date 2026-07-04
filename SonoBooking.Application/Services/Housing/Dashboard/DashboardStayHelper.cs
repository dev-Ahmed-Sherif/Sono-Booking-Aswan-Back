using SonoBooking.Domain;
using SonoBooking.Domain.Entities.Housing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SonoBooking.Application.Services.Housing.Dashboard;

internal static class DashboardStayHelper
{
    internal sealed class StayContext
    {
        public required Dictionary<string, DateOnly> RequestEndById { get; init; }
        public required Dictionary<string, DateOnly> RequestStartById { get; init; }
        public required HashSet<string> ApprovedRequestIds { get; init; }
        public required HashSet<string> BlockedReservationRequestIds { get; init; }
        public required List<RequestUnit> RequestUnits { get; init; }
    }

    internal static bool IsStayActiveOnDay(
        DateOnly day,
        string requestId,
        StayContext context)
    {
        if (string.IsNullOrWhiteSpace(requestId))
            return false;

        var key = requestId.Trim();
        if (!context.ApprovedRequestIds.Contains(key))
            return false;

        if (context.BlockedReservationRequestIds.Contains(key))
            return false;

        if (!context.RequestStartById.TryGetValue(key, out DateOnly startDate))
            return false;

        if (!context.RequestEndById.TryGetValue(key, out DateOnly endDate))
            return false;

        if (startDate > day)
            return false;

        return endDate >= day;
    }

    internal static HashSet<string> GetActiveRequestIdsOnDay(DateOnly day, StayContext context)
    {
        HashSet<string> activeIds = new(StringComparer.OrdinalIgnoreCase);

        foreach (string requestId in context.ApprovedRequestIds)
        {
            if (IsStayActiveOnDay(day, requestId, context))
                activeIds.Add(requestId);
        }

        return activeIds;
    }

    internal static void MergeRequestEnd(IDictionary<string, DateOnly> requestEndById, string requestId, DateOnly endYmd)
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

    internal static string ResolveRootStayRequestId(
        string requestId,
        IReadOnlyDictionary<string, string?> previousRequestById)
    {
        string current = requestId.Trim();
        HashSet<string> visited = new(StringComparer.OrdinalIgnoreCase);

        while (previousRequestById.TryGetValue(current, out string? previous)
            && !string.IsNullOrWhiteSpace(previous))
        {
            if (!visited.Add(current))
                break;

            current = previous.Trim();
        }

        return current;
    }
}
