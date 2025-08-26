using Invoicing.Domain.Entities;

namespace Invoicing.Application.Repositories;

/// <summary>
/// Invoice persistence + query operations.
/// </summary>
public interface IInvoiceRepository
{
    Task<Invoice> AddAsync(Invoice entity, CancellationToken ct = default);

    Task<List<Invoice>> GetInvoicesSentAsync(int companyId, int? counterpartyCompanyId, DateOnly? dateIssued, int? invoiceId, CancellationToken ct = default);

    Task<List<Invoice>> GetInvoicesReceivedAsync(int companyId, int? counterpartyCompanyId, DateOnly? dateIssued, int? invoiceId, CancellationToken ct = default);
}