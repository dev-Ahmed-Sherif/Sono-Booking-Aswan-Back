using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace SonoBooking.Common.Core
{
    [ExcludeFromCodeCoverage]
    public class PagingResult
    {
        public PagingResult(int pageNumber, int pageSize, int totalCount, object result, HttpStatusCode status, string message)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalCount = totalCount;
            Data = result;
            Status = status;
            Message = message;
        }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public object Data { get; set; }
        public HttpStatusCode Status { get; set; }
        public string Message { get; set; }
    }
}
