using FluentAssertions;
using Invoicing.Application.Repositories;
using Invoicing.Application.Services;
using Invoicing.Domain.Entities;
using Invoicing.Tests.Shared;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Invoicing.Tests.Unit.Services;

public class InvoiceServiceTests
{
    private readonly IInvoiceRepository _invoices = Substitute.For<IInvoiceRepository>();
    private readonly ICompanyRepository _companies = Substitute.For<ICompanyRepository>();
    private readonly ILogger<InvoiceService> _log = Substitute.For<ILogger<InvoiceService>>();
    private readonly InvoiceService _invoiceService;

    public InvoiceServiceTests()
    {
        _invoiceService = new InvoiceService(_invoices, _companies, _log);
    }

    [Fact]
    public async Task CreateAsync_returns_validation_errors_when_input_invalid()
    {
        var req = Fakes.NewCreateInvoice(counterpartyCompanyId: 1, net: -1, vat: -2);

        var result = await _invoiceService.CreateAsync(issuerCompanyId: 1, req);

        result.IsSuccess.Should().BeFalse(); 
        result.Errors.Should().HaveCount(3);
        result.Errors.Should().Contain(e => e.Code == "validation" && e.Field == "NetAmount");
        result.Errors.Should().Contain(e => e.Code == "validation" && e.Field == "VatAmount");
        result.Errors.Should().Contain(e => e.Code == "validation" && e.Field == "CounterpartyCompanyId");
    }

    [Fact]
    public async Task CreateAsync_returns_notfound_when_issuer_missing()
    {
        _companies.ExistsAsync(1, default).Returns(false);

        var req = Fakes.NewCreateInvoice(counterpartyCompanyId: 2);
        var result = await _invoiceService.CreateAsync(issuerCompanyId: 1, req);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Code == "company");
        result.Errors[0].Message.Should().Be("Authenticated company does not exist.");
    }

    [Fact]
    public async Task CreateAsync_returns_notfound_when_counterparty_missing_or_inactive()
    {
        _companies.ExistsAsync(1, default).Returns(true);
        // Validator uses GetByIdActiveAsync and returns null for not found OR inactive:
        _companies.GetByIdActiveAsync(2, default).Returns((Company?)null);

        var req = Fakes.NewCreateInvoice(counterpartyCompanyId: 2);
        var result = await _invoiceService.CreateAsync(issuerCompanyId: 1, req);
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Code == "company");
        result.Errors[0].Message.Should().Be("Counterparty company not found.");
    }

    [Fact]
    public async Task CreateAsync_persists_and_returns_dto_on_success()
    {
        _companies.ExistsAsync(1, default).Returns(true);
        _companies.GetByIdActiveAsync(2, default)
            .Returns(Fakes.ActiveCompany(2));
        _invoices.AddAsync(Arg.Any<Invoice>(), default)
            .Returns(ci =>
            {
                var inv = ci.Arg<Invoice>();
                inv.InvoiceId = 123;
                return inv;
            });

        var req = Fakes.NewCreateInvoice(counterpartyCompanyId: 2);
        var result = await _invoiceService.CreateAsync(issuerCompanyId: 1, req);

        result.IsSuccess.Should().BeTrue();
        result.Value!.InvoiceId.Should().Be(123);
    }
}