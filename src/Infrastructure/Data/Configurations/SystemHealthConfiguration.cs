using Microsoft.EntityFrameworkCore;
using MonitorGlass.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MonitorGlass.Infrastructure.Data.Configurations;

internal sealed class SystemHealthConfiguration : IEntityTypeConfiguration<SystemHealth>
{
    public void Configure(EntityTypeBuilder<SystemHealth> builder)
    {
        builder.Property(x => x.Id).IsRequired().ValueGeneratedOnAdd();

        builder.HasOne(x => x.SystemInfo)
            .WithMany(x => x.SystemHealths)
            .HasForeignKey(x => x.SystemInfoId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}