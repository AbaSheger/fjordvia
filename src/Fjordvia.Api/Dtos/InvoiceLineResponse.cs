using Fjordvia.Core.Domain;

namespace Fjordvia.Api.Dtos;

public sealed record InvoiceLineResponse(
    string Description,
    int Quantity,
    decimal UnitPrice,
    decimal LineTotal)
{
    public static InvoiceLineResponse FromDomain(InvoiceLine line) =>
        new(line.Description, line.Quantity, line.UnitPrice, line.LineTotal);
}
