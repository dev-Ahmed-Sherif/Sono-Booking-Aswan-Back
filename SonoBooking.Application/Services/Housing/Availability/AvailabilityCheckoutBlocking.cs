using SonoBooking.Domain;
using System;

namespace SonoBooking.Application.Services.Housing.Availability;

/// <summary>
/// Checkout blocking instants for availability inquiry (noon rule vs early departure + 1h).
/// </summary>
internal static class AvailabilityCheckoutBlocking
{
    /// <summary>
    /// Planned or late checkout: block until 12:00:00 on the checkout day.
    /// Early checkout only: block until actual checkout + 1 hour.
    /// </summary>
    public static DateTime ResolveBlockingEndInstant(
        DateOnly reservationEndDate,
        DateTime actualCheckOutDate)
    {
        var checkoutDay = DateOnly.FromDateTime(actualCheckOutDate.Date);
        if (reservationEndDate == checkoutDay || checkoutDay > reservationEndDate)
            return checkoutDay.ToDateTime(new TimeOnly(12, 0, 0));

        return actualCheckOutDate.AddHours(1);
    }

    /// <summary>Date-only inquiry from the front is effective at 12:00:01 on that day.</summary>
    public static DateTime ResolveInquiryStartInstant(DateOnly inquiryStart) =>
        inquiryStart.ToDateTime(new TimeOnly(12, 0, 1));

    /// <summary>
    /// Parses inquiry start header: date-only → 12:00:01; datetime → use sent time.
    /// </summary>
    public static bool TryParseInquiryStartInstant(string header, out DateTime inquiryStart)
    {
        inquiryStart = default;
        if (string.IsNullOrWhiteSpace(header))
            return false;

        var trimmed = header.Trim();
        if (trimmed.Length > 10 && (trimmed.Contains('T') || trimmed.Contains(' ')))
        {
            if (DateTime.TryParse(trimmed, out var withTime))
            {
                inquiryStart = withTime;
                return true;
            }
        }

        if (DateOnly.TryParse(trimmed, out var dateOnly))
        {
            inquiryStart = dateOnly.ToDateTime(new TimeOnly(12, 0, 1));
            return true;
        }

        return false;
    }

    public static bool IsInquiryAfterBlocking(DateTime inquiryStart, DateTime? blockingEnd) =>
        !blockingEnd.HasValue || inquiryStart > blockingEnd.Value;

    /// <summary>
    /// Resolves whether a reservation blocks an inquiry and the blocking end instant.
    /// Early departure and checkout status use ActualCheckOutDate; in-stay uses EndDate noon;
    /// future stays and post-checkout (past blocking end) do not block.
    /// </summary>
    public static bool TryResolveReservationBlockingEnd(
        ReservationStatus status,
        DateOnly requestStartDate,
        DateOnly reservationEndDate,
        DateTime actualCheckOutDate,
        DateOnly inquiryStartDate,
        out DateTime blockingEnd)
    {
        blockingEnd = default;
        var checkoutDay = DateOnly.FromDateTime(actualCheckOutDate.Date);

        // Completed checkout: unit is bookable from 12:00:01 on the checkout day (same as planned checkout).
        // The +1h early-departure turnover applies only while checkout is still in progress.
        if (status == ReservationStatus.Checkout)
        {
            blockingEnd = checkoutDay.ToDateTime(new TimeOnly(12, 0, 0));
            return true;
        }

        if (checkoutDay < reservationEndDate)
        {
            blockingEnd = ResolveBlockingEndInstant(reservationEndDate, actualCheckOutDate);
            return true;
        }

        if (inquiryStartDate >= requestStartDate && inquiryStartDate < reservationEndDate)
        {
            blockingEnd = reservationEndDate.ToDateTime(new TimeOnly(12, 0, 0));
            return true;
        }

        return false;
    }
}
