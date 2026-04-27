using SonoTracker.Domain.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SonoTracker.Domain.Entities.Lookups
{
    public class City : BaseEntity<string>
    {
        public City()
        {
            if (string.IsNullOrEmpty(Id))
            {
                Id = Guid.CreateVersion7().ToString();
            }
        }

        [Required, MaxLength(35)]
        public required string Code { get; set; }
        [Required, MaxLength(280)]
        public required string NameAr { get; set; }
        [MaxLength(280)]
        public string? NameEn { get; set; }

        [MaxLength(50), ForeignKey(nameof(Governorate))]
        public string? GovernorateId { get; set; }
        public virtual Governorate? Governorate { get; set; }

        public virtual HashSet<Town> Towns { get; set; } = [];
    }
}
