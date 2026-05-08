using SonoBooking.Common.DTO.Lookup.Attachment;
using SonoBooking.Domain.Entities.Lookups;

namespace SonoBooking.Application.Mapping
{
    public partial class MappingService
    {
        public void MapAttachment()
        {
            CreateMap<Attachment, AttachmentDto>().ReverseMap();

            CreateMap<Attachment, EditAttachmentDto>().ReverseMap();

            CreateMap<AddAttachmentDto, Attachment>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ReverseMap();
        }
    }
}
