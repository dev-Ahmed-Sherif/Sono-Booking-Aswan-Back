using SonoBooking.Application.Services.Base;
using SonoBooking.Common.Constants.Auth;
using SonoBooking.Common.Core;
using SonoBooking.Common.DTO.Base;
using SonoBooking.Common.DTO.Housing.Bed;
using SonoBooking.Common.DTO.Housing.Bed.Parameters;
using SonoBooking.Domain;
using SonoBooking.Domain.Entities.Housing;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SonoBooking.Application.Services.Housing.Beds
{
    public class BedService(IServiceBaseParameter<Bed> businessBaseParameter) : BaseService<Bed, AddBedDto, EditBedDto, BedDto, string, string>(businessBaseParameter), IBedService
    {
        public async Task<PagingResult> GetAllPagedAsync(BaseParam<BedFilter> filter, CancellationToken cancellationToken = default)
        {
            int pageNumber = filter.PageNumber;
            int limit = filter.PageSize;
            int offset = (pageNumber - 1) * limit;
            BedFilter bedFilter = filter?.Filter ?? new BedFilter();

            (int Count, IEnumerable<Bed> Result) = await UnitOfWork.Repository.FindPagedAsync(
                predicate: x => x.IsDeleted == bedFilter.IsDeleted,
                pageNumber: offset,
                pageSize: limit,
                filter.OrderByValue,
                cancellationToken: cancellationToken);

            IEnumerable<BedDto> data = Mapper.Map<IEnumerable<Bed>, IEnumerable<BedDto>>(Result ?? []);
            return new PagingResult(pageNumber, limit, Count, data, status: HttpStatusCode.OK, MessagesConstants.Success);
        }
    }
}
