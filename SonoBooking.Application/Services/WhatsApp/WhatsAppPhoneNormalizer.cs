using System;
using System.Text.RegularExpressions;

namespace SonoBooking.Application.Services.WhatsApp
{
    internal static partial class WhatsAppPhoneNormalizer
    {
        public static string? ToDigits(string phoneNumber, string defaultCountryCode)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                return null;

            string digits = NonDigitRegex().Replace(phoneNumber, string.Empty);
            if (digits.Length == 0)
                return null;

            if (digits.StartsWith('0'))
                digits = defaultCountryCode + digits[1..];

            if (digits.StartsWith(defaultCountryCode, StringComparison.Ordinal) &&
                digits.Length == defaultCountryCode.Length + 10)
            {
                return digits;
            }

            if (digits.Length == 10 && digits.StartsWith('1'))
                return defaultCountryCode + digits;

            return digits.Length >= 10 ? digits : null;
        }

        public static string ToE164(string digits) => $"+{digits}";

        public static string ToTwilioAddress(string digits) => $"whatsapp:{ToE164(digits)}";

        [GeneratedRegex(@"\D")]
        private static partial Regex NonDigitRegex();
    }
}
