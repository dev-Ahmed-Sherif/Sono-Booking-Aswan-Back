using SonoTracker.Application.Services.Base;
using SonoTracker.Common.Core;
using SonoTracker.Common.DTO.Base;
using SonoTracker.Common.DTO.Reports.TouristMarina;
using SonoTracker.Common.DTO.Tracker.TouristMarina;
using SonoTracker.Common.DTO.Tracker.TouristMarina.Parameters;
using SonoTracker.Common.DTO.Tracker.TouristMarinaOrganization;
using SonoTracker.Common.DTO.Tracker.TouristMarinaOrganization.Parameters;
using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SonoTracker.Application.Services.Tracker.TouristMarinaOrganization
{
   public  interface ITouristMarinaOrganizationService : IBaseService<Entities.Tracker.TouristMarinaOrganization, AddTouristMarinaOrganizationDto, EditTouristMarinaOrganizationDto, TouristMarinaOrganizationDto, string, string>
    {
        Task<PagingResult> GetAllPagedAsync(BaseParam<TouristMarinaOrganizationFilter> Filter, CancellationToken cancellationToken = default);
        Task<IFinalResult> DeleteRangeAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default);
        Task<PagingResult> GetDropDownAsync(BaseParam<SearchCriteriaFilter> filter, CancellationToken cancellationToken = default);

        Task<byte[]> GenerateReportAsync(FilterTouristMarinaReportDto filter, CancellationToken cancellationToken = default);
    }
}
