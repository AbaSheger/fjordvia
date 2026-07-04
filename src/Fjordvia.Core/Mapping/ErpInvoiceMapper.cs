using Fjordvia.Core.Domain;
using Fjordvia.Core.Exceptions;
using Fjordvia.Core.Integrations;

namespace Fjordvia.Core.Mapping;

public sealed class ErpInvoiceMapper
{
    public ErpInvoiceMappingResult Map(ErpInvoiceImport import, BusinessPartner partner)
    {
        Validate(import, partner);

        var invoice = new Invoice
        {
            ExternalInvoiceNumber = import.ExternalInvoiceNumber.Trim(),
            BusinessPartnerId = partner.Id,
            BusinessPartner = partner,
            Currency = import.Currency.Trim().ToUpperInvariant(),
            InvoiceDate = import.InvoiceDate,
            DueDate = import.DueDate,
            Lines = import.Lines.Select(line => new InvoiceLine
            {
                Description = line.Description.Trim(),
                Quantity = line.Quantity,
                UnitPrice = line.UnitPrice
            }).ToList()
        };

        invoice.TotalAmount = invoice.Lines.Sum(line => line.LineTotal);

        var accountingInvoice = new AccountingInvoiceExport(
            invoice.ExternalInvoiceNumber,
            partner.OrganizationNumber,
            invoice.Currency,
            invoice.TotalAmount,
            invoice.InvoiceDate,
            invoice.DueDate);

        var crmActivity = new CrmInvoiceActivity(
            partner.Name,
            partner.Email,
            "InvoiceImported",
            $"Invoice {invoice.ExternalInvoiceNumber} imported from ERP.",
            invoice.TotalAmount,
            invoice.InvoiceDate);

        return new ErpInvoiceMappingResult(invoice, accountingInvoice, crmActivity);
    }

    private static void Validate(ErpInvoiceImport import, BusinessPartner partner)
    {
        if (string.IsNullOrWhiteSpace(import.ExternalInvoiceNumber))
        {
            throw new DomainValidationException("External invoice number is required.");
        }

        if (string.IsNullOrWhiteSpace(import.Currency) || import.Currency.Trim().Length != 3)
        {
            throw new DomainValidationException("Currency must be a three-letter ISO code.");
        }

        if (import.DueDate < import.InvoiceDate)
        {
            throw new DomainValidationException("Due date cannot be earlier than invoice date.");
        }

        if (import.Lines.Count == 0)
        {
            throw new DomainValidationException("At least one invoice line is required.");
        }

        if (import.Lines.Any(line => string.IsNullOrWhiteSpace(line.Description) || line.Quantity <= 0 || line.UnitPrice < 0))
        {
            throw new DomainValidationException("Invoice lines must include a description, positive quantity, and non-negative unit price.");
        }

        if (!string.Equals(import.CustomerOrganizationNumber.Trim(), partner.OrganizationNumber, StringComparison.OrdinalIgnoreCase))
        {
            throw new DomainValidationException("Invoice customer organization number does not match the business partner.");
        }
    }
}
