using Microsoft.EntityFrameworkCore;
using MonitorGlass.Domain.Entities;

namespace MonitorGlass.Infrastructure.Data.Configurations;

internal sealed class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.Property(x => x.DisplayName).IsRequired().HasMaxLength(50);
    }
}
