using Invoicing.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Invoicing.Infrastructure.Persistence;

public sealed class InvoicingDbContext(DbContextOptions<InvoicingDbContext> options) : DbContext(options)
{
    public DbSet<Company> Companies => Set<Company>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Invoice> Invoices => Set<Invoice>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // company entity
        modelBuilder.Entity<Company>(e =>
        {
            e.ToTable("company");
            e.HasKey(x => x.CompanyId).HasName("pk_company");
            
            e.Property(c => c.CompanyId)
                .HasColumnName("company_id")
                .ValueGeneratedOnAdd()
                .UseIdentityByDefaultColumn();
            
            e.Property(x => x.Name).IsRequired().HasMaxLength(200);
            e.Property(x => x.ApiTokenHash).HasColumnType("bytea");
            e.Property(x => x.IsActive).HasDefaultValue(true);
            
            e.HasIndex(x => x.Name).IsUnique().HasDatabaseName("uq_company_name");
            e.HasIndex(x => x.ApiTokenHash)
             .IsUnique()
             .HasFilter("api_token_hash IS NOT NULL")
             .HasDatabaseName("uq_company_api_token_hash");
        });

        // users entity
        modelBuilder.Entity<User>(e =>
        {
            e.ToTable("users");
            e.HasKey(x => x.UserId).HasName("pk_user");
            
            e.Property(u => u.UserId)
                .HasColumnName("user_id")
                .ValueGeneratedOnAdd()                 
                .UseIdentityByDefaultColumn();
            
            e.Property(x => x.DisplayName).HasMaxLength(200);
            e.Property(x => x.Email).HasMaxLength(320);
            e.Property(x => x.IsActive).HasDefaultValue(true);

            e.HasOne(x => x.Company)
             .WithMany(c => c.Users)
             .HasForeignKey(x => x.CompanyId)
             .OnDelete(DeleteBehavior.Cascade)
             .HasConstraintName("users_company_id_fk");

            e.HasIndex(x => x.CompanyId).HasDatabaseName("ix_user_company_id");
        });

        // invoice entity
        modelBuilder.Entity<Invoice>(e =>
        {
            e.ToTable("invoice");
            e.Property(i => i.InvoiceId)
                .HasColumnName("invoice_id")
                .ValueGeneratedOnAdd()                 
                .UseIdentityByDefaultColumn(); 

            e.Property(x => x.InvoiceId).ValueGeneratedOnAdd();
            e.Property(x => x.DateIssued);
            e.Property(x => x.NetAmount).HasColumnType("numeric(18,2)");
            e.Property(x => x.VatAmount).HasColumnType("numeric(18,2)");
            e.Property(x => x.Description).HasMaxLength(1000);

            e.HasOne<Company>().WithMany()
             .HasForeignKey(x => x.CompanyId)
             .OnDelete(DeleteBehavior.Restrict);

            e.HasOne<Company>().WithMany()
             .HasForeignKey(x => x.CounterpartyCompanyId)
             .OnDelete(DeleteBehavior.Restrict);

            e.HasIndex(x => new { x.CompanyId, x.CounterpartyCompanyId, x.DateIssued })
             .HasDatabaseName("ix_invoice_sent");
            e.HasIndex(x => new { x.CounterpartyCompanyId, x.CompanyId, x.DateIssued })
             .HasDatabaseName("ix_invoice_received");
            e.HasIndex(x => x.DateIssued).HasDatabaseName("ix_invoice_date_issued");
        });
    }
}
