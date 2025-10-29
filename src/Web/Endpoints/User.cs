
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MonitorGlass.Application.Common.Models;
using MonitorGlass.Application.User.Queries.UserProfile;
using MonitorGlass.Domain.Entities;

namespace MonitorGlass.Web.Endpoints;

public class User : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapIdentityApi<ApplicationUser>();

        groupBuilder.MapGet(Me, nameof(Me).ToLower())
        .RequireAuthorization()
        .WithSummary("User profile")
        .WithDescription("Get the user object, after user logged in.")
        .Produces<UserDto>()
        .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
        .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
        .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
        .WithOpenApi();
    }

    async Task<Results<Ok<UserDto>, BadRequest, UnauthorizedHttpResult, NotFound>> Me(ISender sender)
    {
        var user = await sender.Send(new UserProfileQuery());
        return user is null
        ? TypedResults.NotFound()
        : TypedResults.Ok(user);
    }
}
