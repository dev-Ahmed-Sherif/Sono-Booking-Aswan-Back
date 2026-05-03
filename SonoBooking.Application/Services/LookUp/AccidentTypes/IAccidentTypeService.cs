using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SonoBooking.Domain.Entities.Lookups;
using SonoTracker.Application.Services.Base;
using SonoTracker.Common.Core;
using SonoTracker.Common.DTO.Base;
using SonoTracker.Common.DTO.Lookup.AccidentType;
using SonoTracker.Common.DTO.Lookup.AccidentType.Parameters;

namespace SonoTracker.Application.Services.LookUp.AccidentTypes
{
    public interface IAccidentTypeService : IBaseService<BookingType, AddAccidentTypeDto, EditAccidentTypeDto, AccidentTypeDto,string, string>
    {
        Task<PagingResult> GetAllPagedAsync(BaseParam<AccidentTypeFilter> filter, CancellationToken cancellationToken = default);

        Task<PagingResult> GetDropDownAsync(BaseParam<SearchCriteriaFilter> filter, CancellationToken cancellationToken = default);

    }
}
