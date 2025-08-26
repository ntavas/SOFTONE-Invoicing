namespace Invoicing.Api.Responses;

/// <summary>
/// One consistent wrapper for everything the API returns.<br/>
/// - On success: { success: true,  data: {...}, errors: null, traceId: "..." }<br/>
/// - On error:   { success: false, data: null, errors: [...], traceId: "..." }
/// </summary>
public sealed class ApiResponse<T>
{
    public bool Success { get; init; }
    public T? Data { get; init; }
    public List<ApiError>? Errors { get; init; }
    public string? TraceId { get; init; }

    public static ApiResponse<T> Ok(T data, string? traceId = null) =>
        new() { Success = true, Data = data, TraceId = traceId };

    public static ApiResponse<T> Fail(IEnumerable<ApiError> errors, string? traceId = null) =>
        new() { Success = false, Errors = errors.ToList(), TraceId = traceId };
}

public sealed record ApiError(string Code, string Message, string? Field = null);