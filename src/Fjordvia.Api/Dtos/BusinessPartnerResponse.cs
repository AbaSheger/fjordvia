using Fjordvia.Core.Domain;

namespace Fjordvia.Api.Dtos;

public sealed record BusinessPartnerResponse(
    Guid Id,
    string Name,
    string OrganizationNumber,
    string Email,
    string CountryCode,
    DateTimeOffset CreatedAt)
{
    public static BusinessPartnerResponse FromDomain(BusinessPartner partner) =>
        new(partner.Id, partner.Name, partner.OrganizationNumber, partner.Email, partner.CountryCode, partner.CreatedAt);
}
