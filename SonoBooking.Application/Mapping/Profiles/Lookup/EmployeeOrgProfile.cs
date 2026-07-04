using SonoBooking.Common.DTO.Lookup.EmployeeOrg;
using SonoBooking.Domain.Entities.Lookups;

namespace SonoBooking.Application.Mapping
{
    public partial class MappingService
    {
        public void MapEmployeeOrg()
        {
            CreateMap<EmployeeOrg, EmployeeOrgDto>().ReverseMap();

            CreateMap<EmployeeOrg, EditEmployeeOrgDto>().ReverseMap();

            CreateMap<AddEmployeeOrgDto, EmployeeOrg>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ReverseMap();
        }
    }
}
