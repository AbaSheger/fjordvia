import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../environments/environment';

export interface BusinessPartner {
  id: string;
  name: string;
  organizationNumber: string;
  email: string;
  countryCode: string;
  createdAt: string;
}

export interface IntegrationLog {
  id: string;
  type: string;
  status: 'Pending' | 'Completed' | 'Failed' | string;
  sourceSystem: string;
  targetSystem: string;
  reference: string;
  message?: string;
  retryCount: number;
  createdAt: string;
  completedAt?: string;
  lastRetriedAt?: string;
}

export interface InvoiceLineRequest {
  description: string;
  quantity: number;
  unitPrice: number;
}

export interface ImportInvoiceRequest {
  externalInvoiceNumber: string;
  customerName: string;
  customerOrganizationNumber: string;
  customerEmail: string;
  countryCode: string;
  currency: string;
  invoiceDate: string;
  dueDate: string;
  lines: InvoiceLineRequest[];
}

export interface InvoiceImportResponse {
  invoice: {
    id: string;
    externalInvoiceNumber: string;
    totalAmount: number;
  };
}

@Injectable({ providedIn: 'root' })
export class FjordviaApiService {
  private readonly baseUrl = environment.apiBaseUrl;

  constructor(private readonly http: HttpClient) {}

  getBusinessPartners(): Observable<BusinessPartner[]> {
    return this.http.get<BusinessPartner[]>(`${this.baseUrl}/api/business-partners`);
  }

  importInvoice(request: ImportInvoiceRequest): Observable<InvoiceImportResponse> {
    return this.http.post<InvoiceImportResponse>(`${this.baseUrl}/api/invoices/import`, request);
  }

  getIntegrationLogs(): Observable<IntegrationLog[]> {
    return this.http.get<IntegrationLog[]>(`${this.baseUrl}/api/integration-logs`);
  }

  retryIntegrationLog(id: string): Observable<IntegrationLog> {
    return this.http.post<IntegrationLog>(`${this.baseUrl}/api/integration-logs/${id}/retry`, {});
  }
}
