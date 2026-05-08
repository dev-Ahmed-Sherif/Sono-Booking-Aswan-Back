using System;
using System.Collections.Generic;
using System.Linq;
using FuzzySharp;

namespace SonoBooking.Common.Helpers
{
    public static class LookupDuplicateGuard
    {
        public static string NormalizeText(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            var parts = value.Trim().Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
            return string.Join(' ', parts).ToLowerInvariant();
        }

        public static bool HasFuzzyNameDuplicate<T>(
            IEnumerable<T> existing,
            Func<T, string> nameArSelector,
            Func<T, string> nameEnSelector,
            string modelNameAr,
            string modelNameEn,
            int nameThreshold = 90)
        {
            var list = existing as IList<T> ?? [.. existing];
            var normalizedNameAr = NormalizeText(modelNameAr);
            var normalizedNameEn = NormalizeText(modelNameEn);

            var nameArDuplicate = list.Any(x =>
                !string.IsNullOrWhiteSpace(nameArSelector(x)) &&
                !string.IsNullOrWhiteSpace(normalizedNameAr) &&
                Fuzz.TokenSetRatio(normalizedNameAr, NormalizeText(nameArSelector(x))) >= nameThreshold);

            var nameEnDuplicate = list.Any(x =>
                !string.IsNullOrWhiteSpace(nameEnSelector(x)) &&
                !string.IsNullOrWhiteSpace(normalizedNameEn) &&
                Fuzz.TokenSetRatio(normalizedNameEn, NormalizeText(nameEnSelector(x))) >= nameThreshold);

            return nameArDuplicate || nameEnDuplicate;
        }

        public static bool HasFuzzyCodeDuplicate<T>(
            IEnumerable<T> existing,
            Func<T, string> codeSelector,
            string modelCode,
            int threshold = 90)
        {
            var list = existing as IList<T> ?? [.. existing];
            var normalizedCode = NormalizeText(modelCode);
            if (string.IsNullOrWhiteSpace(normalizedCode))
                return false;

            return list.Any(x =>
                !string.IsNullOrWhiteSpace(codeSelector(x)) &&
                Fuzz.Ratio(normalizedCode, NormalizeText(codeSelector(x))) >= threshold);
        }
    }
}

