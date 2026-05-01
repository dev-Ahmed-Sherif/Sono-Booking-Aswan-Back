using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SonoTracker.Common.DTO.Base;

namespace SonoTracker.Common.DTO.Lookup.GeoPoint.Parameters
{
    [ExcludeFromCodeCoverage]
    public class GeoPointFilter : MainFilter
    {
        public string North { get; set; } = string.Empty;
        public string East { get; set; } = string.Empty;
    }
}
