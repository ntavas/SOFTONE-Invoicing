using Invoicing.Application.DTOs;
using Invoicing.Domain.Common;

namespace Invoicing.Application.Services.Interfaces;

public interface IUserService
{
    Task<Result<IReadOnlyList<UserDto>>> GetAllAsync(CancellationToken ct = default);
}