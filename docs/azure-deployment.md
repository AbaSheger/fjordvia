# Fjordvia Azure Deployment

This document records the current Fjordvia Azure portfolio deployment. It is a working demo deployment, not a claim of production readiness.

No secrets, passwords, tokens, connection strings, or real Azure keys are stored in this repository. Configuration examples use placeholders only.

## Live Demo

- Frontend: <https://kind-mud-042d7d90f.7.azurestaticapps.net>
- API base URL: <https://fjordvia-api-rg-2338120496.azurewebsites.net>
- Swagger: <https://fjordvia-api-rg-2338120496.azurewebsites.net/swagger>

The frontend URL serves the Angular application. The API base URL is the root URL used by the frontend for HTTP requests. The Swagger URL is the API documentation and manual endpoint tester.

## Architecture

```text
Azure Static Web Apps -> Azure App Service API -> Azure SQL Database -> Azure Service Bus -> Azure Function
```

Runtime flow:

1. The Angular dashboard runs from Azure Static Web Apps.
2. The dashboard calls the Fjordvia API hosted on Azure App Service.
3. The API reads and writes integration data in Azure SQL Database.
4. Successful invoice imports publish messages to Azure Service Bus.
5. The Azure Function consumes messages from the `invoice-imports` queue.

## Deployment Notes

### Frontend

- Service: Azure Static Web Apps
- Static Web App name: `fjordvia-web`
- Resource group: `fjordvia-rg`
- Live frontend URL: <https://kind-mud-042d7d90f.7.azurestaticapps.net>
- Deployment source: GitHub Actions from `AbaSheger/fjordvia`, `main` branch
- App location: `frontend/fjordvia-web`
- Output location: `dist/fjordvia-web/browser`
- Frontend API base URL: <https://fjordvia-api-rg-2338120496.azurewebsites.net>

### Backend API

- Service: Azure App Service
- App Service name: `fjordvia-api-rg-2338120496`
- Resource group: `fjordvia-rg`
- API base URL: <https://fjordvia-api-rg-2338120496.azurewebsites.net>
- Swagger URL: <https://fjordvia-api-rg-2338120496.azurewebsites.net/swagger>
- Note: the API uses an existing App Service plan from `passagelite-rg` because the subscription quota blocked creating a new App Service plan.

Required API app settings:

```text
ConnectionStrings__FjordviaDatabase=<azure-sql-connection-string>
ServiceBus__ConnectionString=<service-bus-connection-string>
ServiceBus__InvoiceImportQueueName=invoice-imports
Cors__AllowedOrigins__0=https://kind-mud-042d7d90f.7.azurestaticapps.net
Database__EnsureCreatedOnStartup=<true-or-false>
```

### Database

- Service: Azure SQL Database
- Azure SQL Database: `fjordvia-db`
- Azure SQL Server: `fjordvia-sql-se-2510026431`
- Resource group: `fjordvia-rg`

The real SQL password is not documented in this repository.

### Messaging

- Service: Azure Service Bus
- Namespace: `fjordvia-sb-157789427`
- Queue: `invoice-imports`
- Resource group: `fjordvia-rg`

### Function App

- Service: Azure Functions
- Function App: `fjordvia-functions-157789427`
- Runtime: .NET isolated
- Trigger: Service Bus trigger on `invoice-imports`
- Resource group: `fjordvia-rg`
- Storage account: `fjordviast157789427`

Required Function App settings:

```text
AzureWebJobsStorage=<function-storage-connection-string>
FUNCTIONS_WORKER_RUNTIME=dotnet-isolated
ServiceBus__ConnectionString=<service-bus-connection-string>
ServiceBus__InvoiceImportQueueName=invoice-imports
```

### CORS

CORS is configured so the deployed Angular frontend can call the API.

Allowed frontend origin:

```text
https://kind-mud-042d7d90f.7.azurestaticapps.net
```

## Verified Flow

The following checks were completed:

- Angular frontend loads from Azure Static Web Apps.
- Swagger endpoint returns `200 OK`.
- `/api/business-partners` returns `200 OK`.
- Invoice import returned `201 Created`.
- Integration logs show completed invoice imports.
- Service Bus queue reached `0` active messages and `0` dead-letter messages after the Azure Function consumed the message.
- Browser-to-API CORS was verified with `Access-Control-Allow-Origin` for the Static Web App URL.

## Azure Deployment Checklist

Required Azure resources:

- Resource Group: `fjordvia-rg`
- Azure SQL Database: `fjordvia-db`
- Azure SQL Server: `fjordvia-sql-se-2510026431`
- Azure Service Bus namespace: `fjordvia-sb-157789427`
- Azure Service Bus queue: `invoice-imports`
- Azure App Service: `fjordvia-api-rg-2338120496`
- Azure Function App: `fjordvia-functions-157789427`
- Storage Account for Function App: `fjordviast157789427`
- Azure Static Web App: `fjordvia-web`

Configuration requirements:

- API app settings use placeholders locally and real values only in Azure.
- Function App settings use placeholders locally and real values only in Azure.
- The frontend must point to the API base URL, not the Swagger URL.
- The API must allow the Static Web App origin through CORS.

## Example Azure CLI Commands

These commands are examples only. Replace placeholders before running them. Do not paste real secrets into source-controlled files.

```powershell
$resourceGroup = "fjordvia-rg"
$location = "<azure-region>"
$sqlServer = "fjordvia-sql-se-2510026431"
$sqlDatabase = "fjordvia-db"
$sqlAdmin = "<sql-admin-user>"
$sqlPassword = "<sql-admin-password>"
$serviceBusNamespace = "fjordvia-sb-157789427"
$queueName = "invoice-imports"
$apiApp = "fjordvia-api-rg-2338120496"
$functionApp = "fjordvia-functions-157789427"
$storageAccount = "fjordviast157789427"
$staticWebApp = "fjordvia-web"
```

Configure API app settings:

```powershell
az webapp config appsettings set `
  --resource-group $resourceGroup `
  --name $apiApp `
  --settings `
    ConnectionStrings__FjordviaDatabase="<azure-sql-connection-string>" `
    ServiceBus__ConnectionString="<service-bus-connection-string>" `
    ServiceBus__InvoiceImportQueueName=$queueName `
    Cors__AllowedOrigins__0="https://kind-mud-042d7d90f.7.azurestaticapps.net" `
    Database__EnsureCreatedOnStartup="<true-or-false>"
```

Configure Function App settings:

```powershell
az functionapp config appsettings set `
  --resource-group $resourceGroup `
  --name $functionApp `
  --settings `
    AzureWebJobsStorage="<function-storage-connection-string>" `
    FUNCTIONS_WORKER_RUNTIME="dotnet-isolated" `
    ServiceBus__ConnectionString="<service-bus-connection-string>" `
    ServiceBus__InvoiceImportQueueName=$queueName
```

Check the Service Bus queue after testing:

```powershell
az servicebus queue show `
  --resource-group $resourceGroup `
  --namespace-name $serviceBusNamespace `
  --name $queueName `
  --query "{active:countDetails.activeMessageCount, deadLetter:countDetails.deadLetterMessageCount}"
```

## Cost And Cleanup

The resource group `fjordvia-rg` contains the Fjordvia demo resources. Delete this resource group only when the live demo is no longer needed.

```powershell
az group delete --name fjordvia-rg
```

Deleting the resource group removes the Azure Static Web App, App Service-related resources in that group, Azure SQL resources, Service Bus namespace, Function App, and Function storage account resources that belong to the demo.
