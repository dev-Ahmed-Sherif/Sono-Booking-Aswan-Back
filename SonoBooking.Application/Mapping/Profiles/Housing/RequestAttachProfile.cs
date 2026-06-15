using SonoBooking.Common.DTO.Housing.RequestAttach;
using SonoBooking.Domain.Entities.Housing;

namespace SonoBooking.Application.Mapping
{
    public partial class MappingService
    {
        public void MapRequestAttach()
        {
            CreateMap<RequestAttach, RequestAttachDto>()
                .ForMember(dest => dest.FileName, opt => opt.MapFrom(src => src.Attachment.FileName))
                .ForMember(dest => dest.Extension, opt => opt.MapFrom(src => src.Attachment.Extension))
                .ForMember(dest => dest.Url, opt => opt.MapFrom(src => src.Attachment.Url))
                .ReverseMap();

            CreateMap<RequestAttach, EditRequestAttachDto>().ReverseMap();

            CreateMap<AddRequestAttachDto, RequestAttach>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ReverseMap();
        }
    }
}
