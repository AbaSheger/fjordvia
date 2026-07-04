using Fjordvia.Core.Domain;

namespace Fjordvia.Api.Dtos;

public sealed record IntegrationLogResponse(
    Guid Id,
    string Type,
    string Status,
    string SourceSystem,
    string TargetSystem,
    string Reference,
    string? Message,
    int RetryCount,
    DateTimeOffset CreatedAt,
    DateTimeOffset? CompletedAt,
    DateTimeOffset? LastRetriedAt)
{
    public static IntegrationLogResponse FromDomain(IntegrationLog log) =>
        new(
            log.Id,
            log.Type.ToString(),
            log.Status.ToString(),
            log.SourceSystem,
            log.TargetSystem,
            log.Reference,
            log.Message,
            log.RetryCount,
            log.CreatedAt,
            log.CompletedAt,
            log.LastRetriedAt);
}
