using Microsoft.AspNetCore.Identity;

namespace MonitorGlass.Domain.Entities;

public class ApplicationRole : IdentityRole<Guid>
{
    public string? Description { get; set; }
    public ApplicationRole() { }
    public ApplicationRole(string roleName) : base(roleName) { }
    public ApplicationRole(string roleName, string description) : base(roleName)
    {
        Description = description;
    }
}
