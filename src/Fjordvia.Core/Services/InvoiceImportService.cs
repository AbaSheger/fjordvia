using Fjordvia.Core.Domain;
using Fjordvia.Core.Exceptions;
using Fjordvia.Core.Integrations;
using Fjordvia.Core.Interfaces;
using Fjordvia.Core.Mapping;

namespace Fjordvia.Core.Services;

public sealed class InvoiceImportService(
    IBusinessPartnerRepository partners,
    IInvoiceRepository invoices,
    IIntegrationLogRepository logs,
    IUnitOfWork unitOfWork,
    ErpInvoiceMapper mapper)
{
    public Task<IReadOnlyCollection<Invoice>> ListAsync(CancellationToken cancellationToken) =>
        invoices.ListAsync(cancellationToken);

    public async Task<ErpInvoiceMappingResult> ImportAsync(ErpInvoiceImport import, CancellationToken cancellationToken)
    {
        var partner = await partners.GetByOrganizationNumberAsync(import.CustomerOrganizationNumber.Trim(), cancellationToken);
        if (partner is null)
        {
            await logs.AddAsync(new IntegrationLog
            {
                Type = IntegrationType.InvoiceImport,
                Status = IntegrationStatus.Failed,
                SourceSystem = "ERP",
                TargetSystem = "Accounting and CRM",
                Reference = import.ExternalInvoiceNumber,
                Message = "Business partner was not found for invoice import."
            }, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            throw new NotFoundException("Business partner was not found for invoice import.");
        }

        var mapping = mapper.Map(import, partner);
        await invoices.AddAsync(mapping.Invoice, cancellationToken);
        await logs.AddAsync(new IntegrationLog
        {
            Type = IntegrationType.InvoiceImport,
            Status = IntegrationStatus.Completed,
            SourceSystem = "ERP",
            TargetSystem = "Accounting and CRM",
            Reference = import.ExternalInvoiceNumber,
            Message = "Invoice imported and mapped to accounting and CRM payloads.",
            CompletedAt = DateTimeOffset.UtcNow
        }, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return mapping;
    }
}
