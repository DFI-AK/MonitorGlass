using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.Features;

namespace MonitorGlass.Web.Infrastructure;

internal sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, IProblemDetailsService problemDetailsService, IWebHostEnvironment hostEnvironment) : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger = logger;
    private readonly IProblemDetailsService _problemDetailsService = problemDetailsService;
    private readonly IWebHostEnvironment _hostEnvironment = hostEnvironment;

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var (statusCode, title) = exception switch
        {
            ApplicationException or InvalidOperationException or ArgumentNullException => (StatusCodes.Status400BadRequest, "Bad Request"),
            UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Unauthorized"),
            KeyNotFoundException => (StatusCodes.Status404NotFound, "Not Found"),
            _ => (StatusCodes.Status500InternalServerError, "Internal Server Error"),
        };

        httpContext.Response.StatusCode = statusCode;

        var detailMessage = exception.Message;

        var activity = httpContext.Features.Get<IHttpActivityFeature>()?.Activity;

        return await _problemDetailsService.TryWriteAsync(new()
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new()
            {
                Status = statusCode,
                Title = title,
                Detail = detailMessage,
                Type = exception.GetType().Name,
                Extensions =
                {
                    ["request_id"] = activity?.Id ?? httpContext.TraceIdentifier
                }
            }
        });

    }
}
