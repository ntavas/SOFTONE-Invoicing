using System.Net;
using Invoicing.Api.Responses;
using Invoicing.Domain.Common;

namespace Invoicing.Api.Middleware;
/// <summary>
/// Last-chance guard: catches unhandled exceptions and returns a consistent 500 envelope.
/// Keeps stack traces in logs, not in responses.
/// </summary>
public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    
    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext ctx)
    {
        try
        {
            await _next(ctx);
        }
        catch (Exception ex)
        {
            var traceId = ctx.TraceIdentifier;
            _logger.LogError(ex, "Unhandled exception for {TraceId}", traceId);

            ctx.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            ctx.Response.ContentType = "application/json";

            var body = ApiResponse<object>.Fail(
                new[] { new ApiError(ErrorCatalog.Internal, "An unexpected error occurred.") },
                traceId
            );

            await ctx.Response.WriteAsJsonAsync(body);
        }
    }
}