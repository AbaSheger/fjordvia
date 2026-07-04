using Fjordvia.Core.Domain;

namespace Fjordvia.Core.Interfaces;

public interface IIntegrationLogRepository
{
    Task<IReadOnlyCollection<IntegrationLog>> ListAsync(CancellationToken cancellationToken);
    Task<IntegrationLog?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task AddAsync(IntegrationLog log, CancellationToken cancellationToken);
}
