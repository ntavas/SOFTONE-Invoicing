using Invoicing.Application.DTOs;

namespace Invoicing.Application.Services.Interfaces;

public interface IUserService
{
    Task<IReadOnlyList<UserDto>> GetAllAsync(CancellationToken ct = default);
}