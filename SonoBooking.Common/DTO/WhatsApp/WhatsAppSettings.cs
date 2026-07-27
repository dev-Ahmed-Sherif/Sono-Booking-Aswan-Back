namespace SonoBooking.Common.DTO.WhatsApp
{
    using SonoBooking.Common.Constants.WhatsApp;

    public class WhatsAppSettings
    {
        public bool Enabled { get; set; }

        /// <summary>
        /// Supported values: Meta or Twilio.
        /// </summary>
        public string Provider { get; set; } = WhatsAppProviders.Meta;

        public string DefaultCountryCode { get; set; } = "20";

        // Meta Cloud API
        public string ApiBaseUrl { get; set; } = "https://graph.facebook.com/v21.0";

        public string PhoneNumberId { get; set; }

        public string AccessToken { get; set; }

        // Twilio
        public string AccountSid { get; set; }

        public string AuthToken { get; set; }

        /// <summary>
        /// Twilio WhatsApp sender, e.g. whatsapp:+14155238886
        /// </summary>
        public string FromPhoneNumber { get; set; }
    }
}
