param(
    # По умолчанию версия берётся из <Version> в CircleSearch.App.csproj (единый источник правды)
    [string]$Version
)

$ErrorActionPreference = "Stop"
$RootDir = Split-Path -Parent $PSScriptRoot
$AppProject = Join-Path $RootDir "src\CircleSearch.App\CircleSearch.App.csproj"
$PublishDir = Join-Path $RootDir "publish\win-x64"
$DistDir = Join-Path $RootDir "dist"

if ([string]::IsNullOrWhiteSpace($Version)) {
    $Version = ([xml](Get-Content $AppProject -Raw)).Project.PropertyGroup.Version | Where-Object { $_ } | Select-Object -First 1
    if ([string]::IsNullOrWhiteSpace($Version)) { throw "Version not found in $AppProject" }
}
# MSI требует версию формата major.minor.build[.revision]
$MsiVersion = if (($Version -split '\.').Count -lt 4) { "$Version.0" } else { $Version }

Write-Host "=== 1. Running Unit Tests ===" -ForegroundColor Cyan
dotnet run --project "$RootDir\tests\CircleSearch.Tests\CircleSearch.Tests.csproj" -c Release

Write-Host "=== 2. Publishing Framework-Dependent Win-x64 Release (v$Version) ===" -ForegroundColor Cyan
if (Test-Path $PublishDir) { Remove-Item $PublishDir -Recurse -Force }
if (!(Test-Path $DistDir)) { New-Item -ItemType Directory -Path $DistDir | Out-Null }

dotnet publish $AppProject `
    -c Release `
    -r win-x64 `
    --self-contained false `
    -p:PublishSingleFile=false `
    -p:Version=$Version `
    -p:DebugType=None `
    -p:DebugSymbols=false `
    -o $PublishDir

Write-Host "=== 3. Creating Portable ZIP Archive ===" -ForegroundColor Cyan
$ZipPath = Join-Path $DistDir "CircleSearch-v$Version-win-x64-portable.zip"
if (Test-Path $ZipPath) { Remove-Item $ZipPath -Force }
Compress-Archive -Path "$PublishDir\*" -DestinationPath $ZipPath
Write-Host "Portable ZIP created: $ZipPath" -ForegroundColor Green

Write-Host "=== 4. Building Windows Installer (.msi) via WiX ===" -ForegroundColor Cyan
if (-not (Get-Command "wix" -ErrorAction SilentlyContinue)) {
    Write-Host "Installing WiX Toolset dotnet tool..." -ForegroundColor Yellow
    dotnet tool install --global wix
}

wix eula accept wix7 | Out-Null

$MsiPath = Join-Path $DistDir "CircleSearch-v$Version-win-x64.msi"
if (Test-Path $MsiPath) { Remove-Item $MsiPath -Force }
wix build "$PSScriptRoot\Product.wxs" -d PublishDir="$PublishDir" -d MsiVersion="$MsiVersion" -arch x64 -o "$MsiPath"
if (-not (Test-Path $MsiPath)) {
    throw "Failed to create MSI installer at $MsiPath"
}
Write-Host "MSI Installer created: $MsiPath" -ForegroundColor Green
