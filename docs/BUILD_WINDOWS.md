# Building NetWard for Windows Desktop

This document describes how to compile, publish, and package standalone release builds of NetWard for Windows 10/11.

---

## 1. Fast Development Build

```powershell
dotnet build src/NetWard.App/NetWard.App.csproj -c Debug
```

---

## 2. Standalone Single-File Release Build

To produce a single, portable `.exe` file that requires zero pre-installed runtimes on user machines:

```powershell
dotnet publish src/NetWard.App/NetWard.App.csproj `
  -c Release `
  -r win-x64 `
  --self-contained true `
  -p:PublishSingleFile=true `
  -o dist/NetWard-Windows-x64
```

### Artifacts Produced:
- `dist/NetWard-Windows-x64/NetWard.App.exe` (Standalone executable)
- Direct execution: Double-click to launch the dark graphite GUI.

---

## 3. Building Standalone CLI

```powershell
dotnet publish src/NetWard.Cli/NetWard.Cli.csproj `
  -c Release `
  -r win-x64 `
  --self-contained true `
  -p:PublishSingleFile=true `
  -o dist/cli-win-x64
```

---

## 4. Code Signing & SmartScreen Notes

If building without an Extended Validation (EV) code signing certificate, Windows SmartScreen will display an informational prompt on initial launch. 
To verify integrity, generate SHA-256 checksums:

```powershell
Get-FileHash -Algorithm SHA256 dist/NetWard-Windows-x64/NetWard.App.exe
```
Compare the output against official `checksums.txt` in the GitHub release.
