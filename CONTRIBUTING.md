# Contributing to NetWard

Thank you for your interest in contributing to **NetWard**! NetWard is an open-source, local-first internet resilience and diagnostic companion.

## Principles for Contributors

1. **Zero Ads, Zero Tracking, Zero Bullshit:** Never introduce third-party trackers, analytics SDKs, or advertisements.
2. **Local-First:** All features must work offline or locally without requiring central cloud infrastructure.
3. **Legal Integrity:** Features must strictly remain diagnostic, measurement, and local resilience tools. Do not submit code that hosts prohibited proxy networks or circumvents legal restrictions.
4. **Data Honesty:** Never falsify diagnostic metrics, latency, or server statuses.

---

## Development Setup

### Prerequisites
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or higher.
- Windows 10/11 (for WPF Desktop application) or Linux/macOS (for `NetWard.Core` & `NetWard.Cli`).
- Git.

### Clone & Build
```bash
git clone https://github.com/RovelLabs/new-prpect.git
cd new-prpect

# Build entire solution
dotnet build

# Run unit tests
dotnet test

# Run CLI runner in simulated mode
dotnet run --project src/NetWard.Cli/NetWard.Cli.csproj -- simulate discord
```

---

## Coding Guidelines

- **Architecture:** Keep business logic inside `src/NetWard.Core`. Frontends (`NetWard.App` WPF, `NetWard.Cli`) must remain thin presentation adapters.
- **Async/Await:** Use asynchronous I/O (`await Task...`) for all network socket and DNS operations. Never block the UI thread.
- **Testing:** New diagnostic rules or parsers must be covered by xUnit tests in `tests/NetWard.Tests/`.
- **Localization:** All user-facing strings must be localized in both Russian and English inside `NetWard.Core.Localization.LocalizationService`.

---

## Pull Request Workflow

1. Fork the repository and create a feature branch (`git checkout -b feature/my-feature`).
2. Ensure all tests pass (`dotnet test`).
3. Commit your changes with clear Conventional Commits (`feat: add Telegram DC probing`, `fix: handle WSAECONNRESET on Windows 11`).
4. Push to your fork and submit a Pull Request.
