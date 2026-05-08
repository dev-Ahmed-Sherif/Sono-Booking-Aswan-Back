using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using SonoBooking.Common.Core;
using SonoBooking.Common.DTO.Base;

namespace SonoBooking.Application.Services.Base
{
    public interface IBaseService<T, TAddDto, TEditDto, TGetDto, TKey , TKeyDto>
        where T : class
        where TAddDto : IEntityDto<TKeyDto>
        where TEditDto : IEntityDto<TKeyDto>
        where TGetDto : IEntityDto<TKeyDto>
    {
        Task<IFinalResult> GetByIdAsync(object id, CancellationToken cancellationToken = default);
        Task<IFinalResult> GetByIdForEditAsync(object id, CancellationToken cancellationToken = default);
        Task<IFinalResult> GetAllAsync(bool disableTracking = false, Expression<Func<T, bool>> predicate = null, CancellationToken cancellationToken = default);
        Task<IFinalResult> AddAsync(TAddDto model, CancellationToken cancellationToken = default);
        Task<IFinalResult> AddListAsync(List<TAddDto> model, CancellationToken cancellationToken = default);
        Task<IFinalResult> UpdateAsync(TAddDto model, CancellationToken cancellationToken = default);
        Task<IFinalResult> DeleteAsync(object id, CancellationToken cancellationToken = default);
        Task<IFinalResult> DeleteSoftAsync(object id, CancellationToken cancellationToken = default);

        Task<IFinalResult> GetLastRecordAsync(CancellationToken cancellationToken = default);

    }
}
