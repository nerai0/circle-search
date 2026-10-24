$ErrorActionPreference = "Stop"
$RootDir = Split-Path -Parent $PSScriptRoot
$PublishDir = Join-Path $RootDir "publish\win-x64"
$DistDir = Join-Path $RootDir "dist"

Write-Host "=== 1. Running Unit Tests ===" -ForegroundColor Cyan
dotnet run --project "$RootDir\tests\CircleSearch.Tests\CircleSearch.Tests.csproj" -c Release

Write-Host "=== 2. Publishing Self-Contained Win-x64 Release ===" -ForegroundColor Cyan
if (Test-Path $PublishDir) { Remove-Item $PublishDir -Recurse -Force }
if (!(Test-Path $DistDir)) { New-Item -ItemType Directory -Path $DistDir | Out-Null }

dotnet publish "$RootDir\src\CircleSearch.App\CircleSearch.App.csproj" `
    -c Release `
    -r win-x64 `
    --self-contained true `
    -p:PublishSingleFile=false `
    -p:DebugType=None `
    -p:DebugSymbols=false `
    -o $PublishDir

Write-Host "=== 3. Creating Portable ZIP Archive ===" -ForegroundColor Cyan
$ZipPath = Join-Path $DistDir "CircleSearch-v1.0.0-win-x64-portable.zip"
if (Test-Path $ZipPath) { Remove-Item $ZipPath -Force }
Compress-Archive -Path "$PublishDir\*" -DestinationPath $ZipPath
Write-Host "Portable ZIP created: $ZipPath" -ForegroundColor Green

Write-Host "=== 4. Building Windows Installer (.msi) via WiX ===" -ForegroundColor Cyan
if (-not (Get-Command "wix" -ErrorAction SilentlyContinue)) {
    Write-Host "Installing WiX Toolset dotnet tool..." -ForegroundColor Yellow
    dotnet tool install --global wix
}

wix eula accept wix7 | Out-Null

$MsiPath = Join-Path $DistDir "CircleSearch-v1.0.0-win-x64.msi"
if (Test-Path $MsiPath) { Remove-Item $MsiPath -Force }
wix build "$PSScriptRoot\Product.wxs" -d PublishDir="$PublishDir" -arch x64 -o "$MsiPath"
if (-not (Test-Path $MsiPath)) {
    throw "Failed to create MSI installer at $MsiPath"
}
Write-Host "MSI Installer created: $MsiPath" -ForegroundColor Green
