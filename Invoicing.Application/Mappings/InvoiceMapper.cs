using Invoicing.Application.DTOs;
using Invoicing.Domain.Entities;

namespace Invoicing.Application.Mappings;

/// <summary>
/// Converts invoices to DTOs, and builds a new Invoice from a POST request.
/// </summary>
public static class InvoiceMapper
{
    public static InvoiceDto ToDto(this Invoice x) =>
        new(
            x.InvoiceId,
            x.CompanyId,
            x.CounterpartyCompanyId,
            x.DateIssued,
            x.NetAmount,
            x.VatAmount,
            x.Description
        );

    /// <summary>
    /// Create a domain entity from the API request.
    /// </summary>
    public static Invoice ToEntity(this CreateInvoiceRequest req, int issuerCompanyId) =>
        new()
        {
            CompanyId = issuerCompanyId,
            CounterpartyCompanyId = req.CounterpartyCompanyId,
            DateIssued = req.DateIssued,
            NetAmount = req.NetAmount,
            VatAmount = req.VatAmount,
            Description = req.Description
        };
}