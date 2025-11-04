using MonitorGlass.Domain.Constants;

namespace MonitorGlass.Application.User.Queries.UserProfile;

public record UserProfileQuery : IRequest<UserDto?>;

internal sealed class UserProfileQueryHandler(IIdentityService identityService, IUser currentUser) : IRequestHandler<UserProfileQuery, UserDto?>
{
    private readonly IIdentityService _identityService = identityService;
    private readonly IUser _currentUser = currentUser;
    public async Task<UserDto?> Handle(UserProfileQuery request, CancellationToken cancellationToken)
    {
        return string.IsNullOrEmpty(_currentUser.Id)
            ? throw new UnauthorizedAccessException(Error.UnauthorizeError)
            : await _identityService.GetUserAsync(_currentUser.Id, cancellationToken);
    }
}
