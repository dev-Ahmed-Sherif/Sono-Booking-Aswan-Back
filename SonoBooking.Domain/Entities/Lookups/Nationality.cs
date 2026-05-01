using SonoTracker.Domain.Entities.Base;
using SonoTracker.Domain.Entities.Tracker;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SonoTracker.Domain.Entities.Lookups
{
    [ExcludeFromCodeCoverage]
    public class Nationality : Lookup<string>
    {
        public Nationality()
        {
            if (string.IsNullOrEmpty(Id))
            {
                Id = Guid.CreateVersion7().ToString();
            }
        }
        public virtual HashSet<Organization> Organizations { get; set; } = [];

        public virtual HashSet<FloatingUnitStaff> FloatingUnitStaffs { get; set; } = [];

        public virtual HashSet<TripNationality> TripNationalities { get; set; } = [];

    }
}
