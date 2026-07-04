namespace Fjordvia.Core.Domain;

public sealed class IntegrationLog
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public IntegrationType Type { get; set; }
    public IntegrationStatus Status { get; set; } = IntegrationStatus.Pending;
    public string SourceSystem { get; set; } = string.Empty;
    public string TargetSystem { get; set; } = string.Empty;
    public string Reference { get; set; } = string.Empty;
    public string? Message { get; set; }
    public int RetryCount { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? CompletedAt { get; set; }
    public DateTimeOffset? LastRetriedAt { get; set; }
}
