using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Trecco.Api.Middleware;

internal sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    : IExceptionHandler
{
    private const string ProblemType = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1";
    private static readonly ProblemDetails ServerFailureProblemDetails = new()
    {
        Title = "Server failure",
        Detail = "An unexpected error occurred",
        Status = StatusCodes.Status500InternalServerError,
        Type = ProblemType
    };

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        httpContext.Response.StatusCode = ServerFailureProblemDetails.Status!.Value;
        httpContext.Response.ContentType = "application/problem+json";

        logger.LogError(
            exception,
            "Status Code: {StatusCode}, TraceId: {TraceId}, Message: {Message}, Machine: {MachineName}",
            ServerFailureProblemDetails.Status,
            Activity.Current?.Id ?? httpContext.TraceIdentifier,
            exception.Message,
            Environment.MachineName
        );

        await httpContext.Response.WriteAsJsonAsync(ServerFailureProblemDetails, cancellationToken);

        return true;
    }
}
