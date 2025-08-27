using System.Security.Cryptography;
using System.Text;
using Invoicing.Api.Responses;
using Invoicing.Application.Repositories;

namespace Invoicing.Api.Middleware;

/// <summary>
/// B2B API-key auth. For any /api/invoice* request:<br/>
/// - Expect Authorization: Bearer {token}<br/>
/// - Hash with SHA-256 (no pepper), match company.api_token_hash<br/>
/// - On success, sets HttpContext.Items["CompanyId"] = int<br/>
/// - On failure, returns 401/403 with the ApiResponse envelope
/// </summary>
public sealed class ApiKeyAuthMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ApiKeyAuthMiddleware> _logger;

    public ApiKeyAuthMiddleware(RequestDelegate next, ILogger<ApiKeyAuthMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext ctx, ICompanyRepository companies)
    {
        _logger.LogDebug("Auth middleware executing (traceId={TraceId})", ctx.TraceIdentifier);
        var path = ctx.Request.Path.Value ?? string.Empty;

        // Only protect invoice endpoints.
        // Adjustable based on requirements (e.g., some endpoints have public access)
        if (!path.StartsWith("/api/invoice", StringComparison.OrdinalIgnoreCase))
        {
            await _next(ctx);
            return;
        }
        

        if (!ctx.Request.Headers.TryGetValue("Authorization", out var authHeader) ||
            string.IsNullOrWhiteSpace(authHeader))
        {
            await WriteError(ctx, StatusCodes.Status401Unauthorized, "unauthorized", "Missing Authorization header.");
            return;
        }

        var header = authHeader.ToString();
        _logger.LogInformation("Authorization header: {Header} (traceId={TraceId})", header, ctx.TraceIdentifier);
        const string prefix = "Bearer ";
        if (!header.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
        {
            await WriteError(ctx, StatusCodes.Status401Unauthorized, "unauthorized",
                "Authorization header must be 'Bearer <token>'.");
            return;
        }

        var token = header.Substring(prefix.Length).Trim();
        if (string.IsNullOrEmpty(token))
        {
            await WriteError(ctx, StatusCodes.Status401Unauthorized, "unauthorized", "Bearer token is empty.");
            return;
        }

        byte[] hash;
        using (var sha = SHA256.Create())
            hash = sha.ComputeHash(Encoding.UTF8.GetBytes(token));

        var company = await companies.GetByApiTokenHashAsync(hash, ctx.RequestAborted);
        if (company is null)
        {
            _logger.LogInformation("Auth failed: invalid token (traceId={TraceId})", ctx.TraceIdentifier);
            await WriteError(ctx, StatusCodes.Status401Unauthorized, "unauthorized", "Invalid token.");
            return;
        }

        if (!company.IsActive)
        {
            _logger.LogInformation("Auth rejected: inactive company {CompanyId} (traceId={TraceId})",
                company.CompanyId, ctx.TraceIdentifier);
            await WriteError(ctx, StatusCodes.Status403Forbidden, "forbidden", "Company is inactive.");
            return;
        }

        // Success: attach company id for controllers/services
        ctx.Items["CompanyId"] = company.CompanyId;
        await _next(ctx);
    }

    private static Task WriteError(HttpContext ctx, int status, string code, string message)
    {
        ctx.Response.StatusCode = status;
        ctx.Response.ContentType = "application/json";
        var body = ApiResponse<object>.Fail(new[] { new ApiError(code, message) }, ctx.TraceIdentifier);
        return ctx.Response.WriteAsJsonAsync(body);
    }
}