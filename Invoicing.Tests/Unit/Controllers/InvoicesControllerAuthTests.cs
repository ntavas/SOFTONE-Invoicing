using FluentAssertions;
using Invoicing.Api.Controllers;
using Invoicing.Api.Responses;
using Invoicing.Application.DTOs;
using Invoicing.Application.Services.Interfaces;
using Invoicing.Domain.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Invoicing.Tests.Unit.Controllers;

public class InvoicesControllerAuthTests
{
    private readonly InvoicesControllerIntegration _controllerIntegration;

    public InvoicesControllerAuthTests()
    {
        var svc = Substitute.For<IInvoiceService>();
        var log = Substitute.For<ILogger<InvoicesControllerIntegration>>();
        _controllerIntegration = new InvoicesControllerIntegration(svc, log)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };
    }

    [Fact]
    public async Task Create_returns_401_and_envelope_when_missing_company_context()
    {
        var body = new CreateInvoiceRequest
        {
            CounterpartyCompanyId = 123,
            NetAmount = 10,
            VatAmount = 2,
            DateIssued = new DateOnly(2025, 08, 27),
            Description = "test"
        };

        var result = await _controllerIntegration.Create(body, CancellationToken.None);

        var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);
        var envelope = Assert.IsType<ApiResponse<InvoiceDto>>(unauthorized.Value);

        envelope.Success.Should().BeFalse();
        envelope.Errors.Should().NotBeNull();
        envelope.Errors![0].Code.Should().Be(ErrorCatalog.Unauthorized);
    }

    [Fact]
    public async Task GetSent_returns_401_and_envelope_when_missing_company_context()
    {
        var result = await _controllerIntegration.GetSent(
            counterpartyCompanyId: null,
            dateIssued: null,
            invoiceId: null,
            ct: CancellationToken.None);

        var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);
        var envelope = Assert.IsType<ApiResponse<IReadOnlyList<InvoiceDto>>>(unauthorized.Value);

        envelope.Success.Should().BeFalse();
        envelope.Errors.Should().NotBeNull();
        envelope.Errors![0].Code.Should().Be(ErrorCatalog.Unauthorized);
    }

    [Fact]
    public async Task GetReceived_returns_401_and_envelope_when_missing_company_context()
    {
        var result = await _controllerIntegration.GetReceived(
            counterpartyCompanyId: null,
            dateIssued: null,
            invoiceId: null,
            ct: CancellationToken.None);

        var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);
        var envelope = Assert.IsType<ApiResponse<IReadOnlyList<InvoiceDto>>>(unauthorized.Value);

        envelope.Success.Should().BeFalse();
        envelope.Errors.Should().NotBeNull();
        envelope.Errors![0].Code.Should().Be(ErrorCatalog.Unauthorized);
    }
}