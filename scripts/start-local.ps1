[CmdletBinding()]
param(
    [switch]$SkipBuild,
    [switch]$RestartInfrastructure,
    [switch]$SkipAdmin,
    [switch]$SkipMobile,
    [switch]$ClearExpoCache
)

$ErrorActionPreference = "Stop"

$root = Split-Path -Parent $PSScriptRoot
$runtimeDirectory = Join-Path $root "outputs\local-runtime"
[System.IO.Directory]::CreateDirectory($runtimeDirectory) | Out-Null

$applicationPorts = @(5022, 5044, 5093, 5141, 5150, 5174, 5208, 7070, 8081)
$infrastructureServices = @("postgres", "rabbitmq", "redis", "mongo")
$dockerApplicationServices = @("auth", "catalog", "inventory", "basket", "order", "notification", "gateway", "frontend")

$apiServices = @(
    @{ Name = "auth"; Port = 5208; Project = "src\Services\Auth\Auth.Api\Auth.Api.csproj" },
    @{ Name = "catalog"; Port = 5174; Project = "src\Services\Catalog\Catalog.Api\CatalogService.csproj" },
    @{ Name = "inventory"; Port = 5141; Project = "src\Services\Inventory\Inventory.Api\InventoryService.csproj" },
    @{ Name = "basket"; Port = 5150; Project = "src\Services\Basket\Basket.Api\BasketService.csproj" },
    @{ Name = "order"; Port = 5093; Project = "src\Services\Order\Order.Api\OrderService.csproj" },
    @{ Name = "notification"; Port = 5044; Project = "src\Services\Notification\Notification.Api\NotificationService.csproj" }
)

function Write-Step([string]$Message) {
    Write-Host "`n==> $Message" -ForegroundColor Cyan
}

function Get-ListeningProcessIds([int[]]$Ports) {
    $portLookup = @{}
    foreach ($port in $Ports) {
        $portLookup[$port] = $true
    }

    $processIds = foreach ($line in (netstat -ano -p tcp)) {
        if ($line -notmatch '^\s*TCP\s+(\S+):(\d+)\s+\S+\s+LISTENING\s+(\d+)\s*$') {
            continue
        }

        $port = [int]$Matches[2]
        if ($portLookup.ContainsKey($port)) {
            [int]$Matches[3]
        }
    }

    return @($processIds | Sort-Object -Unique)
}

function Stop-LocalProcesses {
    $processIds = Get-ListeningProcessIds $applicationPorts
    if ($processIds.Count -eq 0) {
        Write-Host "No previous local project process was found."
        return
    }

    foreach ($processId in $processIds) {
        $process = Get-Process -Id $processId -ErrorAction SilentlyContinue
        if ($null -eq $process) {
            continue
        }

        Write-Host "Stopping $($process.ProcessName) (PID $processId)..."
        Stop-Process -Id $processId -Force
    }

    Start-Sleep -Seconds 2
}

function Test-TcpPort([int]$Port, [int]$TimeoutMilliseconds = 1000) {
    $client = [System.Net.Sockets.TcpClient]::new()
    try {
        $task = $client.ConnectAsync("127.0.0.1", $Port)
        return $task.Wait($TimeoutMilliseconds) -and $client.Connected
    }
    catch {
        return $false
    }
    finally {
        $client.Dispose()
    }
}

function Wait-TcpPort([string]$Name, [int]$Port, [int]$TimeoutSeconds = 60) {
    $deadline = [DateTime]::UtcNow.AddSeconds($TimeoutSeconds)
    while ([DateTime]::UtcNow -lt $deadline) {
        if (Test-TcpPort $Port) {
            Write-Host "$Name is ready on port $Port." -ForegroundColor Green
            return
        }

        Start-Sleep -Milliseconds 500
    }

    throw "$Name did not open port $Port within $TimeoutSeconds seconds."
}

function Start-LoggedProcess(
    [string]$Name,
    [string]$FilePath,
    [string[]]$ArgumentList,
    [string]$WorkingDirectory
) {
    $outputPath = Join-Path $runtimeDirectory "$Name.log"
    $errorPath = Join-Path $runtimeDirectory "$Name.error.log"
    [System.IO.File]::WriteAllText($outputPath, "")
    [System.IO.File]::WriteAllText($errorPath, "")

    return Start-Process `
        -FilePath $FilePath `
        -ArgumentList $ArgumentList `
        -WorkingDirectory $WorkingDirectory `
        -RedirectStandardOutput $outputPath `
        -RedirectStandardError $errorPath `
        -WindowStyle Hidden `
        -PassThru
}

function Show-StartupFailure([string]$Name) {
    $errorPath = Join-Path $runtimeDirectory "$Name.error.log"
    $outputPath = Join-Path $runtimeDirectory "$Name.log"

    Write-Host "`n$Name failed to start. Last log lines:" -ForegroundColor Red
    Get-Content $errorPath -Tail 30 -ErrorAction SilentlyContinue
    Get-Content $outputPath -Tail 30 -ErrorAction SilentlyContinue
}

