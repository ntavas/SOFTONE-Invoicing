namespace Invoicing.Domain.Entities;

public class Company
{
    public Guid CompanyId { get; set; }
    public string Name { get; set; } = null!;
    public byte[]? ApiTokenHash { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<User> Users { get; set; } = new List<User>();
}