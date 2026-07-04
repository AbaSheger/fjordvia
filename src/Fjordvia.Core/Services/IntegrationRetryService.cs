using Fjordvia.Core.Domain;
using Fjordvia.Core.Exceptions;
using Fjordvia.Core.Interfaces;

namespace Fjordvia.Core.Services;

public sealed class IntegrationRetryService(IIntegrationLogRepository logs, IUnitOfWork unitOfWork)
{
    public async Task<IntegrationLog> RetryAsync(Guid logId, CancellationToken cancellationToken)
    {
        var log = await logs.GetByIdAsync(logId, cancellationToken);
        if (log is null)
        {
            throw new NotFoundException("Integration log was not found.");
        }

        if (log.Status != IntegrationStatus.Failed)
        {
            throw new DomainValidationException("Only failed integrations can be retried.");
        }

        log.Status = IntegrationStatus.Pending;
        log.RetryCount += 1;
        log.LastRetriedAt = DateTimeOffset.UtcNow;
        log.Message = "Retry requested. Integration is pending reprocessing.";

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return log;
    }
}
