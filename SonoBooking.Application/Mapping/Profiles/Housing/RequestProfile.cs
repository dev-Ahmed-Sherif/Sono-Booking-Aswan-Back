using SonoBooking.Common.DTO.Housing.Request;
using SonoBooking.Domain.Entities.Housing;

namespace SonoBooking.Application.Mapping
{
    public partial class MappingService
    {
        public void MapRequest()
        {
            CreateMap<Request, RequestDto>()
                .ForMember(dest => dest.RequestType, opt => opt.MapFrom(src => src.RequestType.NameAr))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.FullName))
                .ForMember(dest => dest.RequestAttaches, opt => opt.MapFrom(src => src.RequestAttaches))
                .ReverseMap();

            CreateMap<Request, EditRequestDto>().ReverseMap();

            CreateMap<AddRequestDto, Request>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.RequestNumber, opt => opt.Ignore())
                .ForMember(dest => dest.RequestDate, opt => opt.Ignore())
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.StartDate.AddDays(src.Nights)))
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.RequestUnits, opt => opt.Ignore())
                .ForMember(dest => dest.RequestParticipants, opt => opt.Ignore())
                .ForMember(dest => dest.RequestAttaches, opt => opt.Ignore())
                .ForMember(dest => dest.Reservation, opt => opt.Ignore())
                .ForMember(dest => dest.Approval, opt => opt.Ignore())
                .ForMember(dest => dest.RequestType, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.ApprovedBy, opt => opt.Ignore())
                .ForMember(dest => dest.PreviousRequest, opt => opt.Ignore());
        }
    }
}
