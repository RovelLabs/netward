# NetWard PC (Windows) Releases & Downloads

Official standalone release builds for Windows 10 & 11 (x64).

---

## 📥 Current Stable Release: v0.1.0

| Package | Description | Size | SHA-256 Checksum | Download |
| :--- | :--- | :---: | :--- | :--- |
| **Desktop GUI Archive** | Full graphical interface with 1-click `install.cmd` script | ~150 MB | `BE3954A0B97F317A84A125BA10BCCF3D673C0D952F16FB57B56C827A5DE57B65` | [NetWard-v0.1.0-Windows-x64.zip](https://github.com/RovelLabs/new-prpect/releases/download/v0.1.0/NetWard-v0.1.0-Windows-x64.zip) |
| **Standalone CLI** | Single-file binary for PowerShell / CMD terminal | ~40 MB | `AFB0348DDD2C556CA6A9BA3BAE91E077BF9BECAC65F5A6DE89A5B25ED3B87A43` | [netward-cli-v0.1.0-windows-x64.exe](https://github.com/RovelLabs/new-prpect/releases/download/v0.1.0/netward-cli-v0.1.0-windows-x64.exe) |

---

## 🚀 Installation Instructions (Windows)

### Option 1: 1-Click Setup (Recommended)
1. Download **NetWard-v0.1.0-Windows-x64.zip**.
2. Extract the archive.
3. Double-click **`install.cmd`**.
4. The program will automatically install to `%LOCALAPPDATA%\Programs\NetWard` and generate Start Menu & Desktop shortcuts. No admin rights required!

### Option 2: Portable Mode
1. Extract the zip archive.
2. Double-click **`NetWard.App.exe`** directly.

---

## 🔍 Verifying Integrity (SHA-256)

In PowerShell:
```powershell
Get-FileHash -Algorithm SHA256 NetWard-v0.1.0-Windows-x64.zip
```
Ensure the hash matches:
`BE3954A0B97F317A84A125BA10BCCF3D673C0D952F16FB57B56C827A5DE57B65`
