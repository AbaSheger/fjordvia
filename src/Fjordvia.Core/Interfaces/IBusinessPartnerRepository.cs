using Fjordvia.Core.Domain;

namespace Fjordvia.Core.Interfaces;

public interface IBusinessPartnerRepository
{
    Task<IReadOnlyCollection<BusinessPartner>> ListAsync(CancellationToken cancellationToken);
    Task<BusinessPartner?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<BusinessPartner?> GetByOrganizationNumberAsync(string organizationNumber, CancellationToken cancellationToken);
    Task AddAsync(BusinessPartner partner, CancellationToken cancellationToken);
}
