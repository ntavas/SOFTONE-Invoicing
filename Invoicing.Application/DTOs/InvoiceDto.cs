namespace Invoicing.Application.DTOs;

public sealed record InvoiceDto(
    int InvoiceId,
    int CompanyId,
    int CounterpartyCompanyId,
    DateOnly DateIssued,
    decimal NetAmount,
    decimal VatAmount,
    decimal TotalAmount,
    string? Description
);