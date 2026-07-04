using SonoBooking.Common.DTO.Identity.User;
using SonoBooking.Domain.Entities.Identity;

namespace SonoBooking.Application.Mapping
{
    public partial class MappingService
    {
        public void MapUser()
        {
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.Role, opt => opt.Ignore())
                .ForMember(dest => dest.RoleId, opt => opt.Ignore())
                .ForMember(dest => dest.LeaderName, opt => opt.Ignore());
        }
    }
}
