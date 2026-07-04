using System;
using System.Collections.Generic;
using System.Linq;

namespace SonoBooking.Application.Services.Housing.Availability;

/// <summary>
/// Blocking end instants and approved stay starts per housing unit id.
/// </summary>
public sealed class UnitBlockingEndIndex
{
    public Dictionary<string, DateTime> Beds { get; } =
        new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, List<DateOnly>> BedApprovedStarts { get; } =
        new(StringComparer.OrdinalIgnoreCase);

    public Dictionary<string, DateTime> Rooms { get; } =
        new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, List<DateOnly>> RoomApprovedStarts { get; } =
        new(StringComparer.OrdinalIgnoreCase);

    public Dictionary<string, DateTime> Apartments { get; } =
        new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, List<DateOnly>> ApartmentApprovedStarts { get; } =
        new(StringComparer.OrdinalIgnoreCase);

    internal static void SetMax(IDictionary<string, DateTime> map, string id, DateTime end)
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

    internal static void AddApprovedStart(IDictionary<string, List<DateOnly>> map, string id, DateOnly start)
    {
        if (string.IsNullOrWhiteSpace(id)) return;
        var key = id.Trim();
        if (!map.TryGetValue(key, out List<DateOnly>? list))
        {
            list = [];
            map[key] = list;
        }

        if (!list.Contains(start))
            list.Add(start);
    }

    public DateTime? GetBedBlockingEnd(string bedId, string roomId, string apartmentId)
    {
        DateTime? best = null;
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

    public DateTime? GetRoomBlockingEnd(string roomId, string apartmentId)
    {
        DateTime? best = null;
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

    public DateTime? GetApartmentBlockingEnd(string apartmentId) =>
        string.IsNullOrWhiteSpace(apartmentId) ||
        !Apartments.TryGetValue(apartmentId.Trim(), out var end)
            ? null
            : end;

    /// <summary>Earliest approved stay start strictly after <paramref name="afterDate"/>.</summary>
    public DateOnly? GetBedNextApprovedStartAfter(
        string bedId,
        string roomId,
        string apartmentId,
        DateOnly afterDate) =>
        MinStartAfter(
            afterDate,
            StartsFor(BedApprovedStarts, bedId),
            StartsFor(RoomApprovedStarts, roomId),
            StartsFor(ApartmentApprovedStarts, apartmentId));

    public DateOnly? GetRoomNextApprovedStartAfter(
        string roomId,
        string apartmentId,
        DateOnly afterDate) =>
        MinStartAfter(
            afterDate,
            StartsFor(RoomApprovedStarts, roomId),
            StartsFor(ApartmentApprovedStarts, apartmentId));

    public DateOnly? GetApartmentNextApprovedStartAfter(string apartmentId, DateOnly afterDate) =>
        MinStartAfter(afterDate, StartsFor(ApartmentApprovedStarts, apartmentId));

    public DateOnly? GetBedNextApprovedStart(
        string bedId,
        string roomId,
        string apartmentId,
        DateOnly inquiryDate) =>
        GetBedNextApprovedStartAfter(bedId, roomId, apartmentId, inquiryDate);

    public DateOnly? GetRoomNextApprovedStart(
        string roomId,
        string apartmentId,
        DateOnly inquiryDate) =>
        GetRoomNextApprovedStartAfter(roomId, apartmentId, inquiryDate);

    public DateOnly? GetApartmentNextApprovedStart(string apartmentId, DateOnly inquiryDate) =>
        GetApartmentNextApprovedStartAfter(apartmentId, inquiryDate);

    public bool HasDirectApartmentBooking(string apartmentId)
    {
        if (string.IsNullOrWhiteSpace(apartmentId)) return false;
        var key = apartmentId.Trim();
        return Apartments.ContainsKey(key) || ApartmentApprovedStarts.ContainsKey(key);
    }

    public bool HasDirectBedBooking(string bedId) =>
        !string.IsNullOrWhiteSpace(bedId) && Beds.ContainsKey(bedId.Trim());

    public bool HasDirectRoomBooking(string roomId) =>
        !string.IsNullOrWhiteSpace(roomId) && Rooms.ContainsKey(roomId.Trim());

    private static IEnumerable<DateOnly> StartsFor(
        IReadOnlyDictionary<string, List<DateOnly>> map,
        string id)
    {
        if (string.IsNullOrWhiteSpace(id)) yield break;
        if (!map.TryGetValue(id.Trim(), out List<DateOnly>? list)) yield break;
        foreach (DateOnly start in list)
            yield return start;
    }

    private static DateOnly? MinStartAfter(DateOnly afterDate, params IEnumerable<DateOnly>[] sources)
    {
        DateOnly? best = null;
        foreach (IEnumerable<DateOnly> source in sources)
        {
            foreach (DateOnly start in source)
            {
                if (start <= afterDate) continue;
                if (!best.HasValue || start < best.Value)
                    best = start;
            }
        }

        return best;
    }

    private static DateTime? Max(DateTime? a, DateTime b) =>
        a == null || b > a.Value ? b : a;
}
