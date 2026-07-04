using System.ComponentModel.DataAnnotations;

namespace Fjordvia.Api.Dtos;

public sealed record PreviewInvoiceMappingRequest(
    [Required, MaxLength(80)] string ExternalInvoiceNumber,
    [Required, MaxLength(160)] string CustomerName,
    [Required, MaxLength(40)] string CustomerOrganizationNumber,
    [Required, EmailAddress, MaxLength(160)] string CustomerEmail,
    [Required, StringLength(2, MinimumLength = 2)] string CountryCode,
    [Required, StringLength(3, MinimumLength = 3)] string Currency,
    DateOnly InvoiceDate,
    DateOnly DueDate,
    [Required, MinLength(1)] IReadOnlyCollection<InvoiceLineRequest> Lines);
