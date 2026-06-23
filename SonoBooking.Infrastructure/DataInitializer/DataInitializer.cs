using System.Collections.Generic;
using System.IO;
using SonoBooking.Common.Extensions;
using SonoBooking.Domain.Entities.BusinessNotification;
using SonoBooking.Domain.Entities.Lookups;
using AllowedDayBeforeReservation = SonoBooking.Domain.Entities.Lookups.AllowedDayBeforeReservation;
using ApartmentType = SonoBooking.Domain.Entities.Lookups.ApartmentType;
using City = SonoBooking.Domain.Entities.Lookups.City;
using Employee = SonoBooking.Domain.Entities.Lookups.Employee;
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

        public IEnumerable<AllowedDayBeforeReservation> SeedAllowedDayBeforeReservationsAsync()
        {
            var path = Path.Combine(contentRootPath, "Seed", "AllowedDayBeforeReservations.json");
            if (!File.Exists(path))
                return [];

            var dataText = File.ReadAllText(path);
            var allowedDays = Seeder<List<AllowedDayBeforeReservation>>.SeedIt(dataText);
            return allowedDays ?? [];
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

        public IEnumerable<Employee> SeedEmployeesAsync()
        {
            var path = Path.Combine(contentRootPath, "Seed", "Employees.json");
            if (!File.Exists(path))
                return [];

            var dataText = File.ReadAllText(path);
            var employees = Seeder<List<Employee>>.SeedIt(dataText);
            return employees ?? [];
        }

        public IEnumerable<NotificationGroup> SeedNotificationGroupsAsync()
        {
            var path = Path.Combine(contentRootPath, "Seed", "NotificationGroups.json");
            if (!File.Exists(path))
                return [];

            var dataText = File.ReadAllText(path);
            var groups = Seeder<List<NotificationGroup>>.SeedIt(dataText);
            return groups ?? [];
        }

        //public IEnumerable<Status> SeedStatusesAsync()
        //{
        //    var dataText = System.IO.File.ReadAllText(@"Seed/Statuses.json");
        //    var statuses = Seeder<List<Status>>.SeedIt(dataText);
        //    return statuses;
        //}
    }
}
