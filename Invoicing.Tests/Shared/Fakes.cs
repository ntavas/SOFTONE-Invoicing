using Invoicing.Application.DTOs;
using Invoicing.Domain.Entities;

namespace Invoicing.Tests.Shared;

public static class Fakes
{
    public static CreateInvoiceRequest NewCreateInvoice(
        int counterpartyCompanyId = 2,
        decimal net = 100m,
        decimal vat = 24m,
        DateOnly? date = null,
        string? desc = "test")
        => new()
        {
            CounterpartyCompanyId = counterpartyCompanyId,
            NetAmount = net,
            VatAmount = vat,
            DateIssued = date ?? DateOnly.FromDateTime(DateTime.UtcNow.Date),
            Description = desc
        };

    public static Company ActiveCompany(int id) => new() { CompanyId = id, Name = $"C{id}", IsActive = true };
}