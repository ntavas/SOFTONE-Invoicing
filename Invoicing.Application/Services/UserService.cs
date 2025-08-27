using Invoicing.Application.DTOs;
using Invoicing.Application.Mappings;
using Invoicing.Application.Repositories;
using Invoicing.Domain.Common;

namespace Invoicing.Application.Services;

public sealed class UserService(IUserRepository repo) : Interfaces.IUserService
{
    public async Task<Result<IReadOnlyList<UserDto>>> GetAllAsync(CancellationToken ct = default)
    {
        var users = await repo.GetAllAsync(ct);
        var userDtos = users.Select(u => u.ToDto()).ToList();
        return Result<IReadOnlyList<UserDto>>.Success(userDtos);
    }
}