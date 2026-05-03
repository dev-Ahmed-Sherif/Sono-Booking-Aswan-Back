using SonoTracker.Domain.Entities.Base;
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
    }
}
