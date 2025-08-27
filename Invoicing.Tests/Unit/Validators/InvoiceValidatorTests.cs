using FluentAssertions;
using Invoicing.Application.Validators;
using Invoicing.Domain.Common;
using Invoicing.Tests.Shared;
using Xunit;

namespace Invoicing.Tests.Unit.Validators;

public class InvoiceValidatorTests
{
    private static readonly decimal Max18_2 = 9_999_999_999_999_999.99m;
    
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
    
    [Fact]
    public void ValidateCreateInput_returns_error_exceed_decimal_on_total_amount()
    {
        // issuer == counterparty and negative amounts
        var req = Fakes.NewCreateInvoice(counterpartyCompanyId: 1, net: Max18_2, vat: Max18_2);
        var errors = InvoiceValidator.ValidateCreateInput(req, issuerCompanyId: 1);

        errors.Should().NotBeEmpty();
        errors.Should().Contain(e => e.Code == ErrorCatalog.Validation && e.Field == "TotalAmount");
        errors.Should().Contain(e => e.Code == ErrorCatalog.Validation && e.Field == nameof(req.CounterpartyCompanyId));
    }
}