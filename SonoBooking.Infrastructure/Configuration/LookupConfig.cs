using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SonoTracker.Domain.Entities.Base;
using System.Diagnostics.CodeAnalysis;

namespace SonoTracker.Infrastructure.Configuration
{
    [ExcludeFromCodeCoverage]
    public class LookupConfig<TEntity,TId> : BaseConfig<TEntity, TId> where TEntity : Lookup<TId> where TId : struct
    {
        public override void Configure(EntityTypeBuilder<TEntity> builder)
        {
            base.Configure(builder);
            builder.Property(e => e.NameEn).HasMaxLength(350);
            builder.Property(e => e.NameAr).HasMaxLength(350);
            builder.Property(e => e.Code).HasMaxLength(100);
            builder.HasIndex(e => e.Code).IsUnique();
        }
    }
}
