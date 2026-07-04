using SonoBooking.Application.Services.Housing.Availability;
using SonoBooking.Common.DTO.Housing.Dashboard;
using SonoBooking.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SonoBooking.Application.Services.Housing.Dashboard;

internal static class DashboardOccupancyCalculator
{
    internal sealed class HousingStructure
    {
        public required List<ApartmentStructureRow> Apartments { get; init; }
    }

    internal sealed class ApartmentStructureRow
    {
        public required string Id { get; init; }
        public required string ApartmentNumber { get; init; }
        public List<RoomStructureRow> Rooms { get; init; } = [];
    }

    internal sealed class RoomStructureRow
    {
        public required string Id { get; init; }
        public required string ApartmentId { get; init; }
        public required UnitStatus Status { get; init; }
        public List<BedStructureRow> Beds { get; init; } = [];
    }

    internal sealed class BedStructureRow
    {
        public required string Id { get; init; }
        public required string RoomId { get; init; }
        public required UnitStatus Status { get; init; }
    }

    internal static (int OverallPercent, List<ApartmentOccupancyItemDto> Apartments) CalculateForDay(
        DateOnly day,
        DashboardOccupancyCache cache,
        HousingStructure structure,
        IUnitOccupancyService unitOccupancyService)
    {
        UnitBlockingEndIndex blockingIndex = cache.BuildBlockingIndexForDay(day);
        DateTime inquiryInstant = AvailabilityCheckoutBlocking.ResolveInquiryStartInstant(day);

        Dictionary<string, int> apartmentCapacity = [];
        Dictionary<string, int> apartmentOccupied = [];
        Dictionary<string, string> apartmentLabels = [];

        foreach (ApartmentStructureRow apartment in structure.Apartments)
        {
            apartmentLabels[apartment.Id] = $"شقة {apartment.ApartmentNumber}";
            apartmentCapacity.TryAdd(apartment.Id, 0);
            apartmentOccupied.TryAdd(apartment.Id, 0);

            foreach (RoomStructureRow room in apartment.Rooms)
            {
                if (room.Beds.Count > 0)
                {
                    foreach (BedStructureRow bed in room.Beds)
                    {
                        apartmentCapacity[apartment.Id] += 1;

                        bool blockedByStatus = bed.Status is UnitStatus.Occupied or UnitStatus.Reserved;
                        bool blockedByStay = !unitOccupancyService.IsUnitFreeOnInquiryStart(
                            inquiryInstant,
                            blockingIndex.GetBedBlockingEnd(bed.Id, bed.RoomId, apartment.Id));

                        if (blockedByStatus || blockedByStay)
                            apartmentOccupied[apartment.Id] += 1;
                    }
                }
                else
                {
                    apartmentCapacity[apartment.Id] += 1;

                    bool blockedByStatus = room.Status is UnitStatus.Occupied or UnitStatus.Reserved;
                    bool blockedByStay = !unitOccupancyService.IsUnitFreeOnInquiryStart(
                        inquiryInstant,
                        blockingIndex.GetRoomBlockingEnd(room.Id, apartment.Id));

                    if (blockedByStatus || blockedByStay)
                        apartmentOccupied[apartment.Id] += 1;
                }
            }
        }

        List<ApartmentOccupancyItemDto> apartmentOccupancy = [];
        int totalCapacity = 0;
        int totalOccupied = 0;

        foreach (ApartmentStructureRow apartment in structure.Apartments)
        {
            int capacity = apartmentCapacity.GetValueOrDefault(apartment.Id);
            int occupied = apartmentOccupied.GetValueOrDefault(apartment.Id);
            totalCapacity += capacity;
            totalOccupied += occupied;

            int percent = capacity == 0
                ? 0
                : (int)Math.Round(occupied * 100m / capacity, MidpointRounding.AwayFromZero);

            apartmentOccupancy.Add(new ApartmentOccupancyItemDto
            {
                UnitLabel = apartmentLabels[apartment.Id],
                Percent = percent
            });
        }

        int overallPercent = totalCapacity == 0
            ? 0
            : (int)Math.Round(totalOccupied * 100m / totalCapacity, MidpointRounding.AwayFromZero);

        return (overallPercent, apartmentOccupancy);
    }
}
