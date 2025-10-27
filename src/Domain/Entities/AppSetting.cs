namespace MonitorGlass.Domain.Entities;

public class AppSetting : BaseAuditableEntity
{
    public Guid UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;
    public string? Key { get; set; }
    public string? Value { get; set; }
}