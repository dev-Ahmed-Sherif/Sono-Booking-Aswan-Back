using SonoBooking.Common.DTO.Lookup.Employee;
using SonoBooking.Domain.Entities.Lookups;

namespace SonoBooking.Application.Mapping
{
    public partial class MappingService
    {
        public void MapEmployee()
        {
            CreateMap<Employee, EmployeeDto>()
                .ForMember(dest => dest.EmployeeOrg, cfg => cfg.MapFrom(src => src.EmployeeOrg.NameAr))
                .ForMember(dest => dest.EmployeeJob, cfg => cfg.MapFrom(src => src.EmployeeJob.NameAr))
                .ReverseMap();

            CreateMap<Employee, EditEmployeeDto>().ReverseMap();

            CreateMap<AddEmployeeDto, Employee>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ReverseMap();
        }
    }
}
