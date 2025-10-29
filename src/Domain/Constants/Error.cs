namespace MonitorGlass.Domain.Constants;

public record Error
{
    public const string UnauthorizeError = "Session is expired or user is not logged in.";
}
