using Invoicing.Api.Responses;
using Invoicing.Domain.Common;
using Microsoft.AspNetCore.Mvc;

namespace Invoicing.Api.Extensions;

/// <summary>
/// Turns a domain/application Result{T} into an HTTP response with our standard envelope.
/// </summary>
public static class ResultToActionResultExtension
{
    public static IActionResult ToActionResult<T>(
        this ControllerBase ctrl,
        Result<T> result,
        ILogger logger)
    {
        var traceId = ctrl.HttpContext.TraceIdentifier;

        if (result.IsSuccess)
        {
            logger.LogInformation("Request {TraceId} succeeded", traceId);
            return ctrl.Ok(ApiResponse<T>.Ok(result.Value!, traceId));
        }
        
        var apiErrors = result.Errors.Select(e => new ApiError(e.Code, e.Message, e.Field)).ToList();
        var status = MapStatus(result.Errors);

        logger.LogInformation(
            "Request {TraceId} failed with status {Status} and {ErrorCount} errors: {Codes}",
            traceId, status, apiErrors.Count, string.Join(",", apiErrors.Select(e => e.Code)));

        return status switch
        {
            400 => ctrl.BadRequest(ApiResponse<T>.Fail(apiErrors, traceId)),
            401 => ctrl.Unauthorized(ApiResponse<T>.Fail(apiErrors, traceId)),
            403 => ctrl.StatusCode(403, ApiResponse<T>.Fail(apiErrors, traceId)),
            404 => ctrl.NotFound(ApiResponse<T>.Fail(apiErrors, traceId)),
            409 => ctrl.Conflict(ApiResponse<T>.Fail(apiErrors, traceId)),
            422 => ctrl.UnprocessableEntity(ApiResponse<T>.Fail(apiErrors, traceId)),
            _   => ctrl.StatusCode(status, ApiResponse<T>.Fail(apiErrors, traceId))
        };
    }

    // Small, predictable mapping from portable error codes to HTTP status.
    private static int MapStatus(IReadOnlyList<Error> errors)
    {
        foreach (var e in errors)
        {
            var code = e.Code.ToLowerInvariant();
            if (code is "unauthorized") return 401;
            if (code is "forbidden")    return 403;
            if (code.Contains("not_found")) return 404;
            if (code.Contains("conflict"))  return 409;
            if (code is "validation")  return 400;
        }
        return 400;
    }
}