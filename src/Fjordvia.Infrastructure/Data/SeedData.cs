using Fjordvia.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace Fjordvia.Infrastructure.Data;

internal static class SeedData
{
    private static readonly Guid NorthwindPartnerId = Guid.Parse("9b7e7a42-15e4-4e67-9b3b-7b2a7cf14401");
    private static readonly Guid AuroraPartnerId = Guid.Parse("3f3395f4-4974-4d3b-8f68-2ff3bf4ea802");
    private static readonly Guid FailedLogId = Guid.Parse("8626ef4a-01ff-4a91-bf44-3ed44a597a18");

    public static void Apply(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BusinessPartner>().HasData(
            new BusinessPartner
            {
                Id = NorthwindPartnerId,
                Name = "Northwind Timber AB",
                OrganizationNumber = "559100-1010",
                Email = "finance@northwind-timber.example",
                CountryCode = "SE",
                CreatedAt = DateTimeOffset.Parse("2026-01-15T08:00:00Z")
            },
            new BusinessPartner
            {
                Id = AuroraPartnerId,
                Name = "Aurora Components AS",
                OrganizationNumber = "918273645",
                Email = "accounts@aurora-components.example",
                CountryCode = "NO",
                CreatedAt = DateTimeOffset.Parse("2026-01-16T08:00:00Z")
            });

        modelBuilder.Entity<IntegrationLog>().HasData(new IntegrationLog
        {
            Id = FailedLogId,
            Type = IntegrationType.InvoiceImport,
            Status = IntegrationStatus.Failed,
            SourceSystem = "ERP",
            TargetSystem = "Accounting",
            Reference = "ERP-INV-9001",
            Message = "Sample failed invoice import for retry testing.",
            RetryCount = 0,
            CreatedAt = DateTimeOffset.Parse("2026-02-01T10:00:00Z")
        });
    }
}
