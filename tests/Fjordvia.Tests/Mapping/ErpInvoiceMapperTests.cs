using Fjordvia.Core.Domain;
using Fjordvia.Core.Exceptions;
using Fjordvia.Core.Integrations;
using Fjordvia.Core.Mapping;

namespace Fjordvia.Tests.Mapping;

public sealed class ErpInvoiceMapperTests
{
    private readonly ErpInvoiceMapper _mapper = new();

    [Fact]
    public void Map_CreatesInvoiceAndAccountingAndCrmPayloads()
    {
        var partner = new BusinessPartner
        {
            Id = Guid.NewGuid(),
            Name = "Northwind Timber AB",
            OrganizationNumber = "559100-1010",
            Email = "finance@northwind-timber.example",
            CountryCode = "SE"
        };
        var import = new ErpInvoiceImport(
            " ERP-INV-1001 ",
            partner.Name,
            partner.OrganizationNumber,
            partner.Email,
            partner.CountryCode,
            "sek",
            new DateOnly(2026, 3, 1),
            new DateOnly(2026, 3, 31),
            [
                new ErpInvoiceLineImport("Integration monitoring", 2, 1200m),
                new ErpInvoiceLineImport("Retry handling setup", 1, 800m)
            ]);

        var result = _mapper.Map(import, partner);

        Assert.Equal("ERP-INV-1001", result.Invoice.ExternalInvoiceNumber);
        Assert.Equal("SEK", result.Invoice.Currency);
        Assert.Equal(3200m, result.Invoice.TotalAmount);
        Assert.Equal(partner.OrganizationNumber, result.AccountingInvoice.CustomerAccount);
        Assert.Equal(3200m, result.AccountingInvoice.NetAmount);
        Assert.Equal("InvoiceImported", result.CrmActivity.ActivityType);
        Assert.Equal(partner.Email, result.CrmActivity.CustomerEmail);
    }

    [Fact]
    public void Map_RejectsInvalidInvoiceLine()
    {
        var partner = new BusinessPartner
        {
            Name = "Aurora Components AS",
            OrganizationNumber = "918273645",
            Email = "accounts@aurora-components.example",
            CountryCode = "NO"
        };
        var import = new ErpInvoiceImport(
            "ERP-INV-1002",
            partner.Name,
            partner.OrganizationNumber,
            partner.Email,
            partner.CountryCode,
            "NOK",
            new DateOnly(2026, 4, 1),
            new DateOnly(2026, 4, 15),
            [new ErpInvoiceLineImport("", 0, -10m)]);

        Assert.Throws<DomainValidationException>(() => _mapper.Map(import, partner));
    }
}
