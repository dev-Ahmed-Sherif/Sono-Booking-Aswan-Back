using SonoTracker.Application.Services.Base;
using SonoTracker.Common.Core;
using SonoTracker.Common.DTO.Base;
using SonoTracker.Common.DTO.Tracker.AccidentAttach;
using SonoTracker.Common.DTO.Tracker.AccidentAttach.Parameters;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SonoTracker.Application.Services.Tracker.AccidentAttach
{
    public interface IAccidentAttachService : IBaseService<Entities.Tracker.AccidentAttachment, AddAccidentAttachDto, EditAccidentAttachDto, AccidentAttachDto, string, string>
    {
        Task<PagingResult> GetAllPagedAsync(BaseParam<AccidentAttachFilter> filter, CancellationToken cancellationToken = default);

        Task<IFinalResult> DeleteRangeWithAttachIdRangeAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default);

        Task<IFinalResult> GetAllFilterAsync(AccidentAttachFilter filter, CancellationToken cancellationToken = default);

        Task<PagingResult> GetDropDownAsync(BaseParam<SearchCriteriaFilter> filter, CancellationToken cancellationToken = default);

        //Task<byte[]> GenerateReportAsync(FilterDirectionReportDto filter, CancellationToken cancellationToken = default);
    }
}
