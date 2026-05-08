using SonoBooking.Common.DTO.Housing.Bed;
using SonoBooking.Domain.Entities.Housing;

namespace SonoBooking.Application.Mapping
{
    public partial class MappingService
    {
        public void MapBed()
        {
            CreateMap<Bed, BedDto>().ReverseMap();

            CreateMap<Bed, EditBedDto>().ReverseMap();

            CreateMap<AddBedDto, Bed>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ReverseMap();
        }
    }
}
