param(
    [string]$Path = "SonoTracker.Api/Seed/Nationalities.json"
)

$ErrorActionPreference = "Stop"

# Resolve path relative to current location or script directory
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$dataPath = Join-Path (Get-Location) $Path
if (-not (Test-Path $dataPath)) {
    $dataPath = Join-Path $scriptDir (Split-Path -Leaf $Path)
}

if (-not (Test-Path $dataPath)) {
    Write-Error "File not found: $Path"
    exit 1
}

# Load JSON as objects
$json = Get-Content $dataPath -Raw -Encoding UTF8
$items = $json | ConvertFrom-Json

$reordered = @()
foreach ($item in $items) {
    # Build an ordered object with Code directly after Id and IsDeleted last
    $o = [ordered]@{
        Id           = $item.Id
        Code         = $item.Code
        NameAr       = $item.NameAr
        NameEn       = $item.NameEn
        CreatedAt    = $item.CreatedAt
        ModifiedAt   = $item.ModifiedAt
        CreatedById  = $item.CreatedById
        CreatedBy    = $item.CreatedBy
        ModifiedById = $item.ModifiedById
        ModifiedBy   = $item.ModifiedBy
        IsDeleted    = $item.IsDeleted
    }

    $reordered += New-Object PSObject -Property $o
}

# Convert back to nicely formatted JSON
$outJson = $reordered | ConvertTo-Json -Depth 10 -Compress:$false

# Save as UTF-8 without BOM
$utf8NoBom = New-Object System.Text.UTF8Encoding $false
[System.IO.File]::WriteAllText((Resolve-Path $dataPath).Path, $outJson, $utf8NoBom)

Write-Host "Reordered properties so 'Code' comes immediately after 'Id' in $Path"

