using SonoTracker.Domain.Entities.Lookups;
using City = SonoTracker.Domain.Entities.Lookups.City;
using System.Collections.Generic;
using AccidentType = SonoTracker.Domain.Entities.Lookups.AccidentType;
using Governorate = SonoTracker.Domain.Entities.Lookups.Governorate;
using Nationality = SonoTracker.Domain.Entities.Lookups.Nationality;

namespace SonoTracker.Infrastructure.DataInitializer
{
    public interface IDataInitializer
    {
        IEnumerable<Nationality> SeedNationalitiesAsync();

        IEnumerable<AccidentType> SeedAccidentTypesAsync();

        IEnumerable<Governorate> SeedGovernoratesAsync();

        IEnumerable<City> SeedCitiesAsync();

        //IEnumerable<Status> SeedStatusesAsync();
    }
}