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
- Azure Service Bus
- Azure Functions isolated worker

## Solution Structure

```text
src/
  Fjordvia.Api/             ASP.NET Core controllers, DTOs, middleware, Swagger
  Fjordvia.Core/            Domain models, services, mapping logic, repository contracts
  Fjordvia.Infrastructure/  EF Core DbContext, repositories, seed data
functions/
  Fjordvia.Functions/       Azure Functions isolated worker for Service Bus messages
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

## Azure Integration Mode

Fjordvia supports an Azure-style asynchronous integration flow without requiring Azure for local development.

```text
Angular dashboard
  -> Fjordvia.Api
  -> Azure Service Bus queue
  -> Fjordvia.Functions
  -> SQL Server / Azure SQL
```

Local mode is the default. If Service Bus settings are missing, the API uses a local publisher that logs the invoice import message and keeps the existing synchronous API behavior unchanged. This means Docker SQL Server, Swagger, the Angular dashboard, and `scripts/smoke-test.ps1` continue to work without Azure resources.

Azure Service Bus mode is enabled only when both settings are configured:

```powershell
$env:ServiceBus__ConnectionString="<your-service-bus-connection-string>"
$env:ServiceBus__InvoiceImportQueueName="invoice-imports"
```

The same settings can be stored with user secrets for local development:

```powershell
dotnet user-secrets set "ServiceBus:ConnectionString" "<your-service-bus-connection-string>" --project src/Fjordvia.Api
dotnet user-secrets set "ServiceBus:InvoiceImportQueueName" "invoice-imports" --project src/Fjordvia.Api
```

Do not commit real Service Bus connection strings. `src/Fjordvia.Api/appsettings.Development.example.json` and `functions/Fjordvia.Functions/local.settings.example.json` show the required setting names with placeholders only.

To run the Azure Functions project later, install Azure Functions Core Tools, copy `functions/Fjordvia.Functions/local.settings.example.json` to `functions/Fjordvia.Functions/local.settings.json`, replace the placeholders, and run:

```powershell
cd functions/Fjordvia.Functions
func start
```

The function listens for invoice import messages, deserializes the shared `InvoiceImportMessage` contract from `Fjordvia.Core`, and logs the invoice reference, partner, amount, source system, and target system. The current portfolio version demonstrates the queue handoff without deploying infrastructure.

## Azure Deployment

Target deployment architecture:

```text
Angular local dashboard
  -> Fjordvia.Api on Azure App Service
  -> Azure Service Bus queue
  -> Fjordvia.Functions on Azure Function App
  -> Azure SQL Database
```

The backend is ready to run from Azure app settings. No connection strings or keys should be committed to the repository.

Required API app settings:

```text
ConnectionStrings__FjordviaDatabase=<azure-sql-connection-string>
ServiceBus__ConnectionString=<service-bus-connection-string>
ServiceBus__InvoiceImportQueueName=invoice-imports
Cors__AllowedOrigins__0=http://localhost:4200
Database__EnsureCreatedOnStartup=true
```

`Database__EnsureCreatedOnStartup=true` is useful for a portfolio deployment to initialize an empty Azure SQL database. For a more production-like setup, replace this with EF Core migrations and remove the startup schema creation setting after the database exists.

Required Function App settings:

```text
AzureWebJobsStorage=<function-storage-connection-string>
FUNCTIONS_WORKER_RUNTIME=dotnet-isolated
ServiceBus__ConnectionString=<service-bus-connection-string>
ServiceBus__InvoiceImportQueueName=invoice-imports
```

### Azure Deployment Checklist

- Resource Group
- Azure SQL logical server and Azure SQL Database
- Azure Service Bus namespace
- Azure Service Bus queue, for example `invoice-imports`
- Azure App Service Plan
- Azure App Service for `Fjordvia.Api`
- Storage Account for the Function App runtime
- Azure Function App for `Fjordvia.Functions`
- App settings configured with placeholders replaced in Azure only
- Local Angular origin allowed through App Service CORS config/app settings

### Example Azure CLI Commands

These commands are examples only. Replace every placeholder before running them. Do not paste real secrets into source-controlled files.

```powershell
$resourceGroup = "<resource-group-name>"
$location = "<azure-region>"
$sqlServer = "<unique-sql-server-name>"
$sqlDatabase = "fjordvia-db"
$sqlAdmin = "<sql-admin-user>"
$sqlPassword = "<sql-admin-password>"
$serviceBusNamespace = "<unique-service-bus-namespace>"
$queueName = "invoice-imports"
$appServicePlan = "<app-service-plan-name>"
$apiApp = "<unique-api-app-name>"
$storageAccount = "<uniquestorageaccount>"
$functionApp = "<unique-function-app-name>"

