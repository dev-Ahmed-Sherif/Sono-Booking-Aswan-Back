using City = SonoBooking.Domain.Entities.Lookups.City;
using System.Collections.Generic;
using ApartmentType = SonoBooking.Domain.Entities.Lookups.ApartmentType;
using Governorate = SonoBooking.Domain.Entities.Lookups.Governorate;
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

        IEnumerable<RequestType> SeedRequestTypesAsync();

        IEnumerable<Relationship> SeedRelationshipsAsync();

        IEnumerable<Role> SeedRolesAsync();

        //IEnumerable<Status> SeedStatusesAsync();
    }
}