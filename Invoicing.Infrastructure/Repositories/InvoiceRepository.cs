using Invoicing.Application.Repositories;
using Invoicing.Domain.Entities;
using Invoicing.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Invoicing.Infrastructure.Repositories;

/// <summary>EF Core implementation of invoice persistence and queries.</summary>
public sealed class InvoiceRepository : IInvoiceRepository
{
    private readonly InvoicingDbContext _dbContext;
    private readonly ILogger<InvoiceRepository> _logger;
    
    public InvoiceRepository(InvoicingDbContext dbContext, ILogger<InvoiceRepository> logger)
    {
        _dbContext = dbContext;
        _logger    = logger;
    }
    
    public async Task<Invoice> AddAsync(Invoice entity, CancellationToken ct = default)
    {
        _logger.LogDebug("Creating invoice for issuer {CompanyId} -> counterparty {CounterpartyCompanyId}",
            entity.CompanyId, entity.CounterpartyCompanyId);

        await _dbContext.Invoices.AddAsync(entity, ct);
        await _dbContext.SaveChangesAsync(ct);
        return entity; // InvoiceId populated by identity
    }

    public Task<List<Invoice>> GetInvoicesSentAsync(
        int companyId, int? counterpartyCompanyId, DateOnly? dateIssued, int? invoiceId, CancellationToken ct = default)
    {
        _logger.LogDebug("Querying sent invoices for company {CompanyId}", companyId);

        IQueryable<Invoice> q = _dbContext.Invoices.AsNoTracking()
            .Where(i => i.CompanyId == companyId);

        if (counterpartyCompanyId.HasValue)
            q = q.Where(i => i.CounterpartyCompanyId == counterpartyCompanyId.Value);

        if (dateIssued.HasValue)
            q = q.Where(i => i.DateIssued == dateIssued.Value);

        if (invoiceId.HasValue)
            q = q.Where(i => i.InvoiceId == invoiceId.Value);

        return q
            .OrderByDescending(i => i.DateIssued)
            .ThenByDescending(i => i.InvoiceId)
            .ToListAsync(ct);
    }

    public Task<List<Invoice>> GetInvoicesReceivedAsync(
        int companyId, int? counterpartyCompanyId, DateOnly? dateIssued, int? invoiceId, CancellationToken ct = default)
    {
        _logger.LogDebug("Querying received invoices for company {CompanyId}", companyId);

        IQueryable<Invoice> q = _dbContext.Invoices.AsNoTracking()
            .Where(i => i.CounterpartyCompanyId == companyId);

        if (counterpartyCompanyId.HasValue)
            q = q.Where(i => i.CompanyId == counterpartyCompanyId.Value);

        if (dateIssued.HasValue)
            q = q.Where(i => i.DateIssued == dateIssued.Value);

        if (invoiceId.HasValue)
            q = q.Where(i => i.InvoiceId == invoiceId.Value);

        return q
            .OrderByDescending(i => i.DateIssued)
            .ThenByDescending(i => i.InvoiceId)
            .ToListAsync(ct);
    }
}