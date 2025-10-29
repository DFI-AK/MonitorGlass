namespace MonitorGlass.Domain.Common;

public interface IAuditableEntity
{
    DateTimeOffset Created { get; set; }
    DateTimeOffset LastModified { get; set; }
}
