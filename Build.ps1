$ErrorActionPreference = 'Stop'
$here = Split-Path -Parent $MyInvocation.MyCommand.Path
$fw = 'C:\Windows\Microsoft.NET\Framework64\v4.0.30319'
$csc = Join-Path $fw 'csc.exe'
$wpf = Join-Path $fw 'WPF'

Get-Process -Name 'Clock' -ErrorAction SilentlyContinue | Where-Object {
    $_.Path -and (
        $_.Path -like '*\Clock\Clock.exe' -or
        $_.Path -like '*\Clock.exe' -or
        $_.Path -like '*\Desktop\Clock.exe'
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
if (Test-Path "$here\clock.ico") { $iconArg = @("/win32icon:$here\clock.ico") }

& $csc /nologo /optimize+ /codepage:65001 /target:winexe /platform:x64 `
  /win32manifest:"$here\app.manifest" `
  @iconArg `
  /r:"$wpf\WindowsBase.dll" `
  /r:"$wpf\PresentationCore.dll" `
  /r:"$wpf\PresentationFramework.dll" `
  /r:"$fw\System.Xaml.dll" `
  /r:Microsoft.CSharp.dll `
  /r:System.Core.dll `
  /out:"$here\Clock.exe" `
  "$here\Program.cs"
if ($LASTEXITCODE -ne 0) { throw "Clock compile failed" }

Write-Host "Built $here\Clock.exe"

$dist = Join-Path $here 'dist'
New-Item -ItemType Directory -Force -Path $dist | Out-Null
Copy-Item -Force "$here\Clock.exe" (Join-Path $dist 'Clock.exe')
if (Test-Path "$here\clock.ico") {
    Copy-Item -Force "$here\clock.ico" (Join-Path $dist 'clock.ico')
}

$desktop = [Environment]::GetFolderPath('Desktop')
$deskExe = Join-Path $desktop 'Clock.exe'
Copy-Item -Force "$here\Clock.exe" $deskExe
Write-Host "Copied $deskExe"

([wmiclass]'Win32_Process').Create($deskExe) | Out-Null
Write-Host "Launched Clock"
