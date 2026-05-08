using SonoBooking.Common.DTO.Housing.Approval;
using SonoBooking.Domain.Entities.Housing;

namespace SonoBooking.Application.Mapping
{
    public partial class MappingService
    {
        public void MapApproval()
        {
            CreateMap<Approval, ApprovalDto>().ReverseMap();

            CreateMap<Approval, EditApprovalDto>().ReverseMap();

            CreateMap<AddApprovalDto, Approval>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ReverseMap();
        }
    }
}
