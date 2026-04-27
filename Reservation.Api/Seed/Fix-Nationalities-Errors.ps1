# Fix corrupted NameAr in Nationalities.json using CodeToNameAr.json (no Arabic in this script)
param(
    [string]$DataFile = "SonoTracker.Api/Seed/Nationalities.json",
    [string]$MapFile = "SonoTracker.Api/Seed/CodeToNameAr.json"
)

$ErrorActionPreference = "Stop"
$replacementChar = [char]0xFFFD

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$dataPath = Join-Path (Get-Location) $DataFile
$mapPath = Join-Path (Get-Location) $MapFile
if (-not (Test-Path $dataPath)) { $dataPath = Join-Path $scriptDir (Split-Path -Leaf $DataFile) }
if (-not (Test-Path $mapPath)) { $mapPath = Join-Path $scriptDir (Split-Path -Leaf $MapFile) }

if (-not (Test-Path $dataPath)) { Write-Error "Not found: $DataFile"; exit 1 }
if (-not (Test-Path $mapPath)) { Write-Error "Not found: $MapFile"; exit 1 }

$mapJson = Get-Content $mapPath -Raw -Encoding UTF8
$codeToArabic = @{}
($mapJson | ConvertFrom-Json).PSObject.Properties | ForEach-Object { $codeToArabic[$_.Name] = $_.Value }

$json = Get-Content $dataPath -Raw -Encoding UTF8
$list = $json | ConvertFrom-Json

$fixed = 0
foreach ($item in $list) {
    $code = $item.Code
    $nameAr = $item.NameAr
    if ($null -ne $code -and $codeToArabic.ContainsKey($code)) {
        $needsFix = ($null -ne $nameAr -and $nameAr.IndexOf($replacementChar) -ge 0)
        if ($needsFix -or [string]::IsNullOrWhiteSpace($nameAr)) {
            $item.NameAr = $codeToArabic[$code]
            $fixed++
        }
    }
}

$json = $list | ConvertTo-Json -Depth 10 -Compress:$false
$json = $json -replace '        "CreatedAt"', '    "CreatedAt"'

$utf8NoBom = New-Object System.Text.UTF8Encoding $false
[System.IO.File]::WriteAllText((Resolve-Path $dataPath).Path, $json, $utf8NoBom)

Write-Host "Fixed $fixed NameAr values. Saved: $dataPath"
