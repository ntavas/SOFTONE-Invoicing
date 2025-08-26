using Invoicing.Domain.Entities;

namespace Invoicing.Application.Repositories;

/// <summary>
/// Minimal company queries needed by our use-cases (auth + validations).
/// </summary>
public interface ICompanyRepository
{
    Task<Company?> GetByIdAsync(int companyId, CancellationToken ct = default);

    Task<Company?> GetByIdActiveAsync(int companyId, CancellationToken ct = default);
    
    Task<bool> ExistsAsync(int companyId, CancellationToken ct = default);
    
    Task<Company?> GetByApiTokenHashAsync(byte[] tokenHash, CancellationToken ct = default);
}