using Fjordvia.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace Fjordvia.Infrastructure.Data;

public sealed class FjordviaDbContext(DbContextOptions<FjordviaDbContext> options) : DbContext(options)
{
    public DbSet<BusinessPartner> BusinessPartners => Set<BusinessPartner>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<InvoiceLine> InvoiceLines => Set<InvoiceLine>();
    public DbSet<IntegrationLog> IntegrationLogs => Set<IntegrationLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BusinessPartner>(entity =>
        {
            entity.HasKey(partner => partner.Id);
            entity.Property(partner => partner.Name).HasMaxLength(160).IsRequired();
            entity.Property(partner => partner.OrganizationNumber).HasMaxLength(40).IsRequired();
            entity.Property(partner => partner.Email).HasMaxLength(160).IsRequired();
            entity.Property(partner => partner.CountryCode).HasMaxLength(2).IsRequired();
            entity.HasIndex(partner => partner.OrganizationNumber).IsUnique();
        });

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasKey(invoice => invoice.Id);
            entity.Property(invoice => invoice.ExternalInvoiceNumber).HasMaxLength(80).IsRequired();
            entity.Property(invoice => invoice.Currency).HasMaxLength(3).IsRequired();
            entity.Property(invoice => invoice.TotalAmount).HasPrecision(18, 2);
            entity.HasOne(invoice => invoice.BusinessPartner)
                .WithMany()
                .HasForeignKey(invoice => invoice.BusinessPartnerId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasMany(invoice => invoice.Lines)
                .WithOne()
                .HasForeignKey(line => line.InvoiceId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<InvoiceLine>(entity =>
        {
            entity.HasKey(line => line.Id);
            entity.Property(line => line.Description).HasMaxLength(240).IsRequired();
            entity.Property(line => line.UnitPrice).HasPrecision(18, 2);
        });

        modelBuilder.Entity<IntegrationLog>(entity =>
        {
            entity.HasKey(log => log.Id);
            entity.Property(log => log.SourceSystem).HasMaxLength(80).IsRequired();
            entity.Property(log => log.TargetSystem).HasMaxLength(80).IsRequired();
            entity.Property(log => log.Reference).HasMaxLength(120).IsRequired();
            entity.Property(log => log.Message).HasMaxLength(500);
            entity.Property(log => log.Type).HasConversion<string>().HasMaxLength(80);
            entity.Property(log => log.Status).HasConversion<string>().HasMaxLength(40);
        });

        SeedData.Apply(modelBuilder);
    }
}
