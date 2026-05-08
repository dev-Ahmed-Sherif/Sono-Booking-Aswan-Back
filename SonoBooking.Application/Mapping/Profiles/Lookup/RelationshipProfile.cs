using SonoBooking.Common.DTO.Lookup.Relationship;
using SonoBooking.Domain.Entities.Lookups;

namespace SonoBooking.Application.Mapping
{
    public partial class MappingService
    {
        public void MapRelationship()
        {
            CreateMap<Relationship, RelationshipDto>().ReverseMap();

            CreateMap<Relationship, EditRelationshipDto>().ReverseMap();

            CreateMap<AddRelationshipDto, Relationship>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ReverseMap();
        }
    }
}
