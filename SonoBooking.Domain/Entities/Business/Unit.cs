using SonoBooking.Domain.Entities.Identity;
using SonoBooking.Domain.Entities.Lookups;
using SonoTracker.Domain.Entities.Base;
using SonoTracker.Domain.Entities.Lookups;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SonoBooking.Domain.Entities.Business
{
    public class Unit : Lookup<string>
    {
        public Unit() 
        {
            if (string.IsNullOrEmpty(Id))
            {
                Id = Guid.CreateVersion7().ToString();
            }
        }

        public Gender? Gender { get; set; }

        [Required, MaxLength(50)]
        [ForeignKey(nameof(UnitType))]
        public required string UnitTypeId { get; set; }
        public virtual UnitType? UnitType { get; set; }
    }
}
