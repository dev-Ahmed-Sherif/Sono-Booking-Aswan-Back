using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace SonoTracker.Domain.Entities.Base
{
    [ExcludeFromCodeCoverage]
    public class BaseEntity<TKey>
    {
        [MaxLength(50)]
        public TKey? Id { get; set; }

        public bool IsDeleted { get; set; } = false;
    }
}