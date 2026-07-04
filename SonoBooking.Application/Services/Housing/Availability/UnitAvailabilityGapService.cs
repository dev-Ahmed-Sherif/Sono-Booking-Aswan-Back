using Microsoft.EntityFrameworkCore;
using SonoBooking.Application.Services.BackgroundJobs.Housing.Reservations;
using SonoBooking.Domain;
using SonoBooking.Domain.Entities.Base;
using SonoBooking.Domain.Entities.Housing;
using SonoBooking.Infrastructure.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SonoBooking.Application.Services.Housing.Availability;

public class UnitAvailabilityGapService(SonoBookingDbContext dbContext) : IUnitAvailabilityGapService
{
    private const string SystemActor = "System";

    public async Task RefreshGapAvailabilityForReservationAsync(
        string reservationId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(reservationId))
            return;

        var reservation = await dbContext.Reservations
            .AsNoTracking()
            .Where(r => !r.IsDeleted && r.Id == reservationId.Trim())
            .Select(r => new
            {
                r.RequestId,
                r.EndDate,
                r.ActualCheckOutDate,
                r.Status
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (reservation == null
            || reservation.Status is ReservationStatus.Canceled or ReservationStatus.NoShow
            || reservation.ActualCheckOutDate is null)
        {
            return;
        }

        DateTime blockingEnd = AvailabilityCheckoutBlocking.ResolveBlockingEndInstant(
            reservation.EndDate,
            reservation.ActualCheckOutDate.Value);
        DateOnly checkoutDay = DateOnly.FromDateTime(reservation.ActualCheckOutDate.Value.Date);

        if (checkoutDay >= reservation.EndDate)
            return;

        List<RequestUnit> sourceUnits = await dbContext.RequestUnits
            .AsNoTracking()
            .Where(ru => !ru.IsDeleted && ru.RequestId == reservation.RequestId)
            .ToListAsync(cancellationToken);

        if (sourceUnits.Count == 0)
            return;

        DateTime now = DateTime.Now;
        if (now < blockingEnd)
            return;

        bool changed = false;
        foreach (RequestUnit unit in sourceUnits)
        {
            DateOnly? nextStart = await FindNextApprovedStartOnUnitAsync(
                unit,
                checkoutDay,
                reservation.RequestId,
                cancellationToken);
            if (!nextStart.HasValue || nextStart.Value <= checkoutDay)
                continue;

            DateTime gapEndsAt = nextStart.Value.ToDateTime(new TimeOnly(12, 0, 0));
            if (now >= gapEndsAt)
                continue;

            if (await HasOtherActiveStayOnUnitAsync(unit, reservation.RequestId, now, cancellationToken))
                continue;

            changed |= await SetUnitAvailableInGapAsync(unit, cancellationToken);
        }

        if (changed)
            await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task OnActualCheckOutDateChangedAsync(
        string reservationId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(reservationId))
            return;

        var reservation = await dbContext.Reservations
            .AsNoTracking()
            .Where(r => !r.IsDeleted && r.Id == reservationId.Trim())
            .Select(r => new
            {
                r.EndDate,
                r.ActualCheckOutDate,
                r.Status
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (reservation == null
            || reservation.Status is ReservationStatus.Canceled or ReservationStatus.NoShow
            || reservation.ActualCheckOutDate is null)
        {
            return;
        }

        DateOnly checkoutDay = DateOnly.FromDateTime(reservation.ActualCheckOutDate.Value.Date);
        if (checkoutDay >= reservation.EndDate)
            return;

        DateTime blockingEnd = AvailabilityCheckoutBlocking.ResolveBlockingEndInstant(
            reservation.EndDate,
            reservation.ActualCheckOutDate.Value);

        if (DateTime.Now >= blockingEnd)
            await RefreshGapAvailabilityForReservationAsync(reservationId, cancellationToken);
        else
            ReservationUnitAvailabilityGapJob.ScheduleForReservation(reservationId, blockingEnd);
    }

    public async Task RefreshAllOpenGapsAsync(CancellationToken cancellationToken = default)
    {
        List<string> reservationIds = await dbContext.Reservations
            .AsNoTracking()
            .Where(r => !r.IsDeleted
                && r.ActualCheckOutDate != null
                && r.Status != ReservationStatus.Canceled
                && r.Status != ReservationStatus.NoShow
                && DateOnly.FromDateTime(r.ActualCheckOutDate.Value.Date) < r.EndDate)
            .Select(r => r.Id)
            .ToListAsync(cancellationToken);

        foreach (string reservationId in reservationIds)
            await RefreshGapAvailabilityForReservationAsync(reservationId, cancellationToken);
    }

    private async Task<DateOnly?> FindNextApprovedStartOnUnitAsync(
        RequestUnit unit,
        DateOnly afterDate,
        string excludeRequestId,
        CancellationToken cancellationToken)
    {
        IQueryable<RequestUnit> query = dbContext.RequestUnits
            .AsNoTracking()
            .Where(ru => !ru.IsDeleted && ru.RequestId != excludeRequestId);

        if (!string.IsNullOrWhiteSpace(unit.BedId))
            query = query.Where(ru => ru.BedId == unit.BedId);
        else if (!string.IsNullOrWhiteSpace(unit.RoomId))
            query = query.Where(ru => ru.RoomId == unit.RoomId);
        else if (!string.IsNullOrWhiteSpace(unit.ApartmentId))
            query = query.Where(ru => ru.ApartmentId == unit.ApartmentId);
        else
            return null;

        return await (
            from ru in query
            join req in dbContext.Requests.AsNoTracking() on ru.RequestId equals req.Id
            where !req.IsDeleted
                && req.Status == Status.Approved
                && req.StartDate > afterDate
            orderby req.StartDate
            select (DateOnly?)req.StartDate
        ).FirstOrDefaultAsync(cancellationToken);
    }

    private async Task<bool> HasOtherActiveStayOnUnitAsync(
        RequestUnit unit,
        string excludeRequestId,
        DateTime now,
        CancellationToken cancellationToken)
    {
        var stays = await (
            from ru in dbContext.RequestUnits.AsNoTracking()
            join req in dbContext.Requests.AsNoTracking() on ru.RequestId equals req.Id
            join res in dbContext.Reservations.AsNoTracking() on req.Id equals res.RequestId
            where !ru.IsDeleted
                && !req.IsDeleted
                && !res.IsDeleted
                && req.Id != excludeRequestId
                && req.Status == Status.Approved
                && res.Status != ReservationStatus.Canceled
                && res.Status != ReservationStatus.NoShow
            select new
            {
                ru.BedId,
                ru.RoomId,
                ru.ApartmentId,
                req.StartDate,
                res.EndDate,
                res.ActualCheckOutDate,
                res.Status
            }
        ).ToListAsync(cancellationToken);

        DateOnly today = DateOnly.FromDateTime(now.Date);

        foreach (var stay in stays)
        {
            if (!UnitMatches(unit, stay.BedId, stay.RoomId, stay.ApartmentId))
                continue;

            if (stay.ActualCheckOutDate is null)
            {
                if (stay.Status == ReservationStatus.Reserved
                    && stay.StartDate <= today
                    && stay.EndDate >= today)
                {
                    return true;
                }

                continue;
            }

            DateTime stayBlockingEnd = AvailabilityCheckoutBlocking.ResolveBlockingEndInstant(
                stay.EndDate,
                stay.ActualCheckOutDate.Value);
            if (now <= stayBlockingEnd)
                return true;
        }

        return false;
    }

    private static bool UnitMatches(
        RequestUnit unit,
        string? bedId,
        string? roomId,
        string? apartmentId)
    {
        if (!string.IsNullOrWhiteSpace(unit.BedId))
            return string.Equals(unit.BedId, bedId, StringComparison.OrdinalIgnoreCase);
        if (!string.IsNullOrWhiteSpace(unit.RoomId))
            return string.Equals(unit.RoomId, roomId, StringComparison.OrdinalIgnoreCase);
        if (!string.IsNullOrWhiteSpace(unit.ApartmentId))
            return string.Equals(unit.ApartmentId, apartmentId, StringComparison.OrdinalIgnoreCase);
        return false;
    }

    private async Task<bool> SetUnitAvailableInGapAsync(
        RequestUnit unit,
        CancellationToken cancellationToken)
    {
        bool changed = false;

        if (!string.IsNullOrWhiteSpace(unit.BedId))
        {
            Bed? bed = await dbContext.Beds
                .FirstOrDefaultAsync(b => !b.IsDeleted && b.Id == unit.BedId, cancellationToken);
            if (bed != null && bed.Status is UnitStatus.Occupied or UnitStatus.Reserved)
            {
                bed.Status = UnitStatus.Available;
                TouchAudit(bed);
                changed = true;
            }
        }

        if (!string.IsNullOrWhiteSpace(unit.RoomId))
        {
            Room? room = await dbContext.Rooms
                .FirstOrDefaultAsync(r => !r.IsDeleted && r.Id == unit.RoomId, cancellationToken);
            if (room != null && room.Status is UnitStatus.Occupied or UnitStatus.Reserved)
            {
                room.Status = UnitStatus.Available;
                TouchAudit(room);
                changed = true;
            }

            List<Bed> bedsInRoom = await dbContext.Beds
                .Where(b => !b.IsDeleted && b.RoomId == unit.RoomId)
                .ToListAsync(cancellationToken);
            foreach (Bed bed in bedsInRoom)
            {
                if (bed.Status is not (UnitStatus.Occupied or UnitStatus.Reserved)) continue;
                bed.Status = UnitStatus.Available;
                TouchAudit(bed);
                changed = true;
            }
        }

        if (!string.IsNullOrWhiteSpace(unit.ApartmentId))
        {
            Apartment? apartment = await dbContext.Apartments
                .FirstOrDefaultAsync(a => !a.IsDeleted && a.Id == unit.ApartmentId, cancellationToken);
            if (apartment != null && apartment.Status is UnitStatus.Occupied or UnitStatus.Reserved)
            {
                apartment.Status = UnitStatus.Available;
                TouchAudit(apartment);
                changed = true;
            }
        }

        return changed;
    }

    private static void TouchAudit(BaseAudit<string> entity)
    {
        entity.ModifiedAt = DateTime.UtcNow;
        entity.ModifiedBy = SystemActor;
        entity.ModifiedById = SystemActor;
    }
}
