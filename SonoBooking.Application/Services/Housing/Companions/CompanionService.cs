using SonoBooking.Application.Services.Base;
using SonoBooking.Common.Constants.Auth;
using SonoBooking.Common.Core;
using SonoBooking.Common.DTO.Base;
using SonoBooking.Common.DTO.Housing.Companion;
using SonoBooking.Common.DTO.Housing.Companion.Parameters;
using SonoBooking.Domain;
using SonoBooking.Domain.Entities.Housing;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SonoBooking.Application.Services.Housing.Companions
{
    public class CompanionService(IServiceBaseParameter<Companion> businessBaseParameter) : BaseService<Companion, AddCompanionDto, EditCompanionDto, CompanionDto, string, string>(businessBaseParameter), ICompanionService
    {
        public async Task<PagingResult> GetAllPagedAsync(BaseParam<CompanionFilter> filter, CancellationToken cancellationToken = default)
        {
            int pageNumber = filter.PageNumber;
            int limit = filter.PageSize;
            int offset = (pageNumber - 1) * limit;
            CompanionFilter companionFilter = filter?.Filter ?? new CompanionFilter();

            (int Count, IEnumerable<Companion> Result) = await UnitOfWork.Repository.FindPagedAsync(
                predicate: x => x.IsDeleted == companionFilter.IsDeleted,
                pageNumber: offset,
                pageSize: limit,
                filter.OrderByValue,
                cancellationToken: cancellationToken);

            IEnumerable<CompanionDto> data = Mapper.Map<IEnumerable<Companion>, IEnumerable<CompanionDto>>(Result ?? []);
            return new PagingResult(pageNumber, limit, Count, data, status: HttpStatusCode.OK, MessagesConstants.Success);
        }
    }
}
