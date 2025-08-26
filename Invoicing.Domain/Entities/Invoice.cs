namespace Invoicing.Domain.Entities;

public class Invoice
{
    public string InvoiceId { get; set; } = null!;
    public DateOnly DateIssued { get; set; }
    public decimal NetAmount { get; set; }
    public decimal VatAmount { get; set; }
    public string? Description { get; set; }

    public Guid CompanyId { get; set; }
    public Guid CounterpartyCompanyId { get; set; }
}