# NetWard Product Roadmap

This roadmap outlines the planned development trajectory for **NetWard**.

---

## Current Release: v0.1.0 (MVP Milestone) - COMPLETED
- [x] Multi-layer network inspection engine (DNS, TCP, TLS, HTTP, Streaming).
- [x] TSPU TCP RST classification and YouTube throttling detection.
- [x] Gaming radar for CS2, Dota 2, Roblox, and Minecraft.
- [x] DoH dual-resolution comparison (Cloudflare & Quad9).
- [x] Native Windows Desktop App with dark graphite design language.
- [x] Standalone cross-platform CLI runner.
- [x] Automatic IP & username redaction for diagnostic export.
- [x] Full unit test coverage and automated GitHub Actions CI pipeline.

---

## v0.2.0 (Resilience Automation) - Q4 2026
- [ ] **Background Tray Sentinel:** System tray daemon providing desktop alerts when a monitored service transitions from Blocked to Reachable.
- [ ] **Automated Local Encrypted DNS (DoH) Forwarder:** Built-in local resolver helper routing system DNS through encrypted TLS/HTTPS upstream without manual router config.
- [ ] **Custom Service Import/Export:** Share custom gaming server lists via compact QR codes or URLs.

---

## v0.5.0 (Peer Network Intelligence) - Q1 2027
- [ ] **Local Peering Comparison:** Compare reachability metrics with peers on the same ISP using zero-knowledge, encrypted local P2P gossip (no central server).
- [ ] **Advanced MTU & Path Optimizer:** Automated TCP window and MTU bottleneck detection to prevent packet fragmentation in online games.

---

## v1.0.0 (Multi-Platform Ecosystem) - Q2 2027
- [ ] **Android Native Companion:** Kotlin / Compose companion app with background network health widget.
- [ ] **Linux & macOS Native Packages:** Native GUI packaging via Avalonia / native desktop frontends.
- [ ] **Reproducible Builds & Signed Artifacts:** Comprehensive Cosign / Sigstore release verification.
