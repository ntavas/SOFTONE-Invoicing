using Invoicing.Application.DTOs;
using Invoicing.Domain.Entities;

namespace Invoicing.Application.Mappings;

/// <summary>Transforms company entity to DTO.</summary>
public static class CompanyMapper
{
    public static CompanyDto ToDto(this Company c) =>
        new(c.CompanyId, c.Name, c.IsActive);  
}