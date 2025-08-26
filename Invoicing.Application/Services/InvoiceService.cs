using Invoicing.Application.DTOs;
using Invoicing.Application.Mappings;
using Invoicing.Application.Repositories;
using Invoicing.Application.Validators;
using Invoicing.Domain.Common;
using Invoicing.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Invoicing.Application.Services;

/// <summary>
/// Orchestrates invoice workflows. This layer owns validations that are not just DB constraints
/// (e.g., amounts non-negative, issuer != counterparty, companies exist/active).
/// </summary>
public sealed class InvoiceService : Interfaces.IInvoiceService
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly ICompanyRepository _companyRepository;
    private readonly ILogger<InvoiceService> _logger;
    
    public InvoiceService(
        IInvoiceRepository invoiceRepository,
        ICompanyRepository companyRepository,
        ILogger<InvoiceService> logger)
    {
        _invoiceRepository = invoiceRepository;
        _companyRepository = companyRepository;
        _logger = logger;
    }
    
    
    public async Task<Result<InvoiceDto>> CreateAsync(
        int issuerCompanyId,
        CreateInvoiceRequest request,
        CancellationToken ct = default)
    {
        _logger.LogDebug("Create invoice start: issuer={Issuer}, counterparty={Counterparty}, net={Net}, vat={Vat}, date={Date}",
            issuerCompanyId, request.CounterpartyCompanyId, request.NetAmount, request.VatAmount, request.DateIssued);
        
        var errors = InvoiceValidator.ValidateCreateInput(request, issuerCompanyId);
        
        if (errors.Count > 0)
        {
            _logger.LogInformation("Create invoice rejected (input validation): {Count} error(s)", errors.Count);
            return Result<InvoiceDto>.Failure(errors);
        }
        
        var dbErrors = await InvoiceValidator.ValidateCreateDbAsync(
            _companyRepository, issuerCompanyId, request.CounterpartyCompanyId, ct);

        if (dbErrors.Count > 0)
        {
            _logger.LogInformation("Create invoice rejected (DB validation): {Count} error(s)", dbErrors.Count);
            return Result<InvoiceDto>.Failure(dbErrors);
        }
        
        
        var entity = request.ToEntity(issuerCompanyId);
        entity = await _invoiceRepository.AddAsync(entity, ct);

        _logger.LogInformation("Invoice created: issuer={Issuer} -> counterparty={Counterparty}",
            entity.CompanyId, entity.CounterpartyCompanyId);

        return Result<InvoiceDto>.Success(entity.ToDto());
    }

    public async Task<Result<IReadOnlyList<InvoiceDto>>> GetInvoicesSentAsync(int companyId, InvoiceQuery query, CancellationToken ct = default)
    {
        _logger.LogDebug("List sent invoices: companyId={CompanyId}, cp={Cp}, date={Date}, invoiceId={InvoiceId}",
            companyId, query.CounterpartyCompanyId, query.DateIssued, query.InvoiceId);

        var list = await _invoiceRepository.GetInvoicesSentAsync(
            companyId,
            query.CounterpartyCompanyId,
            query.DateIssued,
            query.InvoiceId,
            ct);

        var dtos = list.Select(inv => inv.ToDto()).ToList();
        _logger.LogInformation("Found {Count} sent invoices for companyId={CompanyId}", dtos.Count, companyId);
        return Result<IReadOnlyList<InvoiceDto>>.Success(dtos);
    }

    public async Task<Result<IReadOnlyList<InvoiceDto>>> GetInvoicesReceivedAsync(
        int companyId, InvoiceQuery query, CancellationToken ct = default)
    {
        _logger.LogDebug("List received invoices: companyId={CompanyId}, cp={Cp}, date={Date}, invoiceId={InvoiceId}",
            companyId, query.CounterpartyCompanyId, query.DateIssued, query.InvoiceId);

        var list = await _invoiceRepository.GetInvoicesReceivedAsync(
            companyId,
            query.CounterpartyCompanyId,
            query.DateIssued,
            query.InvoiceId,
            ct);

        var dtos = list.Select(inv => inv.ToDto()).ToList();
        _logger.LogInformation("Found {Count} received invoices for companyId={CompanyId}", dtos.Count, companyId);
        return Result<IReadOnlyList<InvoiceDto>>.Success(dtos);
    }
}