az group create --name $resourceGroup --location $location

az sql server create `
  --resource-group $resourceGroup `
  --name $sqlServer `
  --location $location `
  --admin-user $sqlAdmin `
  --admin-password $sqlPassword

az sql db create `
  --resource-group $resourceGroup `
  --server $sqlServer `
  --name $sqlDatabase `
  --service-objective Basic

az servicebus namespace create `
  --resource-group $resourceGroup `
  --name $serviceBusNamespace `
  --location $location `
  --sku Basic

az servicebus queue create `
  --resource-group $resourceGroup `
  --namespace-name $serviceBusNamespace `
  --name $queueName

az appservice plan create `
  --resource-group $resourceGroup `
  --name $appServicePlan `
  --location $location `
  --sku B1 `
  --is-linux

az webapp create `
  --resource-group $resourceGroup `
  --plan $appServicePlan `
  --name $apiApp `
  --runtime "DOTNETCORE:8.0"

az storage account create `
  --resource-group $resourceGroup `
  --name $storageAccount `
  --location $location `
  --sku Standard_LRS

az functionapp create `
  --resource-group $resourceGroup `
  --name $functionApp `
  --storage-account $storageAccount `
  --consumption-plan-location $location `
  --runtime dotnet-isolated `
  --functions-version 4
```

Configure the API app settings in Azure. Use values copied from Azure SQL and Service Bus, not placeholder text:

```powershell
az webapp config appsettings set `
  --resource-group $resourceGroup `
  --name $apiApp `
  --settings `
    ConnectionStrings__FjordviaDatabase="<azure-sql-connection-string>" `
    ServiceBus__ConnectionString="<service-bus-connection-string>" `
    ServiceBus__InvoiceImportQueueName=$queueName `
    Cors__AllowedOrigins__0="http://localhost:4200" `
    Database__EnsureCreatedOnStartup="true"
```

Configure the Function App settings:

```powershell
az functionapp config appsettings set `
  --resource-group $resourceGroup `
  --name $functionApp `
  --settings `
    ServiceBus__ConnectionString="<service-bus-connection-string>" `
    ServiceBus__InvoiceImportQueueName=$queueName
```

Publish and deploy from local build outputs:

```powershell
dotnet publish src/Fjordvia.Api/Fjordvia.Api.csproj --configuration Release --output ./.publish/api
dotnet publish functions/Fjordvia.Functions/Fjordvia.Functions.csproj --configuration Release --output ./.publish/functions
```

Deploy the published output with your preferred Azure workflow, such as GitHub Actions, Visual Studio publish, or `az webapp deploy` / `az functionapp deployment source config-zip` after packaging the publish folders.

After deployment:

```powershell
.\scripts\smoke-test.ps1 -BaseUrl "https://<unique-api-app-name>.azurewebsites.net"
```

For local Angular against the deployed API, update `frontend/fjordvia-web/src/environments/environment.ts` locally to point `apiBaseUrl` at the App Service URL, then run `npm start`. Do not commit environment-specific API URLs unless you intentionally add a separate deployment environment file.

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
- `Fjordvia.Infrastructure` owns persistence and infrastructure adapters: EF Core `DbContext`, SQL Server configuration, repositories, local seed data, local message publishing, and Azure Service Bus publishing.
- `Fjordvia.Functions` owns asynchronous message handling for invoice import messages in the Azure portfolio flow.

The local MVP remains simple and does not require cloud resources. The Azure portfolio path adds a Service Bus queue and Function project to demonstrate asynchronous integration handoff while keeping API contracts and local behavior stable.

## Azure Roadmap

Implemented for portfolio demonstration:

- Azure Service Bus for asynchronous integration messages
- Azure Functions isolated worker for processing invoice import messages

Future production-oriented improvements:

- Azure SQL for managed production database hosting
- Azure Key Vault for connection strings and secrets
- Azure Monitor for logs, metrics, and operational dashboards
- Azure API Management as an optional gateway for throttling, versioning, and partner access control
