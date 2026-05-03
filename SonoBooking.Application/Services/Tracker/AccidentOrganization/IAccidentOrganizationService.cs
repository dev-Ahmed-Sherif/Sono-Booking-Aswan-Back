using SonoTracker.Application.Services.Base;
using SonoTracker.Common.Core;
using SonoTracker.Common.DTO.Base;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SonoTracker.Common.DTO.Tracker.AccidentOrganization;
using SonoBooking.Common.DTO.Tracker.AccidentOrganization.Parameters;

namespace SonoTracker.Application.Services.Tracker.AccidentOrganization
{
    public interface IAccidentOrganizationService : IBaseService<Entities.Tracker.AccidentOrganization, AddAccidentOrganizationDto, EditAccidentOrganizationDto, AccidentOrganizationDto, string, string>
    {
        Task<IFinalResult> GetAllAsync(string accidentId, CancellationToken cancellationToken = default);

        Task<PagingResult> GetAllPagedAsync(BaseParam<AccidentOrganizationFilter> filter, CancellationToken cancellationToken = default);

        Task<IFinalResult> DeleteRangeAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default);

    }
}
