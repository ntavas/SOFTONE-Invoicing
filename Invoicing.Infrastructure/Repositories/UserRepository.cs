using Invoicing.Application.Repositories;
using Invoicing.Domain.Entities;
using Invoicing.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Invoicing.Infrastructure.Repositories;

/// <summary>EF Core implementation of user queries.</summary>
public sealed class UserRepository : IUserRepository
{
    private readonly InvoicingDbContext _dbContext;
    private readonly ILogger<UserRepository> _logger;
    
    public UserRepository(InvoicingDbContext db, ILogger<UserRepository> logger)
    {
        _dbContext = db;
        _logger = logger;
    }
    
    public Task<List<User>> GetAllAsync(CancellationToken ct = default)
    {
        _logger.LogDebug("Querying all users.");
        return _dbContext.Users.AsNoTracking().ToListAsync(ct);
    }
}