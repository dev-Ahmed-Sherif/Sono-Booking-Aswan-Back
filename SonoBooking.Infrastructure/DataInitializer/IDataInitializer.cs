using City = SonoBooking.Domain.Entities.Lookups.City;
using System.Collections.Generic;
using AllowedDayBeforeReservation = SonoBooking.Domain.Entities.Lookups.AllowedDayBeforeReservation;
using ApartmentType = SonoBooking.Domain.Entities.Lookups.ApartmentType;
using Employee = SonoBooking.Domain.Entities.Lookups.Employee;
using Governorate = SonoBooking.Domain.Entities.Lookups.Governorate;
using NotificationGroup = SonoBooking.Domain.Entities.BusinessNotification.NotificationGroup;
using Leader = SonoBooking.Domain.Entities.Housing.Leader;
using RequestType = SonoBooking.Domain.Entities.Lookups.RequestType;
using Relationship = SonoBooking.Domain.Entities.Lookups.Relationship;
using RoomType = SonoBooking.Domain.Entities.Lookups.RoomType;
using Role = SonoBooking.Domain.Entities.Identity.Role;

namespace SonoBooking.Infrastructure.DataInitializer
{
    public interface IDataInitializer
    {
        IEnumerable<Governorate> SeedGovernoratesAsync();

        IEnumerable<City> SeedCitiesAsync();

        IEnumerable<ApartmentType> SeedApartmentTypesAsync();

        IEnumerable<RoomType> SeedRoomTypesAsync();

        IEnumerable<AllowedDayBeforeReservation> SeedAllowedDayBeforeReservationsAsync();

        IEnumerable<RequestType> SeedRequestTypesAsync();

        IEnumerable<Relationship> SeedRelationshipsAsync();

        IEnumerable<Role> SeedRolesAsync();

        IEnumerable<Employee> SeedEmployeesAsync();

        IEnumerable<NotificationGroup> SeedNotificationGroupsAsync();

        IEnumerable<Leader> SeedLeadersAsync();

        //IEnumerable<Status> SeedStatusesAsync();
    }
}