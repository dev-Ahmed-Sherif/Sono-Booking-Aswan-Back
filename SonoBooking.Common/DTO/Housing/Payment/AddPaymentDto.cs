using SonoBooking.Common.Core;
using SonoBooking.Domain;
using System;
using System.Diagnostics.CodeAnalysis;

namespace SonoBooking.Common.DTO.Housing.Payment
{
    [ExcludeFromCodeCoverage]
    public class AddPaymentDto : IEntityDto<string>
    {
        public string Id { get; set; }
        public decimal Amount { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public DateTime PaymentDate { get; set; }
        public string TransactionReference { get; set; }
        public string ReservationId { get; set; }
    }
}
