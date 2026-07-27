using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SonoBooking.Common.Constants.WhatsApp;
using SonoBooking.Common.DTO.WhatsApp;

namespace SonoBooking.Application.Services.WhatsApp
{
    public class WhatsAppService(
        IOptions<WhatsAppSettings> whatsAppSettings,
        HttpClient httpClient,
        ILogger<WhatsAppService> logger) : IWhatsAppService
    {
        private readonly WhatsAppSettings _settings = whatsAppSettings.Value;

        public async Task SendMessageAsync(string toPhoneNumber, string message)
        {
            if (!_settings.Enabled)
            {
                logger.LogDebug("WhatsApp messaging is disabled; skipping send.");
                return;
            }

            string? digits = WhatsAppPhoneNormalizer.ToDigits(toPhoneNumber, _settings.DefaultCountryCode);
            if (digits == null)
            {
                logger.LogWarning("Invalid WhatsApp recipient phone number: {PhoneNumber}", toPhoneNumber);
                return;
            }

            if (string.IsNullOrWhiteSpace(message))
            {
                logger.LogWarning("WhatsApp message body is empty; skipping send to {PhoneNumber}.", digits);
                return;
            }

            string provider = _settings.Provider?.Trim() ?? WhatsAppProviders.Meta;

            try
            {
                if (provider.Equals(WhatsAppProviders.Twilio, StringComparison.OrdinalIgnoreCase))
                {
                    await SendViaTwilioAsync(digits, message.Trim());
                }
                else if (provider.Equals(WhatsAppProviders.Meta, StringComparison.OrdinalIgnoreCase))
                {
                    await SendViaMetaAsync(digits, message.Trim());
                }
                else
                {
                    logger.LogWarning("Unsupported WhatsApp provider '{Provider}'.", provider);
                }
            }
            catch (Exception ex) when (ex is not InvalidOperationException)
            {
                logger.LogError(ex, "Failed to send WhatsApp message to {PhoneNumber} via {Provider}", digits, provider);
                throw;
            }
        }

        private async Task SendViaMetaAsync(string digits, string message)
        {
            if (string.IsNullOrWhiteSpace(_settings.PhoneNumberId) ||
                string.IsNullOrWhiteSpace(_settings.AccessToken))
            {
                logger.LogWarning("Meta WhatsApp settings are incomplete; skipping send.");
                return;
            }

            string requestUrl = $"{_settings.ApiBaseUrl.TrimEnd('/')}/{_settings.PhoneNumberId}/messages";
            var payload = new
            {
                messaging_product = "whatsapp",
                recipient_type = "individual",
                to = digits,
                type = "text",
                text = new
                {
                    preview_url = false,
                    body = message
                }
            };

            using var request = new HttpRequestMessage(HttpMethod.Post, requestUrl);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _settings.AccessToken);
            request.Content = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json");

            await SendAndValidateAsync(request, digits, WhatsAppProviders.Meta);
        }

        private async Task SendViaTwilioAsync(string digits, string message)
        {
            if (string.IsNullOrWhiteSpace(_settings.AccountSid) ||
                string.IsNullOrWhiteSpace(_settings.AuthToken) ||
                string.IsNullOrWhiteSpace(_settings.FromPhoneNumber))
            {
                logger.LogWarning("Twilio WhatsApp settings are incomplete; skipping send.");
                return;
            }

            string requestUrl =
                $"https://api.twilio.com/2010-04-01/Accounts/{_settings.AccountSid.Trim()}/Messages.json";

            using var request = new HttpRequestMessage(HttpMethod.Post, requestUrl);
            request.Headers.Authorization = new AuthenticationHeaderValue(
                "Basic",
                Convert.ToBase64String(
                    Encoding.ASCII.GetBytes($"{_settings.AccountSid.Trim()}:{_settings.AuthToken.Trim()}")));

            request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["To"] = WhatsAppPhoneNormalizer.ToTwilioAddress(digits),
                ["From"] = NormalizeTwilioFromAddress(_settings.FromPhoneNumber),
                ["Body"] = message
            });

            await SendAndValidateAsync(request, digits, WhatsAppProviders.Twilio);
        }

        private async Task SendAndValidateAsync(HttpRequestMessage request, string digits, string provider)
        {
            using HttpResponseMessage response = await httpClient.SendAsync(request);
            string responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                logger.LogError(
                    "Failed to send WhatsApp message to {PhoneNumber} via {Provider}. Status: {StatusCode}. Response: {ResponseBody}",
                    digits,
                    provider,
                    response.StatusCode,
                    responseBody);
                throw new InvalidOperationException($"{provider} WhatsApp API returned {(int)response.StatusCode}.");
            }

            logger.LogInformation(
                "WhatsApp message sent successfully to {PhoneNumber} via {Provider}",
                digits,
                provider);
        }

        private static string NormalizeTwilioFromAddress(string fromPhoneNumber)
        {
            string trimmed = fromPhoneNumber.Trim();
            if (trimmed.StartsWith("whatsapp:", StringComparison.OrdinalIgnoreCase))
                return trimmed;

            if (trimmed.StartsWith('+'))
                return $"whatsapp:{trimmed}";

            return $"whatsapp:+{trimmed}";
        }
    }
}
