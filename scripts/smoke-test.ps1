param(
    [string]$BaseUrl = "http://localhost:5131"
)

$ErrorActionPreference = "Stop"
$script:Failures = 0

function Write-Pass {
    param([string]$Message)
    Write-Host "PASS: $Message" -ForegroundColor Green
}

function Write-Fail {
    param(
        [string]$Message,
        [string]$Detail = $null
    )

    $script:Failures += 1
    Write-Host "FAIL: $Message" -ForegroundColor Red
    if ($Detail) {
        Write-Host "      $Detail" -ForegroundColor DarkRed
    }
}

function Write-Skip {
    param([string]$Message)
    Write-Host "PASS: $Message" -ForegroundColor Yellow
}

function Invoke-Step {
    param(
        [string]$Name,
        [scriptblock]$Action
    )

    try {
        & $Action
        Write-Pass $Name
    }
    catch {
        Write-Fail $Name $_.Exception.Message
    }
}

function As-Array {
    param($Value)

    if ($null -eq $Value) {
        return @()
    }

    return @($Value | ForEach-Object { $_ })
}

$BaseUrl = $BaseUrl.TrimEnd("/")
Write-Host "Fjordvia smoke test"
Write-Host "Base URL: $BaseUrl"
Write-Host ""

$script:Partners = @()
$script:PartnersJson = $null
$script:SelectedPartner = $null
$script:SelectedPartnerId = $null
$script:InvoiceImport = $null
$script:Logs = @()
$script:FailedLog = $null

Invoke-Step "API is reachable" {
    $null = Invoke-RestMethod -Method Get -Uri "$BaseUrl/api/business-partners"
}

Invoke-Step "Get business partners" {
    $partnersResponse = Invoke-WebRequest -Uri "$BaseUrl/api/business-partners" -UseBasicParsing
    $script:PartnersJson = $partnersResponse.Content
    $parsedPartners = $script:PartnersJson | ConvertFrom-Json
    $script:Partners = @($parsedPartners | ForEach-Object { $_ })

    if ($script:Partners.Count -eq 1 -and $script:Partners[0] -is [array]) {
        $script:Partners = @($script:Partners[0] | ForEach-Object { $_ })
    }

    if ($script:Partners.Count -eq 0) {
        throw "No business partners returned."
    }
}

Invoke-Step "Pick first business partner ID" {
    $script:SelectedPartner = $script:Partners[0]
    $script:SelectedPartnerId = $script:SelectedPartner.id
    if ([string]::IsNullOrWhiteSpace($script:SelectedPartnerId)) {
        Write-Host "Business partners raw JSON:"
        Write-Host $script:PartnersJson
        throw "The first business partner did not include an id."
    }

    Write-Host "      Selected partner: $($script:SelectedPartner.name) ($script:SelectedPartnerId)"
}

Invoke-Step "Import sample invoice using selected partner" {
    if ($null -eq $script:SelectedPartner) {
        throw "No selected partner is available."
    }

    $today = Get-Date
    $invoiceDate = $today.ToString("yyyy-MM-dd")
    $dueDate = $today.AddDays(14).ToString("yyyy-MM-dd")
    $invoiceNumber = "SMOKE-$($today.ToString("yyyyMMddHHmmss"))"

    $body = @{
        externalInvoiceNumber = $invoiceNumber
        customerName = $script:SelectedPartner.name
        customerOrganizationNumber = $script:SelectedPartner.organizationNumber
        customerEmail = $script:SelectedPartner.email
        countryCode = $script:SelectedPartner.countryCode
        currency = "SEK"
        invoiceDate = $invoiceDate
        dueDate = $dueDate
        lines = @(
            @{
                description = "Smoke test integration service"
                quantity = 1
                unitPrice = 1000
            }
        )
    } | ConvertTo-Json -Depth 5

    $script:InvoiceImport = Invoke-RestMethod `
        -Method Post `
        -Uri "$BaseUrl/api/invoices/import" `
        -ContentType "application/json" `
        -Body $body

    if ([string]::IsNullOrWhiteSpace($script:InvoiceImport.invoice.id)) {
        throw "Invoice import response did not include an invoice id."
    }

    Write-Host "      Imported invoice: $($script:InvoiceImport.invoice.externalInvoiceNumber)"
}

Invoke-Step "Get integration logs" {
    $script:Logs = As-Array (Invoke-RestMethod -Method Get -Uri "$BaseUrl/api/integration-logs")
    if ($script:Logs.Count -eq 0) {
        throw "No integration logs were returned."
    }
}

Invoke-Step "Find failed integration log if one exists" {
    $script:FailedLog = $script:Logs | Where-Object { $_.status -eq "Failed" } | Select-Object -First 1
    if ($null -eq $script:FailedLog) {
        Write-Host "      No failed integration log found; retry step will be skipped."
    }
    else {
        Write-Host "      Failed log found: $($script:FailedLog.id)"
    }
}

if ($null -eq $script:FailedLog) {
    Write-Skip "Retry failed integration log skipped because no failed log exists"
}
else {
    Invoke-Step "Retry failed integration log" {
        $retryResult = Invoke-RestMethod -Method Post -Uri "$BaseUrl/api/integration-logs/$($script:FailedLog.id)/retry"
        if ($retryResult.status -ne "Pending") {
            throw "Expected retried log status to be Pending, got '$($retryResult.status)'."
        }
    }
}

Write-Host ""
if ($script:Failures -gt 0) {
    Write-Host "Smoke test failed with $script:Failures failure(s)." -ForegroundColor Red
    exit 1
}

Write-Host "Smoke test passed." -ForegroundColor Green
exit 0
