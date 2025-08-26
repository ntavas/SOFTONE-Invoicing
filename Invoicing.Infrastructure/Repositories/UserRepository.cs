using Invoicing.Application.Repositories;
using Invoicing.Domain.Entities;
using Invoicing.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Invoicing.Infrastructure.Repositories;

public sealed class UserRepository(InvoicingDbContext db) : IUserRepository
{
    public Task<List<User>> GetAllAsync(CancellationToken ct = default) =>
        db.Users.AsNoTracking().ToListAsync(ct);
}