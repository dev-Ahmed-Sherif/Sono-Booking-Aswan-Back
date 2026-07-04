using SonoBooking.Common.DTO.Lookup.EmployeeJob;
using SonoBooking.Domain.Entities.Lookups;

namespace SonoBooking.Application.Mapping
{
    public partial class MappingService
    {
        public void MapEmployeeJob()
        {
            CreateMap<EmployeeJob, EmployeeJobDto>().ReverseMap();

            CreateMap<EmployeeJob, EditEmployeeJobDto>().ReverseMap();

            CreateMap<AddEmployeeJobDto, EmployeeJob>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ReverseMap();
        }
    }
}
