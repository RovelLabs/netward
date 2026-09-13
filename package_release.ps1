# NetWard Automated Release Packager
param (
    [string]$Version = "0.1.0"
)

$ErrorActionPreference = "Stop"
Write-Host "[BUILD] Packaging NetWard v$Version Release Artifacts..." -ForegroundColor Cyan

$distDir = "dist\release"
if (Test-Path $distDir) {
    Remove-Item -Recurse -Force $distDir
}
New-Item -ItemType Directory -Force -Path $distDir | Out-Null

$appTemp = "dist\app-temp"
if (Test-Path $appTemp) {
    Remove-Item -Recurse -Force $appTemp
}

Write-Host "1. Publishing NetWard.App (Windows x64 Standalone)..." -ForegroundColor Yellow
dotnet publish src/NetWard.App/NetWard.App.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o $appTemp

Write-Host "2. Adding installer scripts and creating Desktop Zip Archive..." -ForegroundColor Yellow
Copy-Item installer\install.ps1 $appTemp\
Copy-Item installer\install.cmd $appTemp\
$zipPath = "$distDir\NetWard-v$Version-Windows-x64.zip"
Compress-Archive -Path "$appTemp\*" -DestinationPath $zipPath -CompressionLevel Optimal

Write-Host "3. Publishing NetWard.Cli (Windows x64 Standalone)..." -ForegroundColor Yellow
$cliTemp = "dist\cli-temp"
dotnet publish src/NetWard.Cli/NetWard.Cli.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o $cliTemp
Copy-Item "$cliTemp\NetWard.Cli.exe" "$distDir\netward-cli-v$Version-windows-x64.exe"

Write-Host "4. Computing Cryptographic SHA-256 Checksums..." -ForegroundColor Yellow
$checksumsFile = "$distDir\checksums.txt"
$hashes = Get-FileHash -Algorithm SHA256 "$distDir\*.zip", "$distDir\*.exe"
$hashLines = $hashes | ForEach-Object { "$($_.Hash)  $($_.Path | Split-Path -Leaf)" }
$hashLines | Out-File -Encoding utf8 $checksumsFile

Write-Host "[OK] Release Packaging Complete!" -ForegroundColor Green
Write-Host "Generated Artifacts:" -ForegroundColor Cyan
Get-Content $checksumsFile
