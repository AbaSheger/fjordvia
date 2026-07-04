using System.ComponentModel.DataAnnotations;

namespace Fjordvia.Api.Dtos;

public sealed record CreateBusinessPartnerRequest(
    [Required, MaxLength(160)] string Name,
    [Required, MaxLength(40)] string OrganizationNumber,
    [Required, EmailAddress, MaxLength(160)] string Email,
    [Required, StringLength(2, MinimumLength = 2)] string CountryCode);
