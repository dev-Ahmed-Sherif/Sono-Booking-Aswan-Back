using SonoBooking.Application.Services.Base;
using SonoBooking.Common.Constants.Auth;
using SonoBooking.Common.Core;
using SonoBooking.Common.DTO.Base;
using SonoBooking.Common.DTO.Housing.Leader;
using SonoBooking.Common.DTO.Housing.Leader.Parameters;
using SonoBooking.Domain;
using SonoBooking.Domain.Entities.Housing;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SonoBooking.Application.Services.Housing.Leaders
{
    public class LeaderService(IServiceBaseParameter<Leader> businessBaseParameter) : BaseService<Leader, AddLeaderDto, EditLeaderDto, LeaderDto, string, string>(businessBaseParameter), ILeaderService
    {
        public async Task<PagingResult> GetAllPagedAsync(BaseParam<LeaderFilter> filter, CancellationToken cancellationToken = default)
        {
            int pageNumber = filter.PageNumber;
            int limit = filter.PageSize;
            int offset = (pageNumber - 1) * limit;
            LeaderFilter leaderFilter = filter?.Filter ?? new LeaderFilter();

            (int Count, IEnumerable<Leader> Result) = await UnitOfWork.Repository.FindPagedAsync(
                predicate: x => x.IsDeleted == leaderFilter.IsDeleted,
                pageNumber: offset,
                pageSize: limit,
                filter.OrderByValue,
                cancellationToken: cancellationToken);

            IEnumerable<LeaderDto> data = Mapper.Map<IEnumerable<Leader>, IEnumerable<LeaderDto>>(Result ?? []);
            return new PagingResult(pageNumber, limit, Count, data, status: HttpStatusCode.OK, MessagesConstants.Success);
        }
    }
}
