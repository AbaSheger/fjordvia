using Fjordvia.Api.Dtos;
using Fjordvia.Core.Interfaces;
using Fjordvia.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace Fjordvia.Api.Controllers;

[ApiController]
[Route("api/integration-logs")]
public sealed class IntegrationLogsController(
    IIntegrationLogRepository logs,
    IntegrationRetryService retryService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<IntegrationLogResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<IntegrationLogResponse>>> List(CancellationToken cancellationToken)
    {
        var results = await logs.ListAsync(cancellationToken);
        return Ok(results.Select(IntegrationLogResponse.FromDomain).ToList());
    }

    [HttpPost("{id:guid}/retry")]
    [ProducesResponseType(typeof(IntegrationLogResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IntegrationLogResponse>> Retry(Guid id, CancellationToken cancellationToken)
    {
        var result = await retryService.RetryAsync(id, cancellationToken);
        return Ok(IntegrationLogResponse.FromDomain(result));
    }
}