Set-Location $root

Write-Step "Stopping previous local project processes"
Stop-LocalProcesses

Write-Step "Keeping API containers disabled"
& docker compose stop @dockerApplicationServices
if ($LASTEXITCODE -ne 0) {
    throw "Docker application containers could not be stopped."
}

Write-Step "Starting Docker infrastructure"
if ($RestartInfrastructure) {
    & docker compose restart @infrastructureServices
}

& docker compose up -d @infrastructureServices
if ($LASTEXITCODE -ne 0) {
    throw "Docker infrastructure could not be started."
}

Wait-TcpPort "PostgreSQL" 5432
Wait-TcpPort "RabbitMQ" 5672
Wait-TcpPort "Redis" 6379
Wait-TcpPort "MongoDB" 27017

if (-not $SkipBuild) {
    Write-Step "Building the solution"
    & dotnet build (Join-Path $root "OpsFlow.slnx") -c Release
    if ($LASTEXITCODE -ne 0) {
        throw "Solution build failed. APIs were not started."
    }
}

Write-Step "Starting local APIs"
foreach ($service in $apiServices) {
    $projectPath = Join-Path $root $service.Project
    Start-LoggedProcess `
        -Name $service.Name `
        -FilePath "dotnet" `
        -ArgumentList @("run", "--project", $projectPath, "-c", "Release", "--no-build", "--launch-profile", "http") `
        -WorkingDirectory $root | Out-Null
}

foreach ($service in $apiServices) {
    try {
        Wait-TcpPort $service.Name $service.Port
    }
    catch {
        Show-StartupFailure $service.Name
        throw
    }
}

Write-Step "Starting Gateway"
$gatewayProject = Join-Path $root "src\Services\Gateway\Gateway.Api\Gateway.csproj"
Start-LoggedProcess `
    -Name "gateway" `
    -FilePath "dotnet" `
    -ArgumentList @("run", "--project", $gatewayProject, "-c", "Release", "--no-build", "--launch-profile", "http") `
    -WorkingDirectory $root | Out-Null
Wait-TcpPort "gateway" 5022

if (-not $SkipAdmin) {
    Write-Step "Starting admin panel"
    $pythonCommand = Get-Command "python" -ErrorAction Stop
    Start-LoggedProcess `
        -Name "admin-panel" `
        -FilePath $pythonCommand.Source `
        -ArgumentList @("-m", "http.server", "7070", "--directory", (Join-Path $root "admin-panel")) `
        -WorkingDirectory $root | Out-Null
    Wait-TcpPort "admin panel" 7070
}

if (-not $SkipMobile) {
    Write-Step "Starting Expo"
    $mobileArguments = @("run", "start", "--")
    if ($ClearExpoCache) {
        $mobileArguments += "--clear"
    }

    Start-LoggedProcess `
        -Name "mobile" `
        -FilePath "npm.cmd" `
        -ArgumentList $mobileArguments `
        -WorkingDirectory (Join-Path $root "mobile\opsflow-mobile") | Out-Null
    Wait-TcpPort "Expo" 8081 90
}

Write-Step "Verifying HTTP endpoints"
$checks = @(
    @{ Name = "Gateway"; Url = "http://localhost:5022/services" },
    @{ Name = "Catalog through Gateway"; Url = "http://localhost:5022/proxy/catalog/products" }
)

if (-not $SkipAdmin) {
    $checks += @{ Name = "Admin panel"; Url = "http://localhost:7070" }
}

if (-not $SkipMobile) {
    $checks += @{ Name = "Mobile web"; Url = "http://localhost:8081" }
}

foreach ($check in $checks) {
    $response = Invoke-WebRequest -Uri $check.Url -UseBasicParsing -TimeoutSec 15
    if ($response.StatusCode -ne 200) {
        throw "$($check.Name) returned HTTP $($response.StatusCode)."
    }

    Write-Host "$($check.Name): HTTP 200" -ForegroundColor Green
}

Write-Host "`nLocal environment is ready." -ForegroundColor Green
Write-Host "Gateway:     http://localhost:5022"
if (-not $SkipAdmin) {
    Write-Host "Admin panel: http://localhost:7070"
}
if (-not $SkipMobile) {
    Write-Host "Mobile web:  http://localhost:8081"
}
Write-Host "Logs:        $runtimeDirectory"
