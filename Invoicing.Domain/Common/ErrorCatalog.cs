namespace Invoicing.Domain.Common;

public static class ErrorCatalog
{
    public const string Validation   = "validation";
    public const string Unauthorized = "unauthorized";
    public const string Forbidden    = "forbidden";
    public const string NotFound     = "not_found";
    public const string Conflict     = "conflict";
    public const string Internal     = "internal_error";
}