[CmdletBinding()]
param()

$ErrorActionPreference = "Stop"
$ports = @(5022, 5044, 5093, 5141, 5150, 5174, 5208, 7070, 8081)
$portLookup = @{}
foreach ($port in $ports) {
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

foreach ($processId in @($processIds | Sort-Object -Unique)) {
    $process = Get-Process -Id $processId -ErrorAction SilentlyContinue
    if ($null -eq $process) {
        continue
    }

    Write-Host "Stopping $($process.ProcessName) (PID $processId)..."
    Stop-Process -Id $processId -Force
}

Write-Host "Local APIs, Gateway, admin panel and Expo were stopped."
Write-Host "Docker infrastructure was left running."
