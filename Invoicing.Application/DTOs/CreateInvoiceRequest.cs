using System.ComponentModel.DataAnnotations;

namespace Invoicing.Application.DTOs;

/// <summary>
/// Body for POST /invoice. The database generates the InvoiceId (identity)
/// </summary>
public sealed class CreateInvoiceRequest
{
    [Required]
    public DateOnly DateIssued { get; set; }

    [Range(0, double.MaxValue)]
    public decimal NetAmount { get; set; }

    [Range(0, double.MaxValue)]
    public decimal VatAmount { get; set; }

    [MaxLength(2000)]
    public string? Description { get; set; }

    [Required]
    public int CounterpartyCompanyId { get; set; }
}
