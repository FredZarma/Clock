$ErrorActionPreference = 'Stop'
$here = Split-Path -Parent $MyInvocation.MyCommand.Path
$fw = 'C:\Windows\Microsoft.NET\Framework64\v4.0.30319'
$csc = Join-Path $fw 'csc.exe'
$wpf = Join-Path $fw 'WPF'

$dist = Join-Path $here 'dist'
New-Item -ItemType Directory -Force -Path $dist | Out-Null

Get-Process -Name 'ZTime' -ErrorAction SilentlyContinue | Where-Object {
    $_.Path -and (
        $_.Path -like '*\ZTime\ZTime.exe' -or
        $_.Path -like '*\ZTime\dist\ZTime.exe' -or
        $_.Path -like '*\Desktop\ZTime.exe'
    )
} | Stop-Process -Force -ErrorAction SilentlyContinue
Start-Sleep -Milliseconds 300

& $csc /nologo /optimize+ /target:exe /platform:x64 `
  /r:"$wpf\WindowsBase.dll" `
  /r:"$wpf\PresentationCore.dll" `
  /r:"$wpf\PresentationFramework.dll" `
  /r:"$fw\System.Xaml.dll" `
  /out:"$here\RenderIcon.exe" `
  "$here\RenderIcon.cs"
if ($LASTEXITCODE -ne 0) { throw "RenderIcon compile failed" }

& "$here\RenderIcon.exe" $here
if ($LASTEXITCODE -ne 0) { throw "RenderIcon failed" }
Remove-Item "$here\RenderIcon.exe" -ErrorAction SilentlyContinue

$iconArg = @()
if (Test-Path "$here\ztime.ico") { $iconArg = @("/win32icon:$here\ztime.ico") }

& $csc /nologo /optimize+ /codepage:65001 /target:winexe /platform:x64 `
  /win32manifest:"$here\app.manifest" `
  @iconArg `
  /r:"$wpf\WindowsBase.dll" `
  /r:"$wpf\PresentationCore.dll" `
  /r:"$wpf\PresentationFramework.dll" `
  /r:"$fw\System.Xaml.dll" `
  /r:Microsoft.CSharp.dll `
  /r:System.Core.dll `
  /out:"$dist\ZTime.exe" `
  "$here\Program.cs"
if ($LASTEXITCODE -ne 0) { throw "ZTime compile failed" }

Write-Host "Built $dist\ZTime.exe"

if (Test-Path "$here\ztime.ico") {
    Copy-Item -Force "$here\ztime.ico" (Join-Path $dist 'ztime.ico')
}

$installerName = 'ZTime Installer.exe'
$installerPath = Join-Path $dist $installerName
Get-Process | Where-Object {
    $_.Path -and (
        $_.Path -eq (Join-Path $here 'Setup.exe') -or
        $_.Path -eq (Join-Path $dist 'Setup.exe') -or
        $_.Path -eq (Join-Path $dist 'ZTime-Setup.exe') -or
        $_.Path -eq $installerPath -or
        $_.Path -like '*\Program Files\ZTime\Setup.exe' -or
        $_.Path -like '*\Program Files\ZTime\ZTime Installer.exe'
    )
} | Stop-Process -Force -ErrorAction SilentlyContinue

$res = @()
if (Test-Path "$dist\ZTime.exe") { $res += "/resource:$dist\ZTime.exe,ZTime.exe" }
if (Test-Path "$dist\ztime.ico") { $res += "/resource:$dist\ztime.ico,ztime.ico" }

& $csc /nologo /optimize+ /codepage:65001 /target:winexe /platform:x64 `
  /win32manifest:"$here\setup.manifest" `
  @iconArg `
  @res `
  /r:"$wpf\WindowsBase.dll" `
  /r:"$wpf\PresentationCore.dll" `
  /r:"$wpf\PresentationFramework.dll" `
  /r:"$fw\System.Xaml.dll" `
  /r:Microsoft.CSharp.dll `
  /r:System.Core.dll `
  /r:System.Windows.Forms.dll `
  /out:$installerPath `
  "$here\Setup.cs"
if ($LASTEXITCODE -ne 0) { throw "Installer compile failed" }

foreach ($old in @('Setup.exe', 'ZTime-Setup.exe')) {
    $p = Join-Path $dist $old
    if (Test-Path $p) { Remove-Item -Force $p }
}
Write-Host "Built $installerPath"

$distZTime = Join-Path $dist 'ZTime.exe'
([wmiclass]'Win32_Process').Create($distZTime) | Out-Null
Write-Host "Launched $distZTime"
