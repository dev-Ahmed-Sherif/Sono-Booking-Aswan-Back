using System.Threading.Tasks;

namespace SonoBooking.Application.Services.WhatsApp
{
    public interface IWhatsAppService
    {
        Task SendMessageAsync(string toPhoneNumber, string message);
    }
}
