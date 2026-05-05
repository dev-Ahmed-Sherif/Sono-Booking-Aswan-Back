using City = SonoBooking.Domain.Entities.Lookups.City;
using System.Collections.Generic;
using Governorate = SonoBooking.Domain.Entities.Lookups.Governorate;

namespace SonoBooking.Infrastructure.DataInitializer
{
    public interface IDataInitializer
    {
        IEnumerable<Governorate> SeedGovernoratesAsync();

        IEnumerable<City> SeedCitiesAsync();

        //IEnumerable<Status> SeedStatusesAsync();
    }
}