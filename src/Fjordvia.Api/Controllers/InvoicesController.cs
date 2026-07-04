using Fjordvia.Api.Dtos;
using Fjordvia.Core.Integrations;
using Fjordvia.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace Fjordvia.Api.Controllers;

[ApiController]
[Route("api/invoices")]
public sealed class InvoicesController(InvoiceImportService service) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<InvoiceResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<InvoiceResponse>>> List(CancellationToken cancellationToken)
    {
        var invoices = await service.ListAsync(cancellationToken);
        return Ok(invoices.Select(InvoiceResponse.FromDomain).ToList());
    }

    [HttpPost("import")]
    [ProducesResponseType(typeof(InvoiceImportResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<InvoiceImportResponse>> Import(
        ImportInvoiceRequest request,
        CancellationToken cancellationToken)
    {
        var import = new ErpInvoiceImport(
            request.ExternalInvoiceNumber,
            request.CustomerName,
            request.CustomerOrganizationNumber,
            request.CustomerEmail,
            request.CountryCode,
            request.Currency,
            request.InvoiceDate,
            request.DueDate,
            request.Lines.Select(line => new ErpInvoiceLineImport(
                line.Description,
                line.Quantity,
                line.UnitPrice)).ToList());

        var result = await service.ImportAsync(import, cancellationToken);

        return CreatedAtAction(
            nameof(List),
            new { id = result.Invoice.Id },
            InvoiceImportResponse.FromMapping(result));
    }
}
