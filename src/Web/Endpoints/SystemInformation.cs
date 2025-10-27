
using Microsoft.AspNetCore.Http.HttpResults;
using MonitorGlass.Application.Common.Models;
using MonitorGlass.Application.SystemInformation.Commands.AddServer;

namespace MonitorGlass.Web.Endpoints;

public class SystemInformation : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapPost(AddServer, nameof(AddServer).ToLower())
        .WithSummary("Add new server")
        .WithDescription("Adds a new server to be monitored.")
        .Produces<Result>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .WithOpenApi();
    }

    async Task<Results<Created<Result>, BadRequest>> AddServer(ISender sender, AddServerCommand command)
    {
        var response = await sender.Send(command);
        return response.Succeeded
        ? TypedResults.Created(string.Empty, response)
        : TypedResults.BadRequest();
    }

}
