using SonoBooking.Common.DTO.Housing.Payment;
using SonoBooking.Domain.Entities.Housing;

namespace SonoBooking.Application.Mapping
{
    public partial class MappingService
    {
        public void MapPayment()
        {
            CreateMap<Payment, PaymentDto>().ReverseMap();

            CreateMap<Payment, EditPaymentDto>().ReverseMap();

            CreateMap<AddPaymentDto, Payment>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ReverseMap();
        }
    }
}
