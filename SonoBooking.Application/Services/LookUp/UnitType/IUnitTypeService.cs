using SonoTracker.Application.Services.Base;
using SonoTracker.Common.Core;
using SonoTracker.Common.DTO.Base;
using SonoTracker.Common.DTO.Lookup.UnitType;
using SonoTracker.Common.DTO.Lookup.UnitType.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SonoTracker.Application.Services.LookUp.UnitType
{
    public interface IUnitTypeService : IBaseService<SonoBooking.Domain.Entities.Lookups.UnitType, AddUnitTypeDto, EditUnitTypeDto, UnitTypeDto,string, string>
    {
        Task<PagingResult> GetAllPagedAsync(BaseParam<UnitTypeFilter> Filter, CancellationToken cancellationToken = default);
        Task<PagingResult> GetDropDownAsync(BaseParam<SearchCriteriaFilter> filter, CancellationToken cancellationToken = default);
    }
}
