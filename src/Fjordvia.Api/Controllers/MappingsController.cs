using Fjordvia.Api.Dtos;
using Fjordvia.Core.Domain;
using Fjordvia.Core.Integrations;
using Fjordvia.Core.Mapping;
using Microsoft.AspNetCore.Mvc;

namespace Fjordvia.Api.Controllers;

[ApiController]
[Route("api/mappings")]
public sealed class MappingsController(ErpInvoiceMapper mapper) : ControllerBase
{
    [HttpPost("erp-invoice")]
    [ProducesResponseType(typeof(InvoiceImportResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public ActionResult<InvoiceImportResponse> PreviewErpInvoiceMapping(PreviewInvoiceMappingRequest request)
    {
        var partner = new BusinessPartner
        {
            Id = Guid.NewGuid(),
            Name = request.CustomerName,
            OrganizationNumber = request.CustomerOrganizationNumber,
            Email = request.CustomerEmail,
            CountryCode = request.CountryCode
        };

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

        var result = mapper.Map(import, partner);
        return Ok(InvoiceImportResponse.FromMapping(result));
    }
}
