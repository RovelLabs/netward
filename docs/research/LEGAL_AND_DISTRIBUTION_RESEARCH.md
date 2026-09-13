# Legal, Regulatory & Distribution Risk Analysis

**Document Metadata:**
- **Author:** Legal Compliance & Systems Architecture Research Group
- **Jurisdiction of Interest:** Russian Federation (primary user base), International (GitHub, App Store, Google Play)
- **Applicable Statutes:** Federal Law No. 149-FZ, Federal Law No. 152-FZ, Federal Law No. 436-FZ, Order of Roskomnadzor No. 168
- **Date of Analysis:** September 13, 2026

---

## 1. Statutory Framework Analysis

### 1.1 Federal Law No. 149-FZ & Order of Roskomnadzor No. 168 (March 1, 2024)
- **The Core Prohibition:** Under Article 15.1, Part 5, Clause 5 of 149-FZ (introduced via Federal Law No. 406-FZ) and Roskomnadzor Order No. 168 (effective March 1, 2024), information that:
  1. Describes or demonstrates technical methods to gain access to prohibited information resources;
  2. Advertises, advocates, or promotes the use of VPN/proxy services specifically for bypassing official blocklists;
  3. Offers software or instructions solely designed to circumvent restrictions;
  is subject to inclusion in the Unified Register of Prohibited Information and extrajudicial blocking by Roskomnadzor.
- **The Critical Legal Distinction (Diagnostics vs. Circumvention):**
  - **Prohibited:** "Download this tool / config to bypass the block on Instagram/Discord" or hosting an active proxy/VPN node that routes traffic around RKN blocklists.
  - **Fully Permitted:** Network diagnostics, measurement of round-trip time (RTT), detection of packet loss, local DNS configuration analysis, TLS handshake latency monitoring, reporting reachability telemetry, and local network troubleshooting (e.g. diagnosing MTU issues, router gateway health, or identifying whether an endpoint responds). Diagnostic tools such as *Wireshark*, *PingPlotter*, *WinMTR*, *Speedtest by Ookla*, *OONI Probe*, and *F-Droid* operate completely legally worldwide and in the Russian Federation.

### 1.2 Federal Law No. 152-FZ ("On Personal Data")
- Russian law imposes strict localization and consent requirements for the collection, processing, and cross-border transfer of personal data.
- **Our Architectural Defense:** The application **collects zero personal data**. No user accounts, no phone numbers, no email addresses, no device IMEI/MAC fingerprinting, and no IP address telemetry sent to any central server. Local-first architecture completely bypasses 152-FZ regulatory scope.

### 1.3 Federal Law No. 436-FZ ("Protection of Children from Harmful Information")
- Because our primary audience includes minors (13–17), software distributed to them must not expose them to age-restricted or prohibited content.
- The product does not host, curate, or stream content. It only provides network telemetry, system health metrics, and technical diagnostic cards.

---

## 2. Risk Evaluation Matrix

| Feature / Capability | Legal Risk (RU) | Distribution Risk (GitHub) | Store Risk (Google Play / App Store) | User Risk | Strategic Decision | Rationale |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| **Active Network Diagnostics (Ping, DNS, TCP, TLS)** | None | None | None | None | **APPROVED (Core)** | Standard network engineering functionality; completely transparent. |
| **Service Reachability Cards (Discord, YouTube, Steam)** | Very Low | None | Very Low | None | **APPROVED (Core)** | Reports factual reachability telemetry; does not instruct circumvention. |
| **Failure Cause Classifier (DNS vs RST vs TSPU vs Outage)** | Low | None | None | None | **APPROVED (Core)** | Scientific classification of network packet behavior. |
| **Local Network Tuning (DNS Cache, MTU, IPv6 Check)** | None | None | None | None | **APPROVED (Core)** | Standard operating system networking maintenance. |
| **Built-in VPN / SOCKS Proxy Tunnel to Prohibited Sites** | High (149-FZ RKN block) | Medium (DMCA/abuse reports) | High (delisting in RU region) | Medium (potential IP tracking) | **REJECTED** | Creating another proxy client risks immediate domain/repo blocking and App Store removal. |
| **Pre-packaged Circumvention Batch Scripts (e.g. GoodbyeDPI hexes)**| High | Medium | High | Low | **REJECTED** | Violates RKN Order No. 168 regarding "distributing circumvention instructions". |
| **Diagnostic Export for GitHub / Community Debugging** | None | None | None | None | **APPROVED (Core)** | Anonymized JSON/Text diagnostic log with local redaction of personal IPs. |
| **Cloud Telemetry Aggregation (Central Reporting Server)** | Medium (152-FZ data laws) | Low | Low | Low | **REJECTED (Local-First)** | Running a central analytics server creates server costs, attack vectors, and 152-FZ liabilities. Local-first is strictly superior. |

---

## 3. Platform Distribution Viability

### 3.1 Windows Desktop
- **Distribution Vectors:** GitHub Releases (Direct `.exe` portable + `.msi`/installer), winget manifest.
- **Code Signing:** Without an expensive EV Code Signing Certificate ($300–$500/yr), Windows Defender SmartScreen displays a blue warning for new binaries.
  - *Mitigation:* Document clean SHA-256 checksums, provide reproducible builds, and ensure zero heuristics false-positives via VirusTotal clean scan.

### 3.2 Android
- **Distribution Vectors:** Direct APK download via GitHub Releases, F-Droid submission, RuStore (optional), Google Play Store.
- **Permissions:** Minimal network state permissions (`ACCESS_NETWORK_STATE`, `INTERNET`). No unnecessary background location or storage permissions.

### 3.3 Apple iOS / macOS
- **Distribution Vectors:** Xcode project build for open-source developers; TestFlight / App Store submission ready.
- **Compliance:** App Store Review Guideline 2.1 (Performance) & 5.1 (Privacy) compliant because the app requires zero login, has zero tracking, and provides genuine utility without broken promises.
