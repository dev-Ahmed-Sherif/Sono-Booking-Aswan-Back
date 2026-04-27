using System.Diagnostics.CodeAnalysis;

namespace SonoTracker.Common.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class SearchResultNotFoundException : BaseException
    {
        public SearchResultNotFoundException() : base("Result not found")
        {

        }
    }
}
