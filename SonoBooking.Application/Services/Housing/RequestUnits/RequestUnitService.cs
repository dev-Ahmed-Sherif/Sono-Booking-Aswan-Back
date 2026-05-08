using SonoBooking.Application.Services.Base;
using SonoBooking.Common.Constants.Auth;
using SonoBooking.Common.Core;
using SonoBooking.Common.DTO.Base;
using SonoBooking.Common.DTO.Housing.RequestUnit;
using SonoBooking.Common.DTO.Housing.RequestUnit.Parameters;
using SonoBooking.Domain;
using SonoBooking.Domain.Entities.Housing;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SonoBooking.Application.Services.Housing.RequestUnits
{
    public class RequestUnitService(IServiceBaseParameter<RequestUnit> businessBaseParameter) : BaseService<RequestUnit, AddRequestUnitDto, EditRequestUnitDto, RequestUnitDto, string, string>(businessBaseParameter), IRequestUnitService
    {
        public async Task<PagingResult> GetAllPagedAsync(BaseParam<RequestUnitFilter> filter, CancellationToken cancellationToken = default)
        {
            int pageNumber = filter.PageNumber;
            int limit = filter.PageSize;
            int offset = (pageNumber - 1) * limit;
            RequestUnitFilter requestUnitFilter = filter?.Filter ?? new RequestUnitFilter();

            (int Count, IEnumerable<RequestUnit> Result) = await UnitOfWork.Repository.FindPagedAsync(
                predicate: x => x.IsDeleted == requestUnitFilter.IsDeleted,
                pageNumber: offset,
                pageSize: limit,
                filter.OrderByValue,
                cancellationToken: cancellationToken);

            IEnumerable<RequestUnitDto> data = Mapper.Map<IEnumerable<RequestUnit>, IEnumerable<RequestUnitDto>>(Result ?? []);
            return new PagingResult(pageNumber, limit, Count, data, status: HttpStatusCode.OK, MessagesConstants.Success);
        }
    }
}
