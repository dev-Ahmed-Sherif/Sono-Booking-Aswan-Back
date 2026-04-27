using System.Diagnostics.CodeAnalysis;
using System.Linq;
using SonoTracker.Common.Helpers.MediaUploader;

namespace SonoTracker.Common.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class Base64Extensions
    {
        public static (string extension, string data) GetBase64StringContents(this string input)
        {
            input = input.Replace("data:", "");
            var parts = input.Split(';').ToList<string>();
            return (MimeTypeMap.GetExtension(parts[0]), parts[1].Replace("base64,", ""));
        }
    }
}