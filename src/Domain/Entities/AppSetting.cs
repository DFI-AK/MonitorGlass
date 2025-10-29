namespace MonitorGlass.Domain.Entities;

public class AppSetting : BaseEntity<Guid>
{
    public Guid UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;
    public string? Key { get; set; }
    public string? Value { get; set; }
}