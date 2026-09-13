# Changelog

All notable changes to **NetWard** will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

---

## [0.1.0] - 2026-09-13

### Added
- **Core Diagnostic Engine:** Multi-layer inspection across OSI Layers 3–7 (DNS resolution, TCP SYN-ACK handshake, TLS ClientHello negotiation, HTTP status codes, and streaming throughput estimation).
- **TSPU & DPI Detection:** Specific classification of TSPU TCP RST injection (`WSAECONNRESET` 10054) during TLS ClientHello SNI exchange.
- **Preconfigured Service Profiles:** Out-of-the-box telemetry for Discord, YouTube, Steam, Telegram, Roblox, GitHub, OpenAI (ChatGPT), and Twitch.
- **Tactical Gaming Radar:** Multi-sample ping, jitter (mean deviation), and packet loss percentage monitoring for Counter-Strike 2, Dota 2, Roblox, and Minecraft servers.
- **DNS Dual-Resolution Inspector:** Queries local system DNS against encrypted DNS-over-HTTPS (Cloudflare 1.1.1.1 and Quad9 9.9.9.9) to flag DNS poisoning and ISP redirection.
- **Local Network Resilience:** Fast local DNS cache flushing (`DnsFlushResolverCache` / `ipconfig /flushdns`) and router gateway latency probing.
- **Diagnostic Export Subsystem:** Automatic redaction of private IPv4/IPv6 subnets, Windows usernames, and machine hostnames, generating clean GitHub-ready Markdown reports.
- **Native Windows Desktop UI (WPF):** Hardware-accelerated dark graphite dashboard with real-time status cards, expandable technical drawers, and responsive controls.
- **Cross-Platform CLI Companion (`netward`):** High-speed terminal runner with colored ANSI outputs and `--json` machine-readable flag.
- **Deterministic Network Simulator:** Toggleable modes (`SimulateDiscordBlocked`, `SimulateYouTubeThrottled`, `SimulateServerOutage`, `SimulateDnsPoisoning`) for zero-internet testing and CI verification.
- **Comprehensive Unit & Simulation Test Suite:** 21 automated xUnit tests validating classification rules, storage safety, and redaction.

### Security
- Zero-tracking and zero-analytics architecture with no external telemetric dependencies.
- Atomic configuration persistence preventing cache corruption on unexpected power cuts.
