using System;
using System.Diagnostics.CodeAnalysis;
using SonoTracker.Common.DTO.Base;

namespace SonoTracker.Common.DTO.Lookup.Priority
{
    [ExcludeFromCodeCoverage]
    public class PriorityDto : LookupDto<Guid?>
    {
        public DateTime CreatedDate { get; set; }
    }
}
