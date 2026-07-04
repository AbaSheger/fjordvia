namespace Fjordvia.Core.Domain;

public sealed class Invoice
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string ExternalInvoiceNumber { get; set; } = string.Empty;
    public Guid BusinessPartnerId { get; set; }
    public BusinessPartner? BusinessPartner { get; set; }
    public string Currency { get; set; } = "EUR";
    public DateOnly InvoiceDate { get; set; }
    public DateOnly DueDate { get; set; }
    public decimal TotalAmount { get; set; }
    public List<InvoiceLine> Lines { get; set; } = [];
}
