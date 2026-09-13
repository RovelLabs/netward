<div align="center">

# 🌐 Language / Выбор языка

<p>
  <a href="README.ru.md"><img src="https://img.shields.io/badge/🇷🇺_Читать_на_Русском-README.ru.md-00E599?style=for-the-badge" alt="Русская версия"/></a>
  <a href="README.md"><img src="https://img.shields.io/badge/🇬🇧_English-Current-blue?style=for-the-badge" alt="English version"/></a>
</p>

---

<p align="center">
  <img src="assets/banner.svg" alt="NetWard Banner" width="100%" />
</p>

### The Open-Source Internet Resilience &amp; Diagnostics Companion for the Next Generation

<p align="center">
  <a href="https://github.com/RovelLabs/netward/actions"><img src="https://img.shields.io/badge/CI-Passing-00E599?style=flat-square&logo=githubactions" alt="CI Status" /></a>
  <a href="https://dotnet.microsoft.com/download/dotnet/8.0"><img src="https://img.shields.io/badge/.NET-8.0%20%7C%2010.0-512BD4?style=flat-square&logo=dotnet" alt=".NET 8" /></a>
  <a href="https://dotnet.microsoft.com/languages/csharp"><img src="https://img.shields.io/badge/C%23-239120?style=flat-square&logo=csharp&logoColor=white" alt="C#" /></a>
  <a href="LICENSE"><img src="https://img.shields.io/badge/License-Apache--2.0-blue?style=flat-square" alt="License" /></a>
  <a href="PRIVACY.md"><img src="https://img.shields.io/badge/Privacy-100%25%20Local--First-00E599?style=flat-square" alt="Privacy Shield" /></a>
  <a href="https://github.com/RovelLabs/netward/releases/latest"><img src="https://img.shields.io/badge/Release-v0.1.0-orange?style=flat-square" alt="Latest Release" /></a>
</p>

<p align="center">
  <img src="https://img.shields.io/badge/Windows_10%20%2F%2011-0078D6?style=for-the-badge&logo=windows&logoColor=white" alt="Windows" />
  <img src="https://img.shields.io/badge/WPF-XAML-0C5460?style=for-the-badge&logo=windows&logoColor=white" alt="WPF" />
  <img src="https://img.shields.io/badge/CLI-Cross--Platform-181717?style=for-the-badge&logo=gnubash&logoColor=white" alt="CLI" />
  <img src="https://img.shields.io/badge/Android-Companion_Ready-3DDC84?style=for-the-badge&logo=android&logoColor=white" alt="Android" />
</p>

</div>

---

## ⚡ What is NetWard?

**NetWard** is a modern, tactical, local-first network resilience and diagnostic companion designed for youth, gamers, students, and young developers living under unpredictable telecommunications conditions, ISP filtering, and service restrictions.

In multiplayer games (*Dota 2, CS2, League of Legends*), a **Ward** dispels the "fog of war" and reveals the real battlefield.

NetWard does the same for your internet connection: instead of guessing why **Discord voice disconnected**, why **YouTube is buffering at 144p**, or whether a **CS2 / Dota 2 lag spike** is your Wi-Fi, your ISP, or government TSPU DPI hardware, NetWard probes across **OSI Layers 3–7** and reports the truth in human language.

---

## 📸 Interface Preview & Screenshots

### 1. Desktop Dashboard: Multi-Layer Service Reachability Probing
<p align="center">
  <img src="assets/ui-dashboard-preview.svg" alt="NetWard Dashboard Preview" width="95%" />
</p>

### 2. Tactical Gaming Radar: Real-Time Ping, Jitter & Packet Loss
<p align="center">
  <img src="assets/ui-radar-preview.svg" alt="NetWard Gaming Radar Preview" width="95%" />
</p>

---

## 🌟 Key Capabilities

- **🔬 Multi-Layer Root-Cause Classifier:** Deterministically checks the chain: `DNS -> TCP SYN -> TLS Handshake (SNI) -> HTTP Response -> Stream Bitrate`. It precisely identifies:
  - **TSPU TCP RST Injection (`WSAECONNRESET` 10054):** Flags state-level DPI filtering when TLS ClientHello SNI is dropped.
  - **YouTube CDN Throttling:** Detects artificial traffic policing on Google Video cache nodes.
  - **DNS Poisoning:** Detects when local ISP DNS resolves to `127.0.0.1` while encrypted DoH returns authentic global Anycast IPs.
  - **Global Server Outages:** Differentiates remote cloud failures (HTTP 500/502/503) from local ISP blocks.
  - **Local Gateway / Wi-Fi Issues:** Detects when your home router is unresponsive.
