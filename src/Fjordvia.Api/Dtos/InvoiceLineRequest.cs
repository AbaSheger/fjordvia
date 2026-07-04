using System.ComponentModel.DataAnnotations;

namespace Fjordvia.Api.Dtos;

public sealed record InvoiceLineRequest(
    [Required, MaxLength(240)] string Description,
    [Range(1, int.MaxValue)] int Quantity,
    [Range(0, 999999999)] decimal UnitPrice);
