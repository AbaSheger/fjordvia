using Fjordvia.Core.Integrations;

namespace Fjordvia.Api.Dtos;

public sealed record InvoiceImportResponse(
    InvoiceResponse Invoice,
    AccountingInvoiceExport AccountingInvoice,
    CrmInvoiceActivity CrmActivity)
{
    public static InvoiceImportResponse FromMapping(ErpInvoiceMappingResult result) =>
        new(InvoiceResponse.FromDomain(result.Invoice), result.AccountingInvoice, result.CrmActivity);
}
