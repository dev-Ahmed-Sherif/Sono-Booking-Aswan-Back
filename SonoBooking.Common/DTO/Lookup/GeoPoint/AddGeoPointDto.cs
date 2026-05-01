using SonoTracker.Common.Core;
using SonoTracker.Common.DTO.Base;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SonoTracker.Common.DTO.Lookup.GeoPoint
{
    [ExcludeFromCodeCoverage]
    public class AddGeoPointDto : IEntityDto<string>
    {
        public string Id { get; set; }
        public string Code { get; set; }
        public string NameAr { get; set; }
        public string NameEn { get; set; }
        public string North { get; set; }
        public string East { get; set; }

    }
}
