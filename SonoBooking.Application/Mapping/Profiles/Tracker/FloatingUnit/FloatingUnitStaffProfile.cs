using SonoTracker.Common.DTO.Tracker.FloatingUnitStaff;
using SonoTracker.Domain.Entities.Tracker;

namespace SonoTracker.Application.Mapping
{
    public partial class MappingService
    {
        public void MapFloatingUnitStaff()
        {
            CreateMap<FloatingUnitStaff, FloatingUnitStaffDto>()
            //.ForMember(des => des.GenderName, opt => opt.MapFrom(src => src.Gender.GetName()))
            //.ForMember(des => des.IDTypeName, opt => opt.MapFrom(src => src.IDType.GetName()))
            //.ForMember(des => des.NationalityName, opt => opt.MapFrom(src => src.Nationality.NameAr))
            //.ForMember(des => des.FloatingUnitNameAr, opt => opt.MapFrom(src => src.FloatingUnit.NameAr))
            //.ForMember(des => des.FloatingUnitNameEn, opt => opt.MapFrom(src => src.FloatingUnit.NameEn))
            .ReverseMap();
            CreateMap<FloatingUnitStaff, EditFloatingUnitStaffDto>().ReverseMap();
            CreateMap<AddFloatingUnitStaffDto, FloatingUnitStaff>()
                .ForMember(des => des.Id, opt => opt.Ignore())
                .ReverseMap();
        }
    }
}
