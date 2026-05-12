using SonoBooking.Application.Services.Base;
using SonoBooking.Common.Core;
using SonoBooking.Common.DTO.Base;
using SonoBooking.Common.DTO.Housing.Apartment;
using SonoBooking.Common.DTO.Housing.Apartment.Parameters;
using SonoBooking.Domain.Entities.Housing;
using System.Threading;
using System.Threading.Tasks;

namespace SonoBooking.Application.Services.Housing.Apartments
{
    public interface IApartmentService : IBaseService<Apartment, AddApartmentDto, EditApartmentDto, ApartmentDto, string, string>
    {
        Task<PagingResult> GetAllPagedAsync(BaseParam<ApartmentFilter> filter, CancellationToken cancellationToken = default);
        Task<IFinalResult> DeleteAsync(object id, CancellationToken cancellationToken = default);
    }
}
