using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SonoTracker.Application.Services.Base;
using SonoTracker.Common.Core;
using SonoTracker.Common.DTO.Base;
using SonoTracker.Common.DTO.Tracker.Governorate.Parameters;
using SonoTracker.Common.DTO.Tracker.Governorate;
using SonoTracker.Common.DTO.Tracker.TouristMarinaLicenseApplication.Parameters;
using SonoTracker.Common.DTO.Tracker.TouristMarinaLicenseApplication;

namespace SonoTracker.Application.Services.Tracker.TouristMarinaLicenseApplication
{
    public interface ITouristMarinaLicenseApplicationService : IBaseService<Entities.Tracker.TouristMarinaLicenseApplication, AddTouristMarinaLicenseApplicationDto, EditTouristMarinaLicenseApplicationDto, TouristMarinaLicenseApplicationDto, string, string>
    {
        Task<PagingResult> GetAllPagedAsync(BaseParam<TouristMarinaLicenseApplicationFilter> filter, CancellationToken cancellationToken = default);
        Task<IFinalResult> GetAllFilterAsync(TouristMarinaLicenseApplicationFilter filter, CancellationToken cancellationToken = default);
        Task<IFinalResult> DeleteRangeAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default);
        Task<PagingResult> GetDropDownAsync(BaseParam<SearchCriteriaFilter> filter, CancellationToken cancellationToken = default);
    }
}
