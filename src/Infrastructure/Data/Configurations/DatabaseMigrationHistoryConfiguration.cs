using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MonitorGlass.Domain.Entities;

namespace MonitorGlass.Infrastructure.Data.Configurations;

internal sealed class DatabaseMigrationHistoryConfiguration : IEntityTypeConfiguration<DatabaseMigrationHistory>
{
    public void Configure(EntityTypeBuilder<DatabaseMigrationHistory> builder)
    {
        builder.Property(x => x.Id).IsRequired().ValueGeneratedOnAdd();

        builder.HasOne(x => x.User)
            .WithMany(x => x.MigrationHistories)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
