using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace SonoTracker.Domain.Entities.Base
{
    [ExcludeFromCodeCoverage]
    public class Lookup<TKey> : BaseAudit<TKey>
    {
        [Required, MaxLength(35)]
        public  required string Code { get; set; }
        [Required, MaxLength(280)]
        public required string NameAr { get; set; }
        [MaxLength(280)]
        public string? NameEn { get; set; }
    }
}
