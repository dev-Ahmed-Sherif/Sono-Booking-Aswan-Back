using SonoBooking.Application.Services.Base;
using SonoBooking.Common.Constants.Auth;
using SonoBooking.Common.Core;
using SonoBooking.Common.DTO.Base;
using SonoBooking.Common.DTO.Housing.UnitImage;
using SonoBooking.Common.DTO.Housing.UnitImage.Parameters;
using SonoBooking.Domain;
using SonoBooking.Domain.Entities.Housing;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SonoBooking.Application.Services.Housing.UnitImages
{
    public class UnitImageService(IServiceBaseParameter<UnitImage> businessBaseParameter) : BaseService<UnitImage, AddUnitImageDto, EditUnitImageDto, UnitImageDto, string, string>(businessBaseParameter), IUnitImageService
    {
        public async Task<PagingResult> GetAllPagedAsync(BaseParam<UnitImageFilter> filter, CancellationToken cancellationToken = default)
        {
            int pageNumber = filter.PageNumber;
            int limit = filter.PageSize;
            int offset = (pageNumber - 1) * limit;
            UnitImageFilter unitImageFilter = filter?.Filter ?? new UnitImageFilter();

            (int Count, IEnumerable<UnitImage> Result) = await UnitOfWork.Repository.FindPagedAsync(
                predicate: x => x.IsDeleted == unitImageFilter.IsDeleted,
                pageNumber: offset,
                pageSize: limit,
                filter.OrderByValue,
                cancellationToken: cancellationToken);

            IEnumerable<UnitImageDto> data = Mapper.Map<IEnumerable<UnitImage>, IEnumerable<UnitImageDto>>(Result ?? []);
            return new PagingResult(pageNumber, limit, Count, data, status: HttpStatusCode.OK, MessagesConstants.Success);
        }
    }
}
