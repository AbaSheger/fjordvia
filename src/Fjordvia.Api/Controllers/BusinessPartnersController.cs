using Fjordvia.Api.Dtos;
using Fjordvia.Core.Domain;
using Fjordvia.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace Fjordvia.Api.Controllers;

[ApiController]
[Route("api/business-partners")]
public sealed class BusinessPartnersController(BusinessPartnerService service) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<BusinessPartnerResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<BusinessPartnerResponse>>> List(CancellationToken cancellationToken)
    {
        var partners = await service.ListAsync(cancellationToken);
        return Ok(partners.Select(BusinessPartnerResponse.FromDomain).ToList());
    }

    [HttpPost]
    [ProducesResponseType(typeof(BusinessPartnerResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BusinessPartnerResponse>> Create(
        CreateBusinessPartnerRequest request,
        CancellationToken cancellationToken)
    {
        var partner = new BusinessPartner
        {
            Name = request.Name,
            OrganizationNumber = request.OrganizationNumber,
            Email = request.Email,
            CountryCode = request.CountryCode
        };

        var created = await service.CreateAsync(partner, cancellationToken);
        return CreatedAtAction(nameof(List), new { id = created.Id }, BusinessPartnerResponse.FromDomain(created));
    }
}
