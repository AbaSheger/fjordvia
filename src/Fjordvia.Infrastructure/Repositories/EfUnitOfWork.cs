using Fjordvia.Core.Interfaces;
using Fjordvia.Infrastructure.Data;

namespace Fjordvia.Infrastructure.Repositories;

internal sealed class EfUnitOfWork(FjordviaDbContext dbContext) : IUnitOfWork
{
    public async Task SaveChangesAsync(CancellationToken cancellationToken) =>
        await dbContext.SaveChangesAsync(cancellationToken);
}
