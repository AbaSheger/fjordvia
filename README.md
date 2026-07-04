# Fjordvia

Fjordvia is a Nordic-inspired B2B SaaS backend portfolio project for cloud-based business system integrations. It demonstrates a practical .NET backend that connects fictional ERP, accounting, CRM, warehouse, and finance workflows through REST APIs, data mapping, retry handling, integration logs, SQL Server persistence, and tests.

This repository contains the backend API and a small Angular dashboard for demonstrating the integration workflow locally.

## Business Problem

Business systems often need to exchange the same operational data in different shapes. An ERP invoice may need to become an accounting posting and a CRM activity, while failures must be visible and retryable for support teams.

Fjordvia models that workflow with:

- Business partner management
- ERP invoice import
- ERP-to-accounting and ERP-to-CRM mapping
- Integration logs with `Pending`, `Completed`, and `Failed` statuses
- Retry endpoint for failed integration logs
- Seed data for local testing

## Tech Stack

- .NET 8
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- Swagger/OpenAPI
- xUnit
- Docker Compose
- GitHub Actions
- Angular

## Solution Structure

```text
src/
  Fjordvia.Api/             ASP.NET Core controllers, DTOs, middleware, Swagger
  Fjordvia.Core/            Domain models, services, mapping logic, repository contracts
  Fjordvia.Infrastructure/  EF Core DbContext, repositories, seed data
tests/
  Fjordvia.Tests/           xUnit tests for mapping and retry behavior
frontend/
  fjordvia-web/             Angular dashboard for local workflow demos
```

## Local Setup

Prerequisites:

- .NET 8 SDK
- Docker Desktop or Docker Engine

Create a local `.env` file from the example and set a strong local SQL Server password:

```powershell
Copy-Item .env.example .env
```

Example `.env` shape:

```text
ACCEPT_EULA=Y
MSSQL_SA_PASSWORD=<your-local-strong-password>
```

The `.env` file is ignored by Git and must not be committed.

Configure the API connection string locally. The recommended approach is an environment variable:

```powershell
$env:ConnectionStrings__FjordviaDatabase="Server=localhost,1433;Database=Fjordvia;User Id=sa;Password=<your-local-strong-password>;TrustServerCertificate=True;Encrypt=True"
```

Alternatively, copy the example development settings file and replace the placeholder password:

```powershell
Copy-Item src/Fjordvia.Api/appsettings.Development.example.json src/Fjordvia.Api/appsettings.Development.json
```

Do not commit real local passwords in `appsettings.Development.json`.

Start SQL Server:

```powershell
docker compose up -d
```

Run the API:

```powershell
dotnet run --project src/Fjordvia.Api
```

In development, the API creates the database schema with EF Core `EnsureCreated` and inserts seed data. Swagger is available at the URL printed by `dotnet run`, usually:

```text
https://localhost:7000/swagger
```

Open `/swagger` in the browser for the local API URL shown in the terminal.

## Angular Frontend

The Angular dashboard lives in `frontend/fjordvia-web` and expects the API to run at `http://localhost:5131` by default.

Install frontend dependencies:

```powershell
cd frontend/fjordvia-web
npm install
```

Start the Angular development server:

```powershell
npm start
```

Open:

```text
http://localhost:4200
```

The dashboard shows business partner totals, integration log totals, completed and failed integration counts, a business partner table, an invoice import form, and an integration logs table with retry actions for failed logs.

To build the frontend:

```powershell
npm run build
```

### Screenshot Checklist

Use this checklist for a portfolio screenshot:

- Dashboard cards show partner and integration counts.
- Business partners table shows seeded partner data.
- Invoice import form is visible with an existing partner selected.
- Integration logs table shows status badges and retry buttons only for failed logs.
- No browser console errors are present.

## Smoke Test

With SQL Server and the API running locally, run the PowerShell smoke test from the repository root:

```powershell
.\scripts\smoke-test.ps1
```

The script uses `http://localhost:5131` by default. To target a different local API URL:

```powershell
.\scripts\smoke-test.ps1 -BaseUrl "https://localhost:7000"
```

The smoke test checks that the API is reachable, reads business partners, imports a sample invoice for the first partner returned by the API, reads integration logs, and retries a failed integration log when one exists.

## Build and Test

```powershell
dotnet build
dotnet test
```

## Seed Data

Seeded business partners:

- `Northwind Timber AB`, organization number `559100-1010`
- `Aurora Components AS`, organization number `918273645`

Seeded failed integration log for retry testing:

- `8626ef4a-01ff-4a91-bf44-3ed44a597a18`

## API Examples

Create a business partner:

```http
POST /api/business-partners
Content-Type: application/json

{
  "name": "Harbor Ledger Oy",
  "organizationNumber": "3152467-8",
  "email": "finance@harbor-ledger.example",
  "countryCode": "FI"
}
```

List business partners:

```http
GET /api/business-partners
```

Import an ERP invoice:

```http
POST /api/invoices/import
Content-Type: application/json

{
  "externalInvoiceNumber": "ERP-INV-1001",
  "customerName": "Northwind Timber AB",
  "customerOrganizationNumber": "559100-1010",
  "customerEmail": "finance@northwind-timber.example",
  "countryCode": "SE",
  "currency": "SEK",
  "invoiceDate": "2026-03-01",
  "dueDate": "2026-03-31",
  "lines": [
    {
      "description": "Integration monitoring",
      "quantity": 2,
      "unitPrice": 1200
    },
    {
      "description": "Retry handling setup",
      "quantity": 1,
      "unitPrice": 800
    }
  ]
}
```

Preview ERP invoice mapping without saving:

```http
POST /api/mappings/erp-invoice
Content-Type: application/json

{
  "externalInvoiceNumber": "ERP-INV-1002",
  "customerName": "Aurora Components AS",
  "customerOrganizationNumber": "918273645",
  "customerEmail": "accounts@aurora-components.example",
  "countryCode": "NO",
  "currency": "NOK",
  "invoiceDate": "2026-04-01",
  "dueDate": "2026-04-15",
  "lines": [
    {
      "description": "CRM mapping setup",
      "quantity": 1,
      "unitPrice": 1500
    }
  ]
}
```

List integration logs:

```http
GET /api/integration-logs
```

Retry a failed integration:

```http
POST /api/integration-logs/8626ef4a-01ff-4a91-bf44-3ed44a597a18/retry
```

## Architecture

Fjordvia uses a simple layered backend design:

- `Fjordvia.Api` owns HTTP concerns: controllers, DTOs, Swagger metadata, and central error handling.
- `Fjordvia.Core` owns business behavior: domain models, validation-oriented services, repository interfaces, retry rules, and ERP mapping logic.
- `Fjordvia.Infrastructure` owns persistence: EF Core `DbContext`, SQL Server configuration, repositories, and local seed data.

The MVP avoids microservices and background workers. The retry endpoint marks failed integrations as `Pending`, which keeps the domain behavior visible without pretending to implement a full asynchronous processor.

## Azure Roadmap

Future production-oriented improvements:

- Azure Functions for background processing of pending integrations
- Azure Service Bus for asynchronous integration messages
- Azure SQL for managed production database hosting
- Azure Key Vault for connection strings and secrets
- Azure Monitor for logs, metrics, and operational dashboards
- Azure API Management as an optional gateway for throttling, versioning, and partner access control
