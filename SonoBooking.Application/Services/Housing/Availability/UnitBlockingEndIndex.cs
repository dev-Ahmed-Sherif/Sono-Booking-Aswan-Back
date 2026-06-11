using System;
using System.Collections.Generic;

namespace SonoBooking.Application.Services.Housing.Availability;

/// <summary>
/// Latest blocking end date (reservation ActualCheckOutDate, date only) per housing unit id.
/// </summary>
public sealed class UnitBlockingEndIndex
{
    public Dictionary<string, DateOnly> Beds { get; } =
        new(StringComparer.OrdinalIgnoreCase);

    public Dictionary<string, DateOnly> Rooms { get; } =
        new(StringComparer.OrdinalIgnoreCase);

    public Dictionary<string, DateOnly> Apartments { get; } =
        new(StringComparer.OrdinalIgnoreCase);

    internal static void SetMax(IDictionary<string, DateOnly> map, string id, DateOnly end)
    {
        if (string.IsNullOrWhiteSpace(id)) return;
        var key = id.Trim();
        if (map.TryGetValue(key, out var existing))
        {
            if (end > existing) map[key] = end;
        }
        else
        {
            map[key] = end;
        }
    }

    public DateOnly? GetBedBlockingEnd(string bedId, string roomId, string apartmentId)
    {
        DateOnly? best = null;
        if (!string.IsNullOrWhiteSpace(bedId) &&
            Beds.TryGetValue(bedId.Trim(), out var bedEnd))
        {
            best = bedEnd;
        }

        if (!string.IsNullOrWhiteSpace(roomId) &&
            Rooms.TryGetValue(roomId.Trim(), out var roomEnd))
        {
            best = Max(best, roomEnd);
        }

        if (!string.IsNullOrWhiteSpace(apartmentId) &&
            Apartments.TryGetValue(apartmentId.Trim(), out var aptEnd))
        {
            best = Max(best, aptEnd);
        }

        return best;
    }

    public DateOnly? GetRoomBlockingEnd(string roomId, string apartmentId)
    {
        DateOnly? best = null;
        if (!string.IsNullOrWhiteSpace(roomId) &&
            Rooms.TryGetValue(roomId.Trim(), out var roomEnd))
        {
            best = roomEnd;
        }

        if (!string.IsNullOrWhiteSpace(apartmentId) &&
            Apartments.TryGetValue(apartmentId.Trim(), out var aptEnd))
        {
            best = Max(best, aptEnd);
        }

        return best;
    }

    public DateOnly? GetApartmentBlockingEnd(string apartmentId) =>
        string.IsNullOrWhiteSpace(apartmentId) ||
        !Apartments.TryGetValue(apartmentId.Trim(), out var end)
            ? null
            : end;

    private static DateOnly? Max(DateOnly? a, DateOnly b) =>
        a == null || b > a.Value ? b : a;
}
