using Microsoft.EntityFrameworkCore;
using MonitorGlass.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MonitorGlass.Infrastructure.Data.Configurations;

internal sealed class SqlDatabaseConfiguration : IEntityTypeConfiguration<SqlDatabase>
{
    public void Configure(EntityTypeBuilder<SqlDatabase> builder)
    {
        builder.Property(x => x.Id).IsRequired().ValueGeneratedOnAdd();

        builder.Property(x => x.Name).IsRequired();

        builder.Property(x => x.SizeMB).IsRequired();

        builder.HasOne(x => x.SqlServerInstance)
            .WithMany(x => x.Databases)
            .HasForeignKey(x => x.SqlServerInstanceId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}