$ErrorActionPreference = 'Stop'
$here = Split-Path -Parent $MyInvocation.MyCommand.Path
$root = Split-Path -Parent $here
$dist = Join-Path $root 'dist'
$tools = 'C:\Grok\Projets\_tools\winsdk\x64'
$makeappx = Join-Path $tools 'makeappx.exe'
$signtool = Join-Path $tools 'signtool.exe'
if (-not (Test-Path $makeappx)) { throw "makeappx missing: $makeappx" }

function Read-Identity {
    $map = @{}
    foreach ($line in Get-Content (Join-Path $here 'identity.txt') -Encoding UTF8) {
        $t = $line.Trim()
        if ($t.Length -eq 0 -or $t.StartsWith('#')) { continue }
        $eq = $t.IndexOf('=')
        if ($eq -lt 1) { continue }
        $map[$t.Substring(0, $eq).Trim()] = $t.Substring($eq + 1).Trim()
    }
    return $map
}

$id = Read-Identity
$store = $id['Name'] -and $id['Publisher']
if ($store) {
    $name = $id['Name']
    $publisher = $id['Publisher']
    $display = $id['PublisherDisplayName']
    if (-not $display) { $display = 'Fred Zarma' }
    $outName = 'ZTime.msix'
} else {
    $name = 'FredZarma.ZTime.Local'
    $publisher = 'CN=ZTime Local'
    $display = 'Fred Zarma'
    $outName = 'ZTime-local.msix'
}

$layout = Join-Path $env:TEMP 'ZTime-msix-layout'
if (Test-Path $layout) { Remove-Item -Recurse -Force $layout }
New-Item -ItemType Directory -Force -Path (Join-Path $layout 'Assets') | Out-Null
Copy-Item (Join-Path $dist 'ZTime.exe') (Join-Path $layout 'ZTime.exe')
Copy-Item (Join-Path $here 'assets\*.png') (Join-Path $layout 'Assets')
$manifest = Get-Content (Join-Path $here 'AppxManifest.xml') -Raw -Encoding UTF8
$manifest = $manifest.Replace('__NAME__', $name).Replace('__PUBLISHER__', $publisher).Replace('__PUBLISHER_DISPLAY__', $display)
[IO.File]::WriteAllText((Join-Path $layout 'AppxManifest.xml'), $manifest, (New-Object Text.UTF8Encoding $false))

New-Item -ItemType Directory -Force -Path $dist | Out-Null
$out = Join-Path $dist $outName
if (Test-Path $out) { Remove-Item -Force $out }
& $makeappx pack /o /h SHA256 /d $layout /p $out
if ($LASTEXITCODE -ne 0) { throw "makeappx failed: $LASTEXITCODE" }

$existing = Get-ChildItem Cert:\CurrentUser\My | Where-Object { $_.Subject -eq $publisher } | Sort-Object NotAfter -Descending | Select-Object -First 1
if (-not $existing) {
    $existing = New-SelfSignedCertificate -Type Custom -Subject $publisher -KeyUsage DigitalSignature -FriendlyName "ZTime MSIX $publisher" -CertStoreLocation Cert:\CurrentUser\My -TextExtension @('2.5.29.37={text}1.3.6.1.5.5.7.3.3') -NotAfter (Get-Date).AddYears(2)
}
& $signtool sign /fd SHA256 /sha1 $existing.Thumbprint $out
if ($LASTEXITCODE -ne 0) { throw "signtool failed: $LASTEXITCODE" }
Write-Host "Signed $out"
Write-Host "Identity Name=$name"
Write-Host "Publisher=$publisher"
Write-Host "PublisherDisplayName=$display"
if (-not $store) { Write-Host 'LOCAL package. Fill store\identity.txt with the Partner Center identity, then run Pack.ps1 again for the upload file.' }
