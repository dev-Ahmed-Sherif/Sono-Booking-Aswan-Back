using SonoBooking.Application.Services.Base;
using SonoBooking.Common.Constants.Auth;
using SonoBooking.Common.Core;
using SonoBooking.Common.DTO.Base;
using SonoBooking.Common.DTO.Housing.Apartment;
using SonoBooking.Common.DTO.Housing.Apartment.Parameters;
using SonoBooking.Domain;
using SonoBooking.Domain.Entities.Housing;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SonoBooking.Application.Services.Housing.Apartments
{
    public class ApartmentService(IServiceBaseParameter<Apartment> businessBaseParameter) : BaseService<Apartment, AddApartmentDto, EditApartmentDto, ApartmentDto, string, string>(businessBaseParameter), IApartmentService
    {
        public async Task<PagingResult> GetAllPagedAsync(BaseParam<ApartmentFilter> filter, CancellationToken cancellationToken = default)
        {
            int pageNumber = filter.PageNumber;
            int limit = filter.PageSize;
            int offset = (pageNumber - 1) * limit;
            ApartmentFilter apartmentFilter = filter?.Filter ?? new ApartmentFilter();

            (int Count, IEnumerable<Apartment> Result) = await UnitOfWork.Repository.FindPagedAsync(
                predicate: x => x.IsDeleted == apartmentFilter.IsDeleted,
                pageNumber: offset,
                pageSize: limit,
                filter.OrderByValue,
                cancellationToken: cancellationToken);

            IEnumerable<ApartmentDto> data = Mapper.Map<IEnumerable<Apartment>, IEnumerable<ApartmentDto>>(Result ?? []);
            return new PagingResult(pageNumber, limit, Count, data, status: HttpStatusCode.OK, MessagesConstants.Success);
        }
    }
}
