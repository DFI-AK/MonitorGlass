using Microsoft.EntityFrameworkCore;
using MonitorGlass.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MonitorGlass.Infrastructure.Data.Configurations;

internal sealed class SqlInstanceConfiguration : IEntityTypeConfiguration<SqlServerInstance>
{
    public void Configure(EntityTypeBuilder<SqlServerInstance> builder)
    {
        builder.Property(x => x.Id).IsRequired().ValueGeneratedOnAdd();

        builder.Property(x => x.InstanceName).IsRequired().HasMaxLength(256);

        builder.Property(x => x.ConnectionString).IsRequired();

        builder.Property(x => x.Version).IsRequired();
    }
}