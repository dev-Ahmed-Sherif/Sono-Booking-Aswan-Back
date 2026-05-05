using System.Collections.Generic;
using System.IO;
using SonoTracker.Common.Extensions;
using City = SonoBooking.Domain.Entities.Lookups.City;
using Governorate = SonoBooking.Domain.Entities.Lookups.Governorate;

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

        //public IEnumerable<Status> SeedStatusesAsync()
        //{
        //    var dataText = System.IO.File.ReadAllText(@"Seed/Statuses.json");
        //    var statuses = Seeder<List<Status>>.SeedIt(dataText);
        //    return statuses;
        //}
    }
}