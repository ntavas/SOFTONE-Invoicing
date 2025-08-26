using Invoicing.Api.Extensions;
using Invoicing.Api.Responses;
using Invoicing.Application.DTOs;
using Invoicing.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Invoicing.Api.Controllers;

/// <summary>
/// Controller for invoice use-cases.<br/>
/// - Create invoice (POST api/invoice).<br/>
/// - Get sent invoices (GET api/invoice/sent).<br/>
/// - Get received invoices (GET api/invoice/received).<br/>
/// </summary>
[ApiController]
[Route("api/invoice")]
public sealed class InvoicesController : ControllerBase
{
    private readonly IInvoiceService _service;
    private readonly ILogger<InvoicesController> _logger;

    public InvoicesController(IInvoiceService service, ILogger<InvoicesController> logger)
    {
        _service = service;
        _logger  = logger;
    }

    /// <summary>
    /// TEMP helper.<br/>
    /// TODO: Remove when auth middleware is in place.<br/>
    /// </summary>
    private bool TryGetAuthenticatedCompanyId(out int companyId)
    {
        if (HttpContext.Items.TryGetValue("CompanyId", out var v) && v is int idFromMiddleware)
        {
            companyId = idFromMiddleware;
            return true;
        }

        // Dev/testing only: header override. Remove when auth middleware lands.
        if (Request.Headers.TryGetValue("X-Demo-CompanyId", out var header) &&
            int.TryParse(header, out var idFromHeader))
        {
            companyId = idFromHeader;
            return true;
        }

        companyId = default;
        return false;
    }

    /// <summary>
    /// POST api/invoice/createInvoice — create a new invoice issued by the authenticated company.
    /// </summary>
    [HttpPost("createInvoice")]
    public async Task<IActionResult> Create([FromBody] CreateInvoiceRequest body, CancellationToken ct)
    {
        if (!TryGetAuthenticatedCompanyId(out var issuerCompanyId))
        {
            var resp = ApiResponse<InvoiceDto>.Fail(
                new[] { new ApiError("unauthorized", "Missing company context. Provide a valid token or X-Demo-CompanyId for local testing.") },
                HttpContext.TraceIdentifier);
            return Unauthorized(resp);
        }

        var result = await _service.CreateAsync(issuerCompanyId, body, ct);
        return this.ToActionResult(result, _logger);
    }

    /// <summary>
    /// GET api/invoice/sent — invoices this company has SENT.
    /// Optional filters: counterpartyCompanyId, dateIssued (YYYY-MM-DD), invoiceId.
    /// </summary>
    [HttpGet("sent")]
    public async Task<IActionResult> GetSent(
        [FromQuery] int? counterpartyCompanyId,
        [FromQuery] DateOnly? dateIssued,
        [FromQuery] int? invoiceId,
        CancellationToken ct = default)
    {
        if (!TryGetAuthenticatedCompanyId(out var companyId))
        {
            var resp = ApiResponse<IReadOnlyList<InvoiceDto>>.Fail(
                new[] { new ApiError("unauthorized", "Missing company context.") },
                HttpContext.TraceIdentifier);
            return Unauthorized(resp);
        }

        var query = new InvoiceQuery
        {
            CounterpartyCompanyId = counterpartyCompanyId,
            DateIssued = dateIssued,
            InvoiceId = invoiceId
        };

        var result = await _service.GetInvoicesSentAsync(companyId, query, ct);
        return this.ToActionResult(result, _logger);
    }

    /// <summary>
    /// GET /invoice/received — invoices this company has RECEIVED.
    /// Optional filters: counterpartyCompanyId, dateIssued (YYYY-MM-DD), invoiceId.
    /// </summary>
    [HttpGet("received")]
    public async Task<IActionResult> GetReceived(
        [FromQuery] int? counterpartyCompanyId,
        [FromQuery] DateOnly? dateIssued,
        [FromQuery] int? invoiceId,
        CancellationToken ct = default)
    {
        if (!TryGetAuthenticatedCompanyId(out var companyId))
        {
            var resp = ApiResponse<IReadOnlyList<InvoiceDto>>.Fail(
                new[] { new ApiError("unauthorized", "Missing company context.") },
                HttpContext.TraceIdentifier);
            return Unauthorized(resp);
        }

        var query = new InvoiceQuery
        {
            CounterpartyCompanyId = counterpartyCompanyId,
            DateIssued = dateIssued,
            InvoiceId = invoiceId
        };

        var result = await _service.GetInvoicesReceivedAsync(companyId, query, ct);
        return this.ToActionResult(result, _logger);
    }
}