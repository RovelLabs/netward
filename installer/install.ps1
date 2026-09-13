# NetWard Windows Installer (PowerShell)
# Installs NetWard to %LOCALAPPDATA%\Programs\NetWard (No Admin Rights Required!)

$ErrorActionPreference = "Stop"

Write-Host "=========================================" -ForegroundColor Cyan
Write-Host "       NetWard Windows Installer         " -ForegroundColor Cyan
Write-Host "=========================================" -ForegroundColor Cyan

$installDir = "$env:LOCALAPPDATA\Programs\NetWard"
$sourceDir = $PSScriptRoot

Write-Host "[1/4] Preparing target directory: $installDir" -ForegroundColor Yellow
if (-not (Test-Path $installDir)) {
    New-Item -ItemType Directory -Force -Path $installDir | Out-Null
}

Write-Host "[2/4] Copying program files..." -ForegroundColor Yellow
$filesToCopy = Get-ChildItem -Path $sourceDir -Exclude "install.ps1", "install.cmd"
foreach ($file in $filesToCopy) {
    Copy-Item -Path $file.FullName -Destination $installDir -Recurse -Force
}

Write-Host "[3/4] Creating Start Menu shortcut..." -ForegroundColor Yellow
$startMenuDir = "$env:APPDATA\Microsoft\Windows\Start Menu\Programs\NetWard"
if (-not (Test-Path $startMenuDir)) {
    New-Item -ItemType Directory -Force -Path $startMenuDir | Out-Null
}

$wshShell = New-Object -ComObject WScript.Shell
$shortcut = $wshShell.CreateShortcut("$startMenuDir\NetWard.lnk")
$shortcut.TargetPath = "$installDir\NetWard.App.exe"
$shortcut.WorkingDirectory = $installDir
$shortcut.Description = "NetWard — Internet Resilience & Diagnostics Companion"
$shortcut.Save()

# Desktop Shortcut
$desktopShortcut = $wshShell.CreateShortcut("$([Environment]::GetFolderPath('Desktop'))\NetWard.lnk")
$desktopShortcut.TargetPath = "$installDir\NetWard.App.exe"
$desktopShortcut.WorkingDirectory = $installDir
$desktopShortcut.Description = "NetWard — Internet Resilience & Diagnostics Companion"
$desktopShortcut.Save()

Write-Host "[4/4] Creating Uninstaller..." -ForegroundColor Yellow
$uninstallScript = @"
@echo off
echo Uninstalling NetWard...
rmdir /s /q "$startMenuDir" 2>nul
del "$([Environment]::GetFolderPath('Desktop'))\NetWard.lnk" 2>nul
rmdir /s /q "$installDir" 2>nul
echo NetWard uninstalled successfully.
pause
"@
Set-Content -Path "$installDir\uninstall.cmd" -Value $uninstallScript

Write-Host "`n[OK] NetWard has been successfully installed to your PC!" -ForegroundColor Green
Write-Host "You can launch it from your Desktop or Start Menu." -ForegroundColor Cyan
Write-Host "To uninstall, run: $installDir\uninstall.cmd`n" -ForegroundColor DarkGray
