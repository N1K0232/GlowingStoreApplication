using GlowingStoreApplication.DataAccessLayer.Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GlowingStoreApplication.DataAccessLayer.Configurations.Common;

public abstract class BaseEntityConfiguration<T> : IEntityTypeConfiguration<T> where T : BaseEntity
{
    public virtual void Configure(EntityTypeBuilder<T> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd().HasDefaultValueSql("newid()");

        builder.Property(x => x.CreationDate).ValueGeneratedOnAdd().HasDefaultValueSql("getutcdate()");
        builder.Property(x => x.LastModificationDate).IsRequired(false).ValueGeneratedOnUpdate();
    }
}