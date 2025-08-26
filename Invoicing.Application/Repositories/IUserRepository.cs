using Invoicing.Domain.Entities;

namespace Invoicing.Application.Repositories;

public interface IUserRepository
{
    Task<List<User>> GetAllAsync(CancellationToken ct = default);
}