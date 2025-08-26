namespace Invoicing.Application.DTOs;

public sealed record CompanyDto(
    int CompanyId,
    string Name,
    bool IsActive
);