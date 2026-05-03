using SonoTracker.Domain.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace SonoBooking.Domain.Entities.Lookups
{
    [ExcludeFromCodeCoverage]
    public class BookingType : Lookup<string>
    {
        public BookingType()
        {
            if (string.IsNullOrEmpty(Id))
            {
                Id = Guid.CreateVersion7().ToString();
            }
        }
    }
}
