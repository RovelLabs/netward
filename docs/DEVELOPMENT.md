# Development & Contribution Guide

This guide provides instructions for configuring your development environment, running tests, and adding features to NetWard.

---

## 1. Environment Setup

### Required Tools:
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or higher.
- Visual Studio 2022 (with ".NET Desktop Development" workload) OR Visual Studio Code / Rider.
- Git.

### Verifying Toolchain:
```powershell
dotnet --version
git --version
```

---

## 2. Project Structure

```
src/
  NetWard.Core/         # Platform-independent diagnostic domain and engines
    Diagnostics/        # Multi-layer socket prober, DNS inspector, Game radar
    LocalResilience/    # DNS cache flushing, MTU & Gateway testing
    Localization/       # Primary Russian & secondary English strings
    Models/             # Domain entities, verdicts, reports
    Security/           # Telemetry redaction & sanitization
    Services/           # Preloaded service profiles (Discord, YouTube, etc.)
    Storage/            # Atomic local settings & history persistence
  NetWard.App/          # Hardware-accelerated WPF native Windows desktop GUI
    Converters/         # Status badges and color brushes
    ViewModels/         # Reactive MVVM presentation models
    MainWindow.xaml     # Graphite dark layout
  NetWard.Cli/          # Cross-platform CLI runner
tests/
  NetWard.Tests/        # xUnit tests with simulated network mockers
```

---

## 3. Routine Developer Workflows

### Compiling:
```powershell
dotnet build NetWard.sln
```

### Running All Unit Tests:
```powershell
dotnet test NetWard.sln --verbosity normal
```

### Running the Desktop GUI in Debug:
```powershell
dotnet run --project src/NetWard.App/NetWard.App.csproj
```

### Running the CLI Runner:
```powershell
# Run with simulation to test without external network:
dotnet run --project src/NetWard.Cli/NetWard.Cli.csproj -- simulate discord

# Run gaming radar:
dotnet run --project src/NetWard.Cli/NetWard.Cli.csproj -- game
```
