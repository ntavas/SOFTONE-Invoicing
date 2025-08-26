using Invoicing.Application.Repositories;
using Invoicing.Domain.Entities;
using Invoicing.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Invoicing.Infrastructure.Repositories;

/// <summary>EF Core implementation of company lookups.</summary>
public sealed class CompanyRepository : ICompanyRepository
{
    private readonly InvoicingDbContext _dbContext;
    private readonly ILogger<CompanyRepository> _logger;
    
    public CompanyRepository(InvoicingDbContext dbContext, ILogger<CompanyRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    
    public Task<Company?> GetByIdAsync(int companyId, CancellationToken ct = default)
    {
        _logger.LogDebug("Fetching company {CompanyId}", companyId);
        return _dbContext.Companies.AsNoTracking().FirstOrDefaultAsync(c => c.CompanyId == companyId, ct);
    }
    
    public Task<Company?> GetByIdActiveAsync(int companyId, CancellationToken ct = default)
    {
        _logger.LogDebug("Fetching active company {CompanyId}", companyId);
        return _dbContext.Companies.AsNoTracking().FirstOrDefaultAsync(c => c.CompanyId == companyId && c.IsActive, ct);
    }

    public async Task<bool> ExistsAsync(int companyId, CancellationToken ct = default)
    {
        _logger.LogDebug("Checking existence of company {CompanyId}", companyId);
        return await _dbContext.Companies.AsNoTracking().AnyAsync(c => c.CompanyId == companyId, ct);
    }

    public Task<Company?> GetByApiTokenHashAsync(byte[] tokenHash, CancellationToken ct = default)
    {
        _logger.LogDebug("Fetching company by api token hash");
        return _dbContext.Companies.AsNoTracking().FirstOrDefaultAsync(c => c.ApiTokenHash == tokenHash, ct);
    }
}