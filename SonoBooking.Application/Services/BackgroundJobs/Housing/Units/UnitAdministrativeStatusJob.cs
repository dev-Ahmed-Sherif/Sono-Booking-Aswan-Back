using SonoBooking.Common.Infrastructure.UnitOfWork;
using SonoBooking.Domain.Entities.Base;
using SonoBooking.Domain.Entities.Housing;
using System;
using System.Threading.Tasks;

namespace SonoBooking.Application.Services.BackgroundJobs.Housing.Units
{
    public class UnitAdministrativeStatusJob(
        IUnitOfWork<Apartment> apartmentUnitOfWork,
        IUnitOfWork<Room> roomUnitOfWork,
        IUnitOfWork<Bed> bedUnitOfWork)
    {
        private const string SystemActor = "System";

        public async Task ClearApartmentAdministrativeStatusAsync(string apartmentId)
        {
            Apartment apartment = await apartmentUnitOfWork.Repository.FirstOrDefaultAsync(
                x => x.Id == apartmentId,
                disableTracking: false);

            if (apartment == null || !apartment.AdministrativeStatus)
                return;

            apartment.AdministrativeStatus = false;
            apartment.AdministrativeStatusJobId = null;
            TouchAudit(apartment);

            await apartmentUnitOfWork.SaveChangesAsync();
        }

        public async Task ClearRoomAdministrativeStatusAsync(string roomId)
        {
            Room room = await roomUnitOfWork.Repository.FirstOrDefaultAsync(
                x => x.Id == roomId,
                disableTracking: false);

            if (room == null || !room.AdministrativeStatus)
                return;

            room.AdministrativeStatus = false;
            room.AdministrativeStatusJobId = null;
            TouchAudit(room);

            await roomUnitOfWork.SaveChangesAsync();
        }

        public async Task ClearBedAdministrativeStatusAsync(string bedId)
        {
            Bed bed = await bedUnitOfWork.Repository.FirstOrDefaultAsync(
                x => x.Id == bedId,
                disableTracking: false);

            if (bed == null || !bed.AdministrativeStatus)
                return;

            bed.AdministrativeStatus = false;
            bed.AdministrativeStatusJobId = null;
            TouchAudit(bed);

            await bedUnitOfWork.SaveChangesAsync();
        }

        private static void TouchAudit(BaseAudit<string> entity)
        {
            entity.ModifiedAt = DateTime.UtcNow;
            entity.ModifiedBy = SystemActor;
            entity.ModifiedById = SystemActor;
        }
    }
}
