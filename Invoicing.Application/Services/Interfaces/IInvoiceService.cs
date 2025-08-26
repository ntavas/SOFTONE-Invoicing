using Invoicing.Application.DTOs;
using Invoicing.Domain.Common;

namespace Invoicing.Application.Services.Interfaces;

/// <summary>
/// Use-cases around invoices. These methods do input/business checks,
/// call repositories, and return a Result so controllers can map to HTTP.
/// </summary>
public interface IInvoiceService
{
    /// <summary>
    /// Create a new invoice issued by the authenticated company.
    /// The DB assigns InvoiceId (identity).
    /// </summary>
    Task<Result<InvoiceDto>> CreateAsync(int issuerCompanyId, CreateInvoiceRequest request, CancellationToken ct = default);

    /// <summary>Invoices this company SENT (issuer = companyId), with optional filters.</summary>
    Task<Result<IReadOnlyList<InvoiceDto>>> GetInvoicesSentAsync(int companyId, InvoiceQuery query, CancellationToken ct = default);

    /// <summary>Invoices this company RECEIVED (counterparty = companyId), with optional filters.</summary>
    Task<Result<IReadOnlyList<InvoiceDto>>> GetInvoicesReceivedAsync(int companyId, InvoiceQuery query, CancellationToken ct = default);
}