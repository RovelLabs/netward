<p align="center">
  <img src="assets/banner.svg" alt="NetWard Banner" width="100%" />
</p>

<p align="center">
  <strong>The Open-Source Internet Resilience &amp; Diagnostics Companion for the Next Generation</strong>
</p>

<p align="center">
  <a href="https://github.com/RovelLabs/new-prpect/actions"><img src="https://img.shields.io/badge/CI-Passing-00E599?style=flat-square&logo=githubactions" alt="CI Status" /></a>
  <a href="https://dotnet.microsoft.com/download/dotnet/8.0"><img src="https://img.shields.io/badge/.NET-8.0%20%7C%2010.0-512BD4?style=flat-square&logo=dotnet" alt=".NET 8" /></a>
  <a href="LICENSE"><img src="https://img.shields.io/badge/License-Apache--2.0-blue?style=flat-square" alt="License" /></a>
  <a href="PRIVACY.md"><img src="https://img.shields.io/badge/Privacy-100%25%20Local--First-00E599?style=flat-square" alt="Privacy Shield" /></a>
  <a href="README.ru.md"><img src="https://img.shields.io/badge/Документация-Русский-white?style=flat-square" alt="Russian Version" /></a>
</p>

---

## ⚡ What is NetWard?

**NetWard** is a modern, tactical, local-first network resilience and diagnostic companion designed for youth, gamers, and students living under unpredictable telecommunications conditions, ISP filtering, and service restrictions.

Instead of guessing why **Discord voice dropped**, why **YouTube is buffering at 144p**, or whether **Steam / CS2 / Roblox** is experiencing server lag, NetWard pierces the digital "fog of war" and provides **exact root-cause telemetry across OSI Layers 3–7**.

```
+-----------------------------------------------------------------------------------+
|  SERVICE    | STATUS     | PRIMARY CAUSE      | CONFIDENCE | LATENCY | RECOMMEND  |
+-----------------------------------------------------------------------------------+
|  Discord    | BLOCKED    | TSPU SNI Drop (RST)|    96%     |  32 ms  | ISP filter |
|  YouTube    | THROTTLED  | Bandwidth Policied |    94%     | 2400 ms | CDN dropped|
|  Steam      | HEALTHY    | Fully Operational  |    98%     |  34 ms  | Ready      |
|  CS2 (EU)   | OPTIMAL    | Zero Packet Loss   |    100%    |  31 ms  | 0% Loss    |
+-----------------------------------------------------------------------------------+
```

---

## 🌟 Key Features

- **🔬 Multi-Layer Root-Cause Engine:** Automatically tests DNS -> TCP Handshake -> TLS Handshake -> HTTP Codes -> Stream Bitrate. Pinpoints whether a failure is a local Wi-Fi glitch, server-side outage, or state-level TSPU SNI RST injection (`WSAECONNRESET` 10054).
- **🎯 Tactical Gaming Radar:** Real-time multi-sample ping, jitter (mean deviation), and packet loss monitoring for Counter-Strike 2 (Warsaw, Stockholm, Frankfurt), Dota 2 (Stockholm, Vienna), Roblox EU, and Minecraft Hypixel.
- **🛡️ DNS Poisoning Protection:** Cross-references local ISP DNS against encrypted DNS-over-HTTPS (DoH) resolvers (Cloudflare `1.1.1.1` and Quad9 `9.9.9.9`).
- **⚡ Local Network Resilience:** One-click DNS resolver cache flush (`DnsFlushResolverCache`) and router gateway latency probing.
- **🔒 Zero-Knowledge & Safe Export:** One-click anonymized diagnostic telemetry export that automatically redacts private IPs, machine names, and usernames for sharing in GitHub issues.
- **🎮 Graphite Dark Visuals:** High-performance native Windows Desktop UI (WPF) and instant CLI runner with zero Electron bloat, 60+ FPS animations, and dark graphite aesthetics.
- **🚫 100% Free & No Ads:** Zero ads, zero tracking, zero subscriptions, zero account requirements.

---

## 🖥️ Platform Support & Downloads

| Platform | Interface | Status | Release Package |
| :--- | :--- | :---: | :--- |
| **Windows 10 / 11** | Native GUI (WPF) | ✅ Stable | [NetWard.App.exe](https://github.com/RovelLabs/new-prpect/releases/latest) |
| **Windows / Linux / macOS** | Terminal CLI | ✅ Stable | [netward-cli](https://github.com/RovelLabs/new-prpect/releases/latest) |
| **Android / iOS** | Companion Core | 📋 Planned v1.0 | Built into `NetWard.Core` |

---

## 🚀 Quick Start (CLI)

Download the latest binary or run directly via .NET SDK:

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

### Prerequisites
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or higher.
- Git.

```bash
# Clone the repository
git clone https://github.com/RovelLabs/new-prpect.git
cd new-prpect

# Build entire solution
dotnet build

# Run automated tests
dotnet test

# Launch Desktop GUI
dotnet run --project src/NetWard.App/NetWard.App.csproj
```

---

## 🔒 Privacy & Threat Model

NetWard operates under a strict **Zero-Knowledge Privacy Shield**:
- **No telemetry phoning home:** NetWard has no central analytics server.
- **Local storage only:** Configuration and history are stored locally in `%LOCALAPPDATA%\NetWard`.
- **Anonymized export:** All diagnostic exports scrub private RFC 1918 IPs, public IPs, and usernames before generation.

Read our full [Privacy Policy](PRIVACY.md) and [Threat Model](docs/THREAT_MODEL.md).

---

## ⚖️ Legal Compliance

NetWard is designed as a **pure network diagnostic, measurement, and local resilience companion**. It does not distribute, configure, or advertise illegal proxy services or prohibited circumvention mechanisms under Russian Federal Law No. 149-FZ (including Roskomnadzor Order No. 168). It operates as a legitimate network engineering tool akin to *Wireshark*, *PingPlotter*, and *OONI Probe*.

---

## 📄 License

Licensed under the **[Apache License, Version 2.0](LICENSE)**. Free to use, modify, and distribute for everyone.
