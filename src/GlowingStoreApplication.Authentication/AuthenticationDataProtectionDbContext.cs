using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GlowingStoreApplication.Authentication;

public partial class AuthenticationDbContext : IDataProtectionKeyContext
{
    public DbSet<DataProtectionKey> DataProtectionKeys => Set<DataProtectionKey>();

    partial void OnModelCreatingCore(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DataProtectionKey>(builder =>
        {
            builder.ToTable("DataProtectionKeys");
            builder.HasKey(key => key.Id);

            builder.Property(key => key.FriendlyName).HasColumnType("NVARCHAR(MAX)").IsRequired(false);
            builder.Property(key => key.Xml).HasColumnType("NVARCHAR(MAX)").IsRequired(false);
        });
    }
}