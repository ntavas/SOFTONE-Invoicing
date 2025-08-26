using Invoicing.Domain.Entities;

namespace Invoicing.Application.Repositories;

/// <summary>
/// Data access port for users. Reads are AsNoTracking by default.
/// </summary>
public interface IUserRepository
{
    Task<List<User>> GetAllAsync(CancellationToken ct = default);
}