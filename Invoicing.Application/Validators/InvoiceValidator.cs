using Invoicing.Application.DTOs;
using Invoicing.Application.Repositories;
using Invoicing.Domain.Common;

namespace Invoicing.Application.Validators;

/// <summary>
/// Centralizes rules for creating an invoice so the service stays readable.
/// Split into:
///  - input checks (no DB)
///  - DB-backed checks (existence/active flags)
/// </summary>
public static class InvoiceValidator
{
    /// <summary>
    /// Pure input/business rules that don't need the database.
    /// </summary>
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

    /// <summary>
    /// Checks that require DB lookups. Sequential on purpose.
    /// </summary>
    public static async Task<List<Error>> ValidateCreateDbAsync(ICompanyRepository companies, int issuerCompanyId, int counterpartyCompanyId, CancellationToken ct)
    {
        var errors = new List<Error>();

        // issuer must exist
        var issuerExists = await companies.ExistsAsync(issuerCompanyId, ct);
        if (!issuerExists)
            errors.Add(Error.NotFound("company", "Authenticated company does not exist."));

        // counterparty must exist and be active
        var counterparty = await companies.GetByIdActiveAsync(counterpartyCompanyId, ct);
        if (counterparty is null)
            errors.Add(Error.NotFound("company", "Counterparty company not found."));

        return errors;
    }
}