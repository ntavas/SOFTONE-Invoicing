namespace Invoicing.Domain.Common;


/// <summary>
/// A domain/application error that is being returned to the Api client.
/// </summary>
public sealed record Error(string Code, string Message, string? Field = null)
{
    public static Error Validation(string field, string message)
        => new(ErrorCatalog.Validation, message, field);

    public static Error NotFound(string code, string message)
        => new(code, message);

    public static Error Conflict(string code, string message)
        => new(code, message);

    public static Error Unauthorized(string message = "Unauthorized")
        => new(ErrorCatalog.Unauthorized, message);

    public static Error Forbidden(string message = "Forbidden")
        => new(ErrorCatalog.Forbidden, message);
}
