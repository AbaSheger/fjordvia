using Fjordvia.Core.Domain;

namespace Fjordvia.Api.Dtos;

public sealed record InvoiceResponse(
    Guid Id,
    string ExternalInvoiceNumber,
    Guid BusinessPartnerId,
    string? BusinessPartnerName,
    string Currency,
    DateOnly InvoiceDate,
    DateOnly DueDate,
    decimal TotalAmount,
    IReadOnlyCollection<InvoiceLineResponse> Lines)
{
    public static InvoiceResponse FromDomain(Invoice invoice) =>
        new(
            invoice.Id,
            invoice.ExternalInvoiceNumber,
            invoice.BusinessPartnerId,
            invoice.BusinessPartner?.Name,
            invoice.Currency,
            invoice.InvoiceDate,
            invoice.DueDate,
            invoice.TotalAmount,
            invoice.Lines.Select(InvoiceLineResponse.FromDomain).ToList());
}
