using Fjordvia.Core.Domain;
using Fjordvia.Core.Interfaces;
using Fjordvia.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Fjordvia.Infrastructure.Repositories;

internal sealed class IntegrationLogRepository(FjordviaDbContext dbContext) : IIntegrationLogRepository
{
    public async Task<IReadOnlyCollection<IntegrationLog>> ListAsync(CancellationToken cancellationToken) =>
        await dbContext.IntegrationLogs
            .AsNoTracking()
            .OrderByDescending(log => log.CreatedAt)
            .ToListAsync(cancellationToken);

    public Task<IntegrationLog?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        dbContext.IntegrationLogs.FirstOrDefaultAsync(log => log.Id == id, cancellationToken);

    public async Task AddAsync(IntegrationLog log, CancellationToken cancellationToken) =>
        await dbContext.IntegrationLogs.AddAsync(log, cancellationToken);
}
