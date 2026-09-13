# Building NetWard for Linux

This document explains how to build and package `NetWard.Cli` and `NetWard.Core` on Linux systems (Ubuntu, Debian, Arch, Fedora).

---

## 1. Prerequisites

Install the .NET 8.0 SDK:
```bash
# Ubuntu / Debian
sudo apt-get update && sudo apt-get install -y dotnet-sdk-8.0

# Arch Linux
sudo pacman -S dotnet-sdk-8.0
```

---

## 2. Building the CLI Runner for Linux x64

```bash
dotnet publish src/NetWard.Cli/NetWard.Cli.csproj \
  -c Release \
  -r linux-x64 \
  --self-contained true \
  -p:PublishSingleFile=true \
  -o dist/netward-linux-x64
```

### Running on Linux:
```bash
chmod +x dist/netward-linux-x64/NetWard.Cli
./dist/netward-linux-x64/NetWard.Cli check
```
