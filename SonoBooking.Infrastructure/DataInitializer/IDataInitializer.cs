using City = SonoTracker.Domain.Entities.Lookups.City;
using System.Collections.Generic;
using BookingType = SonoBooking.Domain.Entities.Lookups.BookingType;
using Governorate = SonoBooking.Domain.Entities.Lookups.Governorate;
using Nationality = SonoTracker.Domain.Entities.Lookups.Nationality;

namespace SonoTracker.Infrastructure.DataInitializer
{
    public interface IDataInitializer
    {
        IEnumerable<Nationality> SeedNationalitiesAsync();

        IEnumerable<BookingType> SeedAccidentTypesAsync();

        IEnumerable<Governorate> SeedGovernoratesAsync();

        IEnumerable<City> SeedCitiesAsync();

        //IEnumerable<Status> SeedStatusesAsync();
    }
}