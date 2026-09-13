@echo off
title Launching NetWard...
echo ==============================================
echo        Starting NetWard Desktop App...
echo ==============================================
dotnet run --project src\NetWard.App\NetWard.App.csproj
if errorlevel 1 (
    echo.
    echo Failed to start NetWard. Make sure .NET 8.0 SDK is installed.
    pause
)
