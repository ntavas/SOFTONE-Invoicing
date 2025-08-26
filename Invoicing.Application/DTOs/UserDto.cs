namespace Invoicing.Application.DTOs;

public sealed record UserDto(
    int UserId,
    int CompanyId,
    string? DisplayName,
    string? Email,
    bool IsActive
);