namespace Invoicing.Domain.Entities;

public class Invoice
{
    public int InvoiceId { get; set; }
    public DateOnly DateIssued { get; set; }
    public decimal NetAmount { get; set; }
    public decimal VatAmount { get; set; }
    public string? Description { get; set; }

    public int CompanyId { get; set; }
    public int CounterpartyCompanyId { get; set; }
}