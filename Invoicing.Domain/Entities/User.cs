namespace Invoicing.Domain.Entities;

public class User
{
    public int UserId { get; set; }
    public int CompanyId { get; set; }
    public string? DisplayName { get; set; }
    public string? Email { get; set; }
    public bool IsActive { get; set; } = true;

    public Company? Company { get; set; }
}