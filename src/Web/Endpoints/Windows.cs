using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MonitorGlass.Application.Common.Models;
using MonitorGlass.Application.Windows.Commands.DeleteServer;
using MonitorGlass.Application.Windows.Commands.AddServer;
using MonitorGlass.Application.Windows.Queries.GetWindowsServers;
using MonitorGlass.Infrastructure.Hubs;
using Microsoft.AspNetCore.Http.Connections;

namespace MonitorGlass.Web.Endpoints;

public class Windows : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapHub<WindowsHub>("/windows_metric", o => o.Transports = HttpTransportType.WebSockets);

        groupBuilder.MapPost(AddServer, nameof(AddServer).ToLower())
        .WithSummary("Add new server")
        .WithDescription("Adds a new server to be monitored.")
        .Produces<Result>(StatusCodes.Status200OK)
        .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
        .WithOpenApi();

        groupBuilder.MapDelete(DeleteServer, nameof(DeleteServer).ToLower())
        .WithSummary("Delete server")
        .WithDescription("Delete the server from the application, but it won't delete the historical data.")
        .Produces<Result>()
        .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
        .Produces<Result>(StatusCodes.Status400BadRequest)
        .WithOpenApi();

        groupBuilder.MapGet(GetWindowsServers, nameof(GetWindowsServers).ToLower())
        .RequireAuthorization()
        .WithSummary("Windows servers")
        .WithDescription("Get the windows server.")
        .Produces<WindowsDto>()
        .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
        .WithOpenApi();
    }

    async Task<Results<Ok<Result>, BadRequest>> AddServer(ISender sender, AddServerCommand command)
    {
        var response = await sender.Send(command);
        return response.Succeeded
        ? TypedResults.Ok(response)
        : TypedResults.BadRequest();
    }

    async Task<Results<Ok<Result>, BadRequest<Result>, BadRequest>> DeleteServer(ISender sender, Guid serverId)
    {
        var response = await sender.Send(new DeleteServerCommand(serverId));
        return response.Succeeded
        ? TypedResults.Ok(response)
        : TypedResults.BadRequest(response);
    }

    async Task<Results<Ok<WindowsDto>, NotFound>> GetWindowsServers(ISender sender)
    {
        var response = await sender.Send(new GetWindowsServersQuery());
        return response is null
        ? TypedResults.NotFound()
        : TypedResults.Ok(response);
    }
}
