using SonoTracker.Common.DTO.Base;
using SonoTracker.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SonoTracker.Common.DTO.Lookup.UnitType
{
   [ExcludeFromCodeCoverage]
   public class AddUnitTypeDto : LookupDto<string>
   {
       public UnitCategory UnitCategory { get; set; }
   }
}
