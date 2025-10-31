using Microsoft.EntityFrameworkCore;
using MonitorGlass.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MonitorGlass.Infrastructure.Data.Configurations;

internal sealed class SystemInfoConfiguration : IEntityTypeConfiguration<WindowsServer>
{
    public void Configure(EntityTypeBuilder<WindowsServer> builder)
    {
        builder.Property(x => x.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Property(x => x.MachineName).IsRequired();
        builder.Property(x => x.OSVersion).IsRequired();
    }
}