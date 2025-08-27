using FluentAssertions;
using Invoicing.Application.Validators;
using Invoicing.Domain.Common;
using Invoicing.Tests.Shared;
using Xunit;

namespace Invoicing.Tests.Unit.Validators;

public class InvoiceValidatorTests
{
    [Fact]
    public void ValidateCreateInput_returns_all_field_errors()
    {
        // issuer == counterparty and negative amounts
        var req = Fakes.NewCreateInvoice(counterpartyCompanyId: 1, net: -1, vat: -2);
        var errors = InvoiceValidator.ValidateCreateInput(req, issuerCompanyId: 1);

        errors.Should().NotBeEmpty();
        errors.Should().Contain(e => e.Code == ErrorCatalog.Validation && e.Field == nameof(req.NetAmount));
        errors.Should().Contain(e => e.Code == ErrorCatalog.Validation && e.Field == nameof(req.VatAmount));
        errors.Should().Contain(e => e.Code == ErrorCatalog.Validation && e.Field == nameof(req.CounterpartyCompanyId));
    }
}