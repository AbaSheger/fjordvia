using Fjordvia.Core.Domain;
using Fjordvia.Core.Interfaces;
using Fjordvia.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Fjordvia.Infrastructure.Repositories;

internal sealed class BusinessPartnerRepository(FjordviaDbContext dbContext) : IBusinessPartnerRepository
{
    public async Task<IReadOnlyCollection<BusinessPartner>> ListAsync(CancellationToken cancellationToken) =>
        await dbContext.BusinessPartners
            .AsNoTracking()
            .OrderBy(partner => partner.Name)
            .ToListAsync(cancellationToken);

    public Task<BusinessPartner?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        dbContext.BusinessPartners.FirstOrDefaultAsync(partner => partner.Id == id, cancellationToken);

    public Task<BusinessPartner?> GetByOrganizationNumberAsync(string organizationNumber, CancellationToken cancellationToken) =>
        dbContext.BusinessPartners.FirstOrDefaultAsync(
            partner => partner.OrganizationNumber == organizationNumber,
            cancellationToken);

    public async Task AddAsync(BusinessPartner partner, CancellationToken cancellationToken) =>
        await dbContext.BusinessPartners.AddAsync(partner, cancellationToken);
}
