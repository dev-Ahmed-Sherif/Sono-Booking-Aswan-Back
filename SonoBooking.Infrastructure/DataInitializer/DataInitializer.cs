using System.Collections.Generic;
using System.IO;
using SonoTracker.Common.Extensions;
using SonoBooking.Domain.Entities.Lookups;
using ApartmentType = SonoBooking.Domain.Entities.Lookups.ApartmentType;
using City = SonoBooking.Domain.Entities.Lookups.City;
using Governorate = SonoBooking.Domain.Entities.Lookups.Governorate;
using RequestType = SonoBooking.Domain.Entities.Lookups.RequestType;
using RoomType = SonoBooking.Domain.Entities.Lookups.RoomType;
using Role = SonoBooking.Domain.Entities.Identity.Role;

namespace SonoBooking.Infrastructure.DataInitializer
{
    public class DataInitializer(string contentRootPath) : IDataInitializer
    {
       
        public IEnumerable<Governorate> SeedGovernoratesAsync()
        {
            var path = Path.Combine(contentRootPath, "Seed", "Governorates.json");
            var dataText = File.ReadAllText(path);
            var governorates = Seeder<List<Governorate>>.SeedIt(dataText);
            return governorates ?? [];
        }

        public IEnumerable<City> SeedCitiesAsync()
        {
            var path = Path.Combine(contentRootPath, "Seed", "Cities.json");
            var dataText = File.ReadAllText(path);
            var cities = Seeder<List<City>>.SeedIt(dataText);
            return cities ?? [];
        }

        public IEnumerable<ApartmentType> SeedApartmentTypesAsync()
        {
            var path = Path.Combine(contentRootPath, "Seed", "ApartmentTypes.json");
            var dataText = File.ReadAllText(path);
            var apartmentTypes = Seeder<List<ApartmentType>>.SeedIt(dataText);
            return apartmentTypes ?? [];
        }

        public IEnumerable<RoomType> SeedRoomTypesAsync()
        {
            var path = Path.Combine(contentRootPath, "Seed", "RoomTypes.json");
            var dataText = File.ReadAllText(path);
            var roomTypes = Seeder<List<RoomType>>.SeedIt(dataText);
            return roomTypes ?? [];
        }

        public IEnumerable<RequestType> SeedRequestTypesAsync()
        {
            var path = Path.Combine(contentRootPath, "Seed", "RequestTypes.json");
            var dataText = File.ReadAllText(path);
            var requestTypes = Seeder<List<RequestType>>.SeedIt(dataText);
            return requestTypes ?? [];
        }

        public IEnumerable<Relationship> SeedRelationshipsAsync()
        {
            var path = Path.Combine(contentRootPath, "Seed", "Relationships.json");
            var dataText = File.ReadAllText(path);
            var relationships = Seeder<List<Relationship>>.SeedIt(dataText);
            return relationships ?? [];
        }

        public IEnumerable<Role> SeedRolesAsync()
        {
            var path = Path.Combine(contentRootPath, "Seed", "Roles.json");
            var dataText = File.ReadAllText(path);
            var roles = Seeder<List<Role>>.SeedIt(dataText);
            return roles ?? [];
        }

        //public IEnumerable<Status> SeedStatusesAsync()
        //{
        //    var dataText = System.IO.File.ReadAllText(@"Seed/Statuses.json");
        //    var statuses = Seeder<List<Status>>.SeedIt(dataText);
        //    return statuses;
        //}
    }
}