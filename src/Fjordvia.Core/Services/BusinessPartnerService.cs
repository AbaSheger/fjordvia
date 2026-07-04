using Fjordvia.Core.Domain;
using Fjordvia.Core.Exceptions;
using Fjordvia.Core.Interfaces;

namespace Fjordvia.Core.Services;

public sealed class BusinessPartnerService(
    IBusinessPartnerRepository partners,
    IIntegrationLogRepository logs,
    IUnitOfWork unitOfWork)
{
    public Task<IReadOnlyCollection<BusinessPartner>> ListAsync(CancellationToken cancellationToken) =>
        partners.ListAsync(cancellationToken);

    public async Task<BusinessPartner> CreateAsync(BusinessPartner partner, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(partner.Name))
        {
            throw new DomainValidationException("Business partner name is required.");
        }

        if (string.IsNullOrWhiteSpace(partner.OrganizationNumber))
        {
            throw new DomainValidationException("Organization number is required.");
        }

        if (string.IsNullOrWhiteSpace(partner.Email) || !partner.Email.Contains('@'))
        {
            throw new DomainValidationException("A valid email address is required.");
        }

        var existing = await partners.GetByOrganizationNumberAsync(partner.OrganizationNumber.Trim(), cancellationToken);
        if (existing is not null)
        {
            throw new DomainValidationException("A business partner with this organization number already exists.");
        }

        partner.Name = partner.Name.Trim();
        partner.OrganizationNumber = partner.OrganizationNumber.Trim();
        partner.Email = partner.Email.Trim();
        partner.CountryCode = partner.CountryCode.Trim().ToUpperInvariant();

        await partners.AddAsync(partner, cancellationToken);
        await logs.AddAsync(new IntegrationLog
        {
            Type = IntegrationType.BusinessPartnerSync,
            Status = IntegrationStatus.Completed,
            SourceSystem = "Fjordvia API",
            TargetSystem = "Partner Registry",
            Reference = partner.OrganizationNumber,
            Message = "Business partner created.",
            CompletedAt = DateTimeOffset.UtcNow
        }, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return partner;
    }
}
