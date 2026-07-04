using Fjordvia.Core.Domain;
using Fjordvia.Core.Exceptions;
using Fjordvia.Core.Interfaces;
using Fjordvia.Core.Services;

namespace Fjordvia.Tests.Services;

public sealed class IntegrationRetryServiceTests
{
    [Fact]
    public async Task RetryAsync_MarksFailedLogAsPendingAndIncrementsRetryCount()
    {
        var log = new IntegrationLog
        {
            Id = Guid.NewGuid(),
            Type = IntegrationType.InvoiceImport,
            Status = IntegrationStatus.Failed,
            SourceSystem = "ERP",
            TargetSystem = "Accounting",
            Reference = "ERP-INV-9001",
            RetryCount = 1
        };
        var repository = new InMemoryIntegrationLogRepository([log]);
        var unitOfWork = new CountingUnitOfWork();
        var service = new IntegrationRetryService(repository, unitOfWork);

        var result = await service.RetryAsync(log.Id, CancellationToken.None);

        Assert.Equal(IntegrationStatus.Pending, result.Status);
        Assert.Equal(2, result.RetryCount);
        Assert.NotNull(result.LastRetriedAt);
        Assert.Equal(1, unitOfWork.SaveCount);
    }

    [Fact]
    public async Task RetryAsync_RejectsCompletedLog()
    {
        var log = new IntegrationLog
        {
            Id = Guid.NewGuid(),
            Type = IntegrationType.InvoiceImport,
            Status = IntegrationStatus.Completed,
            SourceSystem = "ERP",
            TargetSystem = "Accounting",
            Reference = "ERP-INV-9002"
        };
        var service = new IntegrationRetryService(
            new InMemoryIntegrationLogRepository([log]),
            new CountingUnitOfWork());

        await Assert.ThrowsAsync<DomainValidationException>(() =>
            service.RetryAsync(log.Id, CancellationToken.None));
    }

    private sealed class InMemoryIntegrationLogRepository(List<IntegrationLog> logs) : IIntegrationLogRepository
    {
        public Task<IReadOnlyCollection<IntegrationLog>> ListAsync(CancellationToken cancellationToken) =>
            Task.FromResult<IReadOnlyCollection<IntegrationLog>>(logs);

        public Task<IntegrationLog?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
            Task.FromResult(logs.FirstOrDefault(log => log.Id == id));

        public Task AddAsync(IntegrationLog log, CancellationToken cancellationToken)
        {
            logs.Add(log);
            return Task.CompletedTask;
        }
    }

    private sealed class CountingUnitOfWork : IUnitOfWork
    {
        public int SaveCount { get; private set; }

        public Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            SaveCount += 1;
            return Task.CompletedTask;
        }
    }
}
