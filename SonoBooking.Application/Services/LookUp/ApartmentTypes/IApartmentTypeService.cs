using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SonoBooking.Domain.Entities.Lookups;
using SonoBooking.Application.Services.Base;
using SonoBooking.Common.Core;
using SonoBooking.Common.DTO.Base;
using SonoBooking.Common.DTO.Lookup.ApartmentType;
using SonoBooking.Common.DTO.Lookup.ApartmentType.Parameters;

namespace SonoBooking.Application.Services.LookUp.ApartmentTypes
{
    public interface IApartmentTypeService : IBaseService<ApartmentType, AddApartmentTypeDto, EditApartmentTypeDto, ApartmentTypeDto, string, string>
    {
        Task<PagingResult> GetAllPagedAsync(BaseParam<ApartmentTypeFilter> filter, CancellationToken cancellationToken = default);

        Task<PagingResult> GetDropDownAsync(BaseParam<SearchCriteriaFilter> filter, CancellationToken cancellationToken = default);

    }
}

