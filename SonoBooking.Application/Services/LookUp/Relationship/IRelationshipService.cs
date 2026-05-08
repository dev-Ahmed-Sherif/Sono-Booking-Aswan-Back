using SonoBooking.Application.Services.Base;
using SonoBooking.Common.DTO.Lookup.Relationship;
using SonoBooking.Common.DTO.Lookup.Relationship.Parameters;
using SonoBooking.Domain.Entities.Lookups;
using SonoBooking.Common.Core;
using SonoBooking.Common.DTO.Base;
using System.Threading;
using System.Threading.Tasks;

namespace SonoBooking.Application.Services.LookUp.Relationship
{
    public interface IRelationshipService : IBaseService<Entities.Lookups.Relationship, AddRelationshipDto, EditRelationshipDto, RelationshipDto, string, string>
    {
        Task<PagingResult> GetAllPagedAsync(BaseParam<RelationshipFilter> filter, CancellationToken cancellationToken = default);

        Task<PagingResult> GetDropDownAsync(BaseParam<SearchCriteriaFilter> filter, CancellationToken cancellationToken = default);
    }
}
