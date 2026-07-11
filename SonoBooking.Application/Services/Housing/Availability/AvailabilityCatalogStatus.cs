using SonoBooking.Domain;

using SonoBooking.Domain.Entities.Housing;

using System;

using System.Linq.Expressions;



namespace SonoBooking.Application.Services.Housing.Availability;



/// <summary>

/// Unit catalog status rules for availability inquiry (<c>getAll</c> with <c>Status: Available</c>).

/// </summary>

public static class AvailabilityCatalogStatus

{

    /// <summary>

    /// When an inquiry start date is provided, include reserved/occupied units and let the

    /// occupancy date filter decide; otherwise only catalog-available units are returned.

    /// </summary>

    public static bool MatchesInquiry(UnitStatus status, bool hasInquiryStartDate) =>

        hasInquiryStartDate

            ? status is UnitStatus.Available or UnitStatus.Reserved or UnitStatus.Occupied

            : status == UnitStatus.Available;



    public static Expression<Func<Apartment, bool>> ApartmentMatchesInquiry(

        bool hasInquiryStartDate,

        bool excludeAdministrative = true) =>

        hasInquiryStartDate

            ? a => (!excludeAdministrative || !a.AdministrativeStatus)

                   && (a.Status == UnitStatus.Available || a.Status == UnitStatus.Reserved || a.Status == UnitStatus.Occupied)

            : a => (!excludeAdministrative || !a.AdministrativeStatus)

                   && a.Status == UnitStatus.Available;



    public static Expression<Func<Room, bool>> RoomMatchesInquiry(

        bool hasInquiryStartDate,

        string? apartmentId = null,

        bool excludeAdministrative = true) =>

        string.IsNullOrWhiteSpace(apartmentId)

            ? RoomStatusMatchesInquiry(hasInquiryStartDate, excludeAdministrative)

            : hasInquiryStartDate

                ? r => r.ApartmentId == apartmentId

                       && (!excludeAdministrative || !r.AdministrativeStatus)

                       && (r.Status == UnitStatus.Available || r.Status == UnitStatus.Reserved || r.Status == UnitStatus.Occupied)

                : r => r.ApartmentId == apartmentId

                       && (!excludeAdministrative || !r.AdministrativeStatus)

                       && r.Status == UnitStatus.Available;



    public static Expression<Func<Bed, bool>> BedMatchesInquiry(

        bool hasInquiryStartDate,

        string? roomId = null,

        bool excludeAdministrative = true) =>

        string.IsNullOrWhiteSpace(roomId)

            ? BedStatusMatchesInquiry(hasInquiryStartDate, excludeAdministrative)

            : hasInquiryStartDate

                ? b => b.RoomId == roomId

                       && (!excludeAdministrative || !b.AdministrativeStatus)

                       && (b.Status == UnitStatus.Available || b.Status == UnitStatus.Reserved || b.Status == UnitStatus.Occupied)

                : b => b.RoomId == roomId

                       && (!excludeAdministrative || !b.AdministrativeStatus)

                       && b.Status == UnitStatus.Available;



    private static Expression<Func<Room, bool>> RoomStatusMatchesInquiry(

        bool hasInquiryStartDate,

        bool excludeAdministrative) =>

        hasInquiryStartDate

            ? r => (!excludeAdministrative || !r.AdministrativeStatus)

                   && (r.Status == UnitStatus.Available || r.Status == UnitStatus.Reserved || r.Status == UnitStatus.Occupied)

            : r => (!excludeAdministrative || !r.AdministrativeStatus)

                   && r.Status == UnitStatus.Available;



    private static Expression<Func<Bed, bool>> BedStatusMatchesInquiry(

        bool hasInquiryStartDate,

        bool excludeAdministrative) =>

        hasInquiryStartDate

            ? b => (!excludeAdministrative || !b.AdministrativeStatus)

                   && (b.Status == UnitStatus.Available || b.Status == UnitStatus.Reserved || b.Status == UnitStatus.Occupied)

            : b => (!excludeAdministrative || !b.AdministrativeStatus)

                   && b.Status == UnitStatus.Available;

}


