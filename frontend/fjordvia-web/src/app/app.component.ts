import { Component, OnInit, computed, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import {
  BusinessPartner,
  FjordviaApiService,
  ImportInvoiceRequest,
  IntegrationLog
} from './fjordvia-api.service';

interface InvoiceForm {
  businessPartnerId: string;
  invoiceNumber: string;
  currency: string;
  description: string;
  quantity: number;
  unitPrice: number;
}

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit {
  readonly businessPartners = signal<BusinessPartner[]>([]);
  readonly integrationLogs = signal<IntegrationLog[]>([]);
  readonly isLoading = signal(false);
  readonly message = signal<string | null>(null);
  readonly error = signal<string | null>(null);

  readonly completedIntegrations = computed(
    () => this.integrationLogs().filter((log) => log.status === 'Completed').length
  );
  readonly failedIntegrations = computed(
    () => this.integrationLogs().filter((log) => log.status === 'Failed').length
  );
  readonly latestIntegrationLogs = computed(() =>
    [...this.integrationLogs()]
      .sort((first, second) => new Date(second.createdAt).getTime() - new Date(first.createdAt).getTime())
      .slice(0, 6)
  );

  invoiceForm: InvoiceForm = {
    businessPartnerId: '',
    invoiceNumber: `WEB-${new Date().toISOString().slice(0, 10).replaceAll('-', '')}`,
    currency: 'SEK',
    description: 'Integration service',
    quantity: 1,
    unitPrice: 1000
  };

  constructor(private readonly api: FjordviaApiService) {}

  ngOnInit(): void {
    this.refreshData();
  }

  refreshData(): void {
    this.isLoading.set(true);
    this.error.set(null);

    this.api.getBusinessPartners().subscribe({
      next: (partners) => {
        this.businessPartners.set(partners);
        if (!this.invoiceForm.businessPartnerId && partners.length > 0) {
          this.invoiceForm.businessPartnerId = partners[0].id;
        }
        this.loadIntegrationLogs();
      },
      error: () => {
        this.isLoading.set(false);
        this.error.set('Unable to load business partners. Confirm the API is running.');
      }
    });
  }

  loadIntegrationLogs(): void {
    this.api.getIntegrationLogs().subscribe({
      next: (logs) => {
        this.integrationLogs.set(logs);
        this.isLoading.set(false);
      },
      error: () => {
        this.isLoading.set(false);
        this.error.set('Unable to load integration logs.');
      }
    });
  }

  importInvoice(): void {
    const partner = this.businessPartners().find((item) => item.id === this.invoiceForm.businessPartnerId);
    if (!partner) {
      this.error.set('Select a business partner before importing an invoice.');
      return;
    }

    const today = new Date();
    const dueDate = new Date(today);
    dueDate.setDate(today.getDate() + 14);

    const request: ImportInvoiceRequest = {
      externalInvoiceNumber: this.invoiceForm.invoiceNumber.trim(),
      customerName: partner.name,
      customerOrganizationNumber: partner.organizationNumber,
      customerEmail: partner.email,
      countryCode: partner.countryCode,
      currency: this.invoiceForm.currency.trim().toUpperCase(),
      invoiceDate: this.toDateOnly(today),
      dueDate: this.toDateOnly(dueDate),
      lines: [
        {
          description: this.invoiceForm.description.trim(),
          quantity: Number(this.invoiceForm.quantity),
          unitPrice: Number(this.invoiceForm.unitPrice)
        }
      ]
    };

    this.message.set(null);
    this.error.set(null);

    this.api.importInvoice(request).subscribe({
      next: (response) => {
        this.message.set(`Invoice ${response.invoice.externalInvoiceNumber} imported successfully.`);
        this.invoiceForm.invoiceNumber = `WEB-${new Date().toISOString().replace(/[-:.TZ]/g, '').slice(0, 14)}`;
        this.loadIntegrationLogs();
      },
      error: (response: { error?: { detail?: string; title?: string } }) => {
        this.error.set(response.error?.detail ?? response.error?.title ?? 'Invoice import failed.');
      }
    });
  }

  retry(log: IntegrationLog): void {
    this.message.set(null);
    this.error.set(null);

    this.api.retryIntegrationLog(log.id).subscribe({
      next: () => {
        this.message.set(`Retry requested for ${log.reference}.`);
        this.loadIntegrationLogs();
      },
      error: (response: { error?: { detail?: string; title?: string } }) => {
        this.error.set(response.error?.detail ?? response.error?.title ?? 'Retry failed.');
      }
    });
  }

  statusClass(status: string): string {
    return `status status-${status.toLowerCase()}`;
  }

  private toDateOnly(date: Date): string {
    return date.toISOString().slice(0, 10);
  }
}
