using Fjordvia.Core.Domain;

namespace Fjordvia.Core.Interfaces;

public interface IInvoiceRepository
{
    Task<IReadOnlyCollection<Invoice>> ListAsync(CancellationToken cancellationToken);
    Task AddAsync(Invoice invoice, CancellationToken cancellationToken);
}
