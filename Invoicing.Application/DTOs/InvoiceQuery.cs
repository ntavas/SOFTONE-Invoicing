namespace Invoicing.Application.DTOs;

/// <summary>Optional filters.</summary>
public sealed class InvoiceQuery
{
    public int? CounterpartyCompanyId { get; init; }
    public DateOnly? DateIssued { get; init; }
    public int? InvoiceId { get; init; }  // filter by generated id if provided
}