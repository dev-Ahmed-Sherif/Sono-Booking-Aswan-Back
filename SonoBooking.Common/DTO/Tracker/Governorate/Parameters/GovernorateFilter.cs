using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SonoTracker.Common.DTO.Tracker.Governorate.Parameters
{
    [ExcludeFromCodeCoverage]
    public class GovernorateFilter
    {

        public string Name { get; set; }

        public string Url { get; set; }

        public bool IsDeleted { get; set; } = false;
    }
}
