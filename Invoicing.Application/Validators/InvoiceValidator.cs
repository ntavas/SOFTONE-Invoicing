using Invoicing.Application.DTOs;
using Invoicing.Application.Repositories;
using Invoicing.Domain.Common;

namespace Invoicing.Application.Validators;

/// <summary>
/// Centralizes rules for creating an invoice so the service stays readable.<br/>
/// Split into:<br/>
///  - input checks (no DB)<br/>
///  - DB-backed checks (existence)
/// </summary>
public static class InvoiceValidator
{
    public static List<Error> ValidateCreateInput(CreateInvoiceRequest req, int issuerCompanyId)
    {
        var errors = new List<Error>();

        if (req.NetAmount < 0)
            errors.Add(Error.Validation(nameof(req.NetAmount), "Must be >= 0"));

        if (req.VatAmount < 0)
            errors.Add(Error.Validation(nameof(req.VatAmount), "Must be >= 0"));

        if (req.CounterpartyCompanyId == issuerCompanyId)
            errors.Add(Error.Validation(nameof(req.CounterpartyCompanyId),
                "Issuer and counterparty cannot be the same company."));

        return errors;
    }


    public static async Task<Error?> ValidateIssuerAsync(ICompanyRepository companies, int issuerCompanyId, CancellationToken ct)
    {
        var exists = await companies.ExistsAsync(issuerCompanyId, ct);
        return exists ? null : Error.NotFound("company", "Authenticated company does not exist.");
    }
    
    public static async Task<Error?> ValidateCounterpartyAsync(ICompanyRepository companies, int counterpartyCompanyId, CancellationToken ct)
    {
        var cp = await companies.GetByIdActiveAsync(counterpartyCompanyId, ct);
        return cp != null ? null : Error.NotFound("company", "Counterparty company not found.");
    }
}