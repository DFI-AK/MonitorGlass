using Microsoft.EntityFrameworkCore;
using MonitorGlass.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MonitorGlass.Infrastructure.Data.Configurations;

internal sealed class SystemInfoConfiguration : IEntityTypeConfiguration<SystemInfo>
{
    public void Configure(EntityTypeBuilder<SystemInfo> builder)
    {
        builder.Property(x => x.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Property(x => x.MachineName).IsRequired();
        builder.Property(x => x.OSVersion).IsRequired();
    }
}