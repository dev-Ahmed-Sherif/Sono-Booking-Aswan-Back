using System.Collections.Generic;
using System.IO;
using SonoTracker.Common.Extensions;
using AccidentType = SonoTracker.Domain.Entities.Lookups.AccidentType;
using City = SonoTracker.Domain.Entities.Lookups.City;
using Governorate = SonoTracker.Domain.Entities.Lookups.Governorate;
using Nationality = SonoTracker.Domain.Entities.Lookups.Nationality;

namespace SonoTracker.Infrastructure.DataInitializer
{
    public class DataInitializer(string contentRootPath) : IDataInitializer
    {
        public IEnumerable<Nationality> SeedNationalitiesAsync()
        {
            var path = Path.Combine(contentRootPath, "Seed", "Nationalities.json");
            var dataText = File.ReadAllText(path);
            var nationalities = Seeder<List<Nationality>>.SeedIt(dataText);
            return nationalities ?? [];
        }

        public IEnumerable<AccidentType> SeedAccidentTypesAsync()
        {
            var path = Path.Combine(contentRootPath, "Seed", "AccidentTypes.json");
            var dataText = File.ReadAllText(path);
            var accidentTypes = Seeder<List<AccidentType>>.SeedIt(dataText);
            return accidentTypes ?? [];
        }

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