using Microsoft.EntityFrameworkCore;
using MonitorGlass.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MonitorGlass.Infrastructure.Data.Configurations;

internal sealed class SystemMetricConfiguration : IEntityTypeConfiguration<SystemMetric>
{
    public void Configure(EntityTypeBuilder<SystemMetric> builder)
    {
        builder.Property(x => x.Id).IsRequired().ValueGeneratedOnAdd();

        builder.HasOne(x => x.SystemInfo)
            .WithMany(x => x.Metrics)
            .HasForeignKey(x => x.SystemInfoId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.OwnsOne(x => x.CpuDetail, cpu => cpu.Property(x => x.CpuCoreUsage).HasPrecision(10, 2));
        builder.OwnsOne(x => x.MemoryDetail);

        builder.HasMany(x => x.DiskDetails)
            .WithOne(x => x.SystemMetric)
            .HasForeignKey(c => c.SystemMetricId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(x => x.NetworkDetails)
            .WithOne(x => x.SystemMetric)
            .HasForeignKey(x => x.SystemMetricId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}