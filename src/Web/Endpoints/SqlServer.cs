
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MonitorGlass.Application.Common.Models;
using MonitorGlass.Application.SqlServer.Commands.AddNewSqlServer;
using MonitorGlass.Application.SqlServer.Commands.DeleteSqlInstance;
using MonitorGlass.Application.SqlServer.Queries.GetSqlServers;

namespace MonitorGlass.Web.Endpoints;

public class SqlServer : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapPost(AddNewSqlServer, nameof(AddNewSqlServer).ToLower())
        .RequireAuthorization()
        .WithSummary("Add new SQL instance")
        .WithDescription("Add new SQL Instance to app.")
        .WithDisplayName("Create SQL Instance")
        .Produces<SqlServerDto>()
        .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
        .WithOpenApi();

        groupBuilder.MapGet(GetSqlLookup, nameof(GetSqlLookup).ToLower())
        .RequireAuthorization()
        .WithSummary("SQL Lookup")
        .WithDescription("Get the lookup object for SQL Server")
        .Produces<SqlServerLookup>()
        .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
        .WithOpenApi();

        groupBuilder.MapDelete(DeleteInstance, nameof(DeleteInstance).ToLower())
        .RequireAuthorization()
        .WithSummary("Delete instance")
        .WithDescription("Delete the SQL server instance from the applicationm, if user have the valid permission to delete it.")
        .Produces<Result>()
        .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
        .WithOpenApi();
    }

    async Task<Results<Created<SqlServerDto>, BadRequest>> AddNewSqlServer(ISender sender, AddNewSqlServerCommand command)
    {
        var response = await sender.Send(command);
        return response is not null
        ? TypedResults.Created(string.Empty, response)
        : TypedResults.BadRequest();
    }

    async Task<Results<Ok<SqlServerLookup>, BadRequest>> GetSqlLookup(ISender sender)
    {
        var lookup = await sender.Send(new GetSqlServersQuery());
        return lookup is null
        ? TypedResults.BadRequest()
        : TypedResults.Ok(lookup);
    }

    async Task<Results<Ok<Result>, NotFound>> DeleteInstance(ISender sender, Guid instanceId)
    {
        var response = await sender.Send(new DeleteSqlInstanceCommand(instanceId));
        return response.Succeeded
        ? TypedResults.Ok(response)
        : TypedResults.NotFound();
    }

}
