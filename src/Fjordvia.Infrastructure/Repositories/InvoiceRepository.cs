using Fjordvia.Core.Domain;
using Fjordvia.Core.Interfaces;
using Fjordvia.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Fjordvia.Infrastructure.Repositories;

internal sealed class InvoiceRepository(FjordviaDbContext dbContext) : IInvoiceRepository
{
    public async Task<IReadOnlyCollection<Invoice>> ListAsync(CancellationToken cancellationToken) =>
        await dbContext.Invoices
            .AsNoTracking()
            .Include(invoice => invoice.BusinessPartner)
            .Include(invoice => invoice.Lines)
            .OrderByDescending(invoice => invoice.InvoiceDate)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(Invoice invoice, CancellationToken cancellationToken) =>
        await dbContext.Invoices.AddAsync(invoice, cancellationToken);
}
