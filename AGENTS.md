\# AGENTS.md



\## Project



Fjordvia is a Nordic-inspired B2B SaaS portfolio project for cloud-based business system integrations.



It connects ERP, CRM, warehouse, and finance systems through REST APIs, data transformation, retry handling, and integration logs.



The goal is to demonstrate practical skills for roles involving .NET, Azure-oriented architecture, REST APIs, SQL Server, integrations, business systems, testing, and clean backend design.



\## MVP Scope



Build the backend MVP only.



Required features:



\- Customer or business partner API

\- Invoice import API

\- ERP-to-accounting/CRM data mapping

\- Integration logs with statuses: Pending, Completed, Failed

\- Retry failed integration endpoint

\- Seed data for local testing

\- Unit tests for mapping logic and retry logic

\- Docker Compose for local SQL Server

\- GitHub Actions CI

\- Professional README with business problem, setup, architecture, API examples, and Azure roadmap



\## Tech Stack



Use:



\- .NET 8

\- ASP.NET Core Web API

\- SQL Server

\- Entity Framework Core

\- Swagger/OpenAPI

\- xUnit

\- Docker Compose

\- GitHub Actions



\## Solution Structure



Create and follow this structure:



\- src/Fjordvia.Api

\- src/Fjordvia.Core

\- src/Fjordvia.Infrastructure

\- tests/Fjordvia.Tests



\## Architecture Rules



Use a clean layered backend structure.



Separate:



\- API/controllers

\- DTOs

\- domain models

\- services/business logic

\- repositories/persistence

\- integration mapping logic

\- error handling



Keep the design simple and portfolio-ready. Do not over-engineer the MVP with microservices.



\## Coding Rules



\- Do not add secrets or fake API keys.

\- Do not build a frontend yet.

\- Do not make the project specific to Visma, Control Edge, or any real company.

\- Use realistic but fictional business names and sample data.

\- Keep the code readable and testable.

\- Prefer clear naming over clever abstractions.

\- Add validation and central error handling.

\- Make sure `dotnet build` passes.

\- Make sure `dotnet test` passes.



\## Azure Implementation Scope



\### Phase 1: Current Local MVP



The current MVP is local-first and should include:



\- ASP.NET Core Web API

\- SQL Server through Docker Compose

\- Entity Framework Core

\- Swagger/OpenAPI

\- xUnit tests

\- GitHub Actions CI



Phase 1 does not require live Azure resources.



\### Phase 2: Required Azure Portfolio Version



The stronger portfolio-ready version should implement these four main Azure services:



1. Azure App Service

   \- Host the ASP.NET Core API.

2. Azure SQL Database

   \- Use as the production database instead of local SQL Server.

3. Azure Service Bus

   \- Queue integration messages asynchronously, such as invoice import events.

4. Azure Functions

   \- Process Service Bus messages and update integration logs.



These four services are the primary Azure services Fjordvia should include for the portfolio-ready version.



\### Phase 3: Recommended Enhancements



Recommended later additions:



\- Azure Key Vault for secrets and connection strings

\- Azure Monitor or Application Insights for logs, errors, request tracing, and observability



\### Optional Future Addition



Azure API Management is optional because it may add cost. It can later be used for:



\- API versioning

\- Rate limiting

\- Partner access

\- Request policies



\## Done Means



A task is done only when:



\- The solution builds successfully

\- Tests pass

\- Main API endpoints are documented in Swagger

\- README explains how to run the project locally

\- README explains the business problem and architecture

\- No secrets are committed

