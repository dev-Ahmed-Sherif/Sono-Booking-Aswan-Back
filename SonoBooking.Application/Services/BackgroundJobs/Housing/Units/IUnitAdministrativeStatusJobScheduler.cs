using System;

namespace SonoBooking.Application.Services.BackgroundJobs.Housing.Units
{
    public interface IUnitAdministrativeStatusJobScheduler
    {
        string? SyncAdministrativeStatusJob(
            bool administrativeStatus,
            string? existingJobId,
            DateOnly? endAdministrativeDate,
            string unitId,
            HousingUnitType unitType);
    }
}
