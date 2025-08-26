using Invoicing.Application.DTOs;
using Invoicing.Domain.Entities;

namespace Invoicing.Application.Mappings;

/// <summary>Transforms User entity to dto.</summary>
public static class UserMapper
{
    public static UserDto ToDto(this User u) =>
        new(u.UserId, u.CompanyId, u.DisplayName, u.Email, u.IsActive);
}