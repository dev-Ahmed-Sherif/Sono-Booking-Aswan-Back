using System;
using System.Collections.Generic;

namespace SonoBooking.Common.Constants
{
    /// <summary>
    /// Seeded request type lookup IDs. Display order: mission, medical, personal.
    /// </summary>
    public static class RequestTypeIds
    {
        public const string Mission = "019f0d6c-3a9b-7a5c-9c9b-5b44b1a1f001";
        public const string Medical = "019f0d6c-3a9b-7a5c-9c9b-5b44b1a1f002";
        public const string Personal = "019f0d6c-3a9b-7a5c-9c9b-5b44b1a1f003";

        private static readonly Dictionary<string, int> SortOrder = new(StringComparer.Ordinal)
        {
            [Mission] = 0,
            [Medical] = 1,
            [Personal] = 2,
        };

        public static int GetSortOrder(string requestTypeId) =>
            SortOrder.TryGetValue(requestTypeId, out int order) ? order : int.MaxValue;
    }
}
