using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using Invoicing.Application.DTOs;
using Invoicing.Domain.Common;
using Invoicing.Tests.Shared;
using Xunit;

namespace Invoicing.Tests.Integration.Api;

public class InvoicesControllerIntegrationTests : IClassFixture<PostgresFixture>
{
    private readonly ApiFactory _factory;
    
    private const int _counterPartyId = 2;

    public InvoicesControllerIntegrationTests(PostgresFixture pg)
    {
        _factory = new ApiFactory(pg.ConnectionString);
    }

    [Fact]
    public async Task Post_invoice_requires_bearer_token()
    {
        var client = _factory.CreateClient();

        var body = new CreateInvoiceRequest
        {
            CounterpartyCompanyId = _counterPartyId,
            DateIssued = new DateOnly(2025, 08, 26),
            NetAmount = 10, VatAmount = 2
        };

        var resp = await client.PostAsJsonAsync("api/invoice", body);
        resp.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        var envelope = await resp.Content.ReadFromJsonAsync<ApiEnvelope>();
        envelope!.Success.Should().BeFalse();
        envelope.Errors![0].Code.Should().Be(ErrorCatalog.Unauthorized);
        envelope.Errors![0].Message.Should().Be("Missing Authorization header.");
    }

    [Fact]
    public async Task Post_invoice_happy_path_returns_ok_and_body()
    {
        var client = _factory.CreateClient().WithBearer(TestConstants.SeedTokenCompany1);

        var body = new CreateInvoiceRequest
        {
            // issuer is company 1 (from token)
            CounterpartyCompanyId = _counterPartyId,
            DateIssued = new DateOnly(2025, 08, 26),
            NetAmount = 100, 
            VatAmount = 24,
            Description = "integration test"
        };

        var resp = await client.PostAsJsonAsync("api/invoice/createInvoice", body);
        resp.StatusCode.Should().Be(HttpStatusCode.OK);

        var envelope = await resp.Content.ReadFromJsonAsync<ApiEnvelope<InvoiceDto>>();
        envelope!.Success.Should().BeTrue();
        envelope.Data!.CompanyId.Should().Be(1);
        envelope.Data!.TotalAmount.Should().Be(body.NetAmount + body.VatAmount);
        envelope.Data.CounterpartyCompanyId.Should().Be(2);
        envelope.Data.InvoiceId.Should().NotBe(null);
        envelope.Data.InvoiceId.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task Get_sent_returns_data_for_authenticated_company()
    {
        var client = _factory.CreateClient().WithBearer(TestConstants.SeedTokenCompany1);

        var resp = await client.GetAsync("api/invoice/sent");
        resp.StatusCode.Should().Be(HttpStatusCode.OK);

        var envelope = await resp.Content.ReadFromJsonAsync<ApiEnvelope<List<InvoiceDto>>>();
        envelope!.Success.Should().BeTrue();
        envelope.Data.Should().NotBeNull();
        envelope.Data!.Count.Should().BeGreaterThan(0);
    }
    
    [Fact]
    public async Task Get_received_returns_data_for_authenticated_company()
    {
        var issuerClient = _factory.CreateClient().WithBearer(TestConstants.SeedTokenCompany2);

        var createBody = new CreateInvoiceRequest
        {
            CounterpartyCompanyId = 1,
            DateIssued = new DateOnly(2025, 08, 27),
            NetAmount = 50, VatAmount = 12,
            Description = "received-integration-test"
        };

        var createResp = await issuerClient.PostAsJsonAsync("api/invoice/createInvoice", createBody);
        createResp.StatusCode.Should().Be(HttpStatusCode.OK);

        var created = await createResp.Content.ReadFromJsonAsync<ApiEnvelope<InvoiceDto>>();
        created!.Success.Should().BeTrue();
        var newId = created.Data!.InvoiceId;

        // 2) As company 1, fetch received invoices
        var receiverClient = _factory.CreateClient().WithBearer(TestConstants.SeedTokenCompany1);
        var getResp = await receiverClient.GetAsync($"api/invoice/received?invoiceId={newId}");
        getResp.StatusCode.Should().Be(HttpStatusCode.OK);

        var envelope = await getResp.Content.ReadFromJsonAsync<ApiEnvelope<List<InvoiceDto>>>();
        envelope!.Success.Should().BeTrue();
        envelope.Data.Should().NotBeNull().And.HaveCountGreaterOrEqualTo(1);
        envelope.Data!.Should().Contain(i =>
            i.InvoiceId == newId &&
            i.CompanyId == 2 &&
            i.CounterpartyCompanyId == 1 &&
            i.Description == "received-integration-test");
    }
    
    [Fact]
    public async Task Users_endpoint_bypasses_auth_middleware_when_path_not_invoice()
    {
        // No Authorization header, middleware should skip because path != /api/invoice*
        var client = _factory.CreateClient();
        var resp = await client.GetAsync("/api/users/getAllUsers");

        resp.StatusCode.Should().Be(HttpStatusCode.OK);

        var envelope = await resp.Content.ReadFromJsonAsync<ApiEnvelope<List<UserDto>>>();
        envelope!.Success.Should().BeTrue();
        envelope.Data.Should().NotBeNull();
        envelope.Data!.Count.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task Invoice_post_rejects_non_bearer_authorization_scheme()
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", "abc123");

        var body = new CreateInvoiceRequest
        {
            CounterpartyCompanyId = 2,
            DateIssued = new DateOnly(2025, 08, 27),
            NetAmount = 10, VatAmount = 2
        };

        var resp = await client.PostAsJsonAsync("/api/invoice", body);
        resp.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        var envelope = await resp.Content.ReadFromJsonAsync<ApiEnvelope>();
        envelope!.Success.Should().BeFalse();
        envelope.Errors![0].Code.Should().Be(ErrorCatalog.Unauthorized);
        envelope.Errors![0].Message.Should().Contain("must be 'Bearer <token>'");
    }

    [Fact]
    public async Task Invoice_post_rejects_empty_bearer_token()
    {
        var client = _factory.CreateClient();
        // empty Bearer token
        client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Bearer ");

        var body = new CreateInvoiceRequest
        {
            CounterpartyCompanyId = 2,
            DateIssued = new DateOnly(2025, 08, 27),
            NetAmount = 10, VatAmount = 2
        };

        var resp = await client.PostAsJsonAsync("/api/invoice", body);
        resp.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        var envelope = await resp.Content.ReadFromJsonAsync<ApiEnvelope>();
        envelope!.Success.Should().BeFalse();
        envelope.Errors![0].Code.Should().Be(ErrorCatalog.Unauthorized);
        envelope.Errors![0].Message.Should().Contain("Authorization header must be 'Bearer <token>");
    }

    [Fact]
    public async Task Invoice_post_rejects_invalid_token()
    {
        var client = _factory.CreateClient().WithBearer("totally_invalid_token");

        var body = new CreateInvoiceRequest
        {
            CounterpartyCompanyId = 2,
            DateIssued = new DateOnly(2025, 08, 27),
            NetAmount = 10, VatAmount = 2
        };

        var resp = await client.PostAsJsonAsync("/api/invoice", body);
        resp.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        var envelope = await resp.Content.ReadFromJsonAsync<ApiEnvelope>();
        envelope!.Success.Should().BeFalse();
        envelope.Errors![0].Code.Should().Be(ErrorCatalog.Unauthorized);
        envelope.Errors![0].Message.Should().Contain("Invalid token");
    }    

    // envelope mirror for deserialization
    private sealed record ApiEnvelope(bool Success, string? TraceId, List<ApiError>? Errors = null);
    private sealed record ApiEnvelope<T>(bool Success, string? TraceId, T? Data, List<ApiError>? Errors = null);
    private sealed record ApiError(string Code, string Message, string? Field = null);
}