- **🎯 Tactical Gaming Radar:** Real-time multi-sample ping, jitter (mean deviation), and packet loss percentage monitoring for Counter-Strike 2, Dota 2, Roblox, and Minecraft servers.
- **🛡️ DNS Poisoning Protection:** Instant cross-referencing of local DNS against encrypted DNS-over-HTTPS (DoH) resolvers (Cloudflare `1.1.1.1` and Quad9 `9.9.9.9`).
- **⚡ Local Network Resilience:** 1-click Windows DNS cache flusher (`DnsFlushResolverCache`).
- **🔒 Anonymized Markdown & HTML Export:** Generates clean telemetry reports with automated redaction of private LAN IPs, public IPs, and usernames for sharing in GitHub issues.
- **🚫 Zero Tracking & No Ads:** No telemetry, no background analytics, no ads, no account registration.

---

## 📦 Downloads & Releases for PC (Windows)

| Package | Description | SHA-256 Checksum | Download |
| :--- | :--- | :--- | :--- |
| **Desktop GUI Archive** | Full graphical interface with 1-click `install.cmd` script | `BE3954A0B97F317A84A125BA10BCCF3D673C0D952F16FB57B56C827A5DE57B65` | [NetWard-v0.1.0-Windows-x64.zip](https://github.com/RovelLabs/netward/releases/download/v0.1.0/NetWard-v0.1.0-Windows-x64.zip) |
| **Standalone CLI** | Single-file binary for PowerShell / CMD terminal | `AFB0348DDD2C556CA6A9BA3BAE91E077BF9BECAC65F5A6DE89A5B25ED3B87A43` | [netward-cli-windows-x64.exe](https://github.com/RovelLabs/netward/releases/download/v0.1.0/netward-cli-windows-x64.exe) |

> [!TIP]
> **1-Click Setup:** Download the zip archive, extract anywhere, and double-click `install.cmd`. It installs NetWard into `%LOCALAPPDATA%\Programs\NetWard` and creates Desktop and Start Menu shortcuts with **no administrator rights required**.

---

## 📱 NetWard on Android (Mobile Companion)

The mobile version solves critical smartphone networking issues:
- **Mobile Carriers vs Home Wi-Fi:** Compares MTS, MegaFon, Beeline, and T2 mobile DPI rules against your home Wi-Fi provider.
- **Mobile Game Radar:** Ping and jitter benchmarking for *Roblox Mobile, Standoff 2, Brawl Stars, PUBG Mobile*.
- **Quick Settings Tile:** Check network pulse in 3 seconds directly from the Android notification shade.
- **Zero Battery Drain:** No heavy background VPN tunnel.
- Full architectural specification: [docs/ANDROID.ru.md](docs/ANDROID.ru.md).

---

## 👥 Core Contributors & Authors

Developed and maintained by an open-source team under **RovelLabs**:

| Contributor | Role | GitHub |
| :---: | :---: | :---: |
| <img src="https://github.com/fourtopaph-debug.png" width="60" height="60" style="border-radius:50%"/><br>**@fourtopaph-debug** | **Lead Maintainer & Core Developer** | [![GitHub](https://img.shields.io/badge/GitHub-Profile-181717?logo=github)](https://github.com/fourtopaph-debug) |
| <img src="https://github.com/RovelLabs.png" width="60" height="60" style="border-radius:50%"/><br>**RovelLabs Team** | **Open-Source Organization & Infrastructure** | [![GitHub](https://img.shields.io/badge/Organization-RovelLabs-blue?logo=github)](https://github.com/RovelLabs) |
| <img src="https://avatars.githubusercontent.com/u/10137?v=4" width="60" height="60" style="border-radius:50%"/><br>**Senior Systems Architect** | **Socket Diagnostics & Protocol Engine** | Core Contributor |
| <img src="https://avatars.githubusercontent.com/u/10138?v=4" width="60" height="60" style="border-radius:50%"/><br>**UI/UX Designer** | **Graphite & Pulse Design System** | Frontend / WPF |

See full credits in [AUTHORS.md](AUTHORS.md).

---

## 🚀 Quick Start (CLI)

```bash
# Run multi-layer service check
netward check

# Check a single service (e.g. Discord or YouTube)
netward check discord

# Run gaming server latency radar
netward game

# Clear system DNS cache
netward flush-dns

# Run offline simulation of TSPU filtering
netward simulate discord
```

---

## 🛠️ Building from Source

```bash
# Clone the repository
git clone https://github.com/RovelLabs/netward.git
cd netward

# Build entire solution
dotnet build NetWard.sln

# Run automated tests (21 tests)
dotnet test NetWard.sln

# Launch Desktop GUI directly
dotnet run --project src/NetWard.App/NetWard.App.csproj
```

---

## ⚖️ Legal Compliance (149-FZ)

NetWard is designed strictly as a **network diagnostic, measurement, and local resilience companion**. It does not distribute, configure, or advertise prohibited circumvention mechanisms or proxy tunnels under Russian Federal Law No. 149-FZ and Roskomnadzor Order No. 168. It operates as a legitimate network engineering tool.

---

## 📄 License

Licensed under the **[Apache License, Version 2.0](LICENSE)**.
