using Invoicing.Application.DTOs;
using Invoicing.Application.Repositories;

namespace Invoicing.Application.Services;

public sealed class UserService(IUserRepository repo) : Interfaces.IUserService
{
    public async Task<IReadOnlyList<UserDto>> GetAllAsync(CancellationToken ct = default)
    {
        var users = await repo.GetAllAsync(ct);
        return users.Select(u => new UserDto(u.UserId, u.CompanyId, u.DisplayName, u.Email, u.IsActive))
            .ToList();
    }
}