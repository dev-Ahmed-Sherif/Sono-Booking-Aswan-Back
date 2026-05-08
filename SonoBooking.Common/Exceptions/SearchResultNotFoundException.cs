using System.Diagnostics.CodeAnalysis;

namespace SonoBooking.Common.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class SearchResultNotFoundException : BaseException
    {
        public SearchResultNotFoundException() : base("Result not found")
        {

        }
    }
}

