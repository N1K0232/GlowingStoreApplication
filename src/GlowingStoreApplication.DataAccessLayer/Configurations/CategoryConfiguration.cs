﻿using GlowingStoreApplication.DataAccessLayer.Configurations.Common;
using GlowingStoreApplication.DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GlowingStoreApplication.DataAccessLayer.Configurations;

public class CategoryConfiguration : BaseEntityConfiguration<Category>
{
    public override void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.Property(c => c.Name).HasMaxLength(256).IsRequired();
        builder.Property(c => c.Description).HasMaxLength(512).IsRequired(false);

        builder.ToTable("Categories");
        base.Configure(builder);
    }
}