using SonoTracker.Domain.Entities.Base;
using SonoTracker.Domain.Entities.Tracker;
using SonoTracker.Domain.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SonoTracker.Domain.Entities.Lookups
{
    public class UnitType : Lookup<string>
    {
        public UnitType() 
        {
            if (string.IsNullOrEmpty(Id))
            {
                Id = Guid.CreateVersion7().ToString();
            }
        }
        public UnitCategory UnitCategory { get; set; }

        [MaxLength(50), ForeignKey(nameof(Governorate))]
        public string? GovernorateId { get; set; }
        public virtual Governorate? Governorate { get; set; }

        public virtual HashSet<FloatingUnit> FloatingUnits { get; set; } = [];
    }
}
