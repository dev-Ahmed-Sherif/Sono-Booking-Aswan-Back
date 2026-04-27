param(
    [string]$Path = "SonoTracker.Api/Seed/Nationalities.json"
)

Write-Host "Fixing Arabic NameAr values in $Path"

if (-not (Test-Path $Path)) {
    Write-Error "File not found: $Path"
    exit 1
}

function Fix-Mojibake {
    param(
        [string]$Value
    )

    # Convert mojibake (UTF-8 decoded as Latin-1/Windows-1252) back to proper UTF-8 Arabic
    $bytes = New-Object byte[] ($Value.Length)
    for ($i = 0; $i -lt $Value.Length; $i++) {
        $bytes[$i] = [byte][char]$Value[$i]
    }
    return [System.Text.Encoding]::UTF8.GetString($bytes)
}

$content = Get-Content $Path -Raw -Encoding UTF8

$pattern = '"NameAr"\s*:\s*"([^"]*)"'
$regex = [System.Text.RegularExpressions.Regex]::new($pattern)

$fixed = $regex.Replace($content, {
    param($m)
    $orig = $m.Groups[1].Value

    # Detect mojibake by checking for extended Latin-1 bytes (0xC0-0xFF).
    # Correct Arabic letters are outside this range (U+0600+), so they will be left as-is.
    $hasExtendedLatin = $false
    foreach ($ch in $orig.ToCharArray()) {
        $code = [int][char]$ch
        if ($code -ge 192 -and $code -le 255) {
            $hasExtendedLatin = $true
            break
        }
    }

    if ($hasExtendedLatin) {
        $newVal = Fix-Mojibake -Value $orig
        '"NameAr": "' + $newVal + '"'
    } else {
        $m.Value
    }
})

Set-Content -Path $Path -Value $fixed -Encoding UTF8

Write-Host "Completed fixing NameAr values."

