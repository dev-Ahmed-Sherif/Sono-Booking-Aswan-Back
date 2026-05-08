using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Task = System.Threading.Tasks.Task;

namespace SonoBooking.Api.MiddleWares
{
    /// <summary>
    /// Language Middleware
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class LanguageMiddleware
    {
        private readonly RequestDelegate _next;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="next"></param>
        public LanguageMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// Invoke
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>

        public async Task InvokeAsync(HttpContext httpContext)
        {
            var languageHeader = httpContext.Request.Headers["Accept-Language"];
            if (string.IsNullOrEmpty(languageHeader))
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            }
            else if(languageHeader == "ar")
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("ar-OM");
            }
            else
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            }

            await _next(httpContext);
        }


    }
}



