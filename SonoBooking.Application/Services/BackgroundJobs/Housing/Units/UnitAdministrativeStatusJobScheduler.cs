using Hangfire;
using System;

namespace SonoBooking.Application.Services.BackgroundJobs.Housing.Units
{
    public class UnitAdministrativeStatusJobScheduler : IUnitAdministrativeStatusJobScheduler
    {
        public string? SyncAdministrativeStatusJob(
            bool administrativeStatus,
            string? existingJobId,
            DateOnly? endAdministrativeDate,
            string unitId,
            HousingUnitType unitType)
        {
            CancelJob(existingJobId);

            if (!administrativeStatus || !endAdministrativeDate.HasValue)
                return null;

            DateTime runAt = endAdministrativeDate.Value.AddDays(1).ToDateTime(TimeOnly.MinValue);
            if (runAt <= DateTime.Now)
                runAt = DateTime.Now.AddSeconds(5);

            return unitType switch
            {
                HousingUnitType.Apartment => BackgroundJob.Schedule<UnitAdministrativeStatusJob>(
                    job => job.ClearApartmentAdministrativeStatusAsync(unitId),
                    runAt),
                HousingUnitType.Room => BackgroundJob.Schedule<UnitAdministrativeStatusJob>(
                    job => job.ClearRoomAdministrativeStatusAsync(unitId),
                    runAt),
                HousingUnitType.Bed => BackgroundJob.Schedule<UnitAdministrativeStatusJob>(
                    job => job.ClearBedAdministrativeStatusAsync(unitId),
                    runAt),
                _ => null
            };
        }

        private static void CancelJob(string? jobId)
        {
            if (string.IsNullOrWhiteSpace(jobId))
                return;

            try
            {
                BackgroundJob.Delete(jobId);
            }
            catch
            {
                // Job may already have run or been removed.
            }
        }
    }
}
