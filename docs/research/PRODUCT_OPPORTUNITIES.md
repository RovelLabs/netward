# Product Opportunities Discovery & Concept Scoring

**Document Metadata:**
- **Author:** Senior Product Management & Architecture Team
- **Date:** September 13, 2026
- **Objective:** Evaluate at least 10 concrete product concepts across 17 rigorous criteria to identify the optimal open-source product for Russian-speaking youth.

---

## 1. Ten Candidate Product Concepts

1. **Concept 1: Internet Resilience & Diagnostic Companion ("NetPulse" / "Pulseway")**
   - A modern, local-first desktop and mobile application that instantly diagnoses why popular services (Discord, YouTube, Steam, Telegram, Roblox, GitHub, AI) are failing. Performs multi-layered network probing (DNS -> TCP -> TLS handshake -> HTTP/RTC) to distinguish server outages, ISP routing degradation, Wi-Fi issues, and state-level DPI/TSPU interference. Includes real-time gaming ping/jitter monitors and safe local network optimization (DNS cache flush, MTU suggestions, DoH verification).
2. **Concept 2: Yet Another Multi-Protocol Proxy Client (GUI wrapper for sing-box/Xray)**
   - A generic desktop/mobile client allowing users to paste VLESS/Reality/Hysteria subscription links and route traffic.
3. **Concept 3: Local P2P Voice & Text Mesh for Gamers**
   - An encrypted local-network/peer-to-peer audio communicator (like Mumble or WebRTC mesh) allowing friends on the same LAN or local peering to talk without central servers.
4. **Concept 4: Game Latency & Route Optimizer (Open-Source ExitLag)**
   - An application that monitors packet loss and ping across multiple game servers (CS2, Dota 2, Valorant) and optimizes Windows TCP/UDP socket settings (Nagle's algorithm, MTU, DSCP priority tags).
5. **Concept 5: Centralized Crowd-Sourced Outage Dashboard (Sboy.rf Clone)**
   - A crowd-reporting app where users press "YouTube is down for me" and view heatmaps.
6. **Concept 6: Automated DPI Bypass Daemon (GUI for Zapret/GoodbyeDPI)**
   - A graphical front-end that installs WinDivert kernel drivers and injects TCP segmentation / fake packets to evade DPI.
7. **Concept 7: Encrypted DNS & DoH Local Resolver Suite**
   - A system utility that manages encrypted DNS over HTTPS (Cloudflare, Quad9, AdGuard) with latency benchmarking and split-horizon routing.
8. **Concept 8: Censorship Measurement Probe (Russian Youth OONI)**
   - An academic background collector running scheduled batch tests and publishing raw network telemetry to a global database.
9. **Concept 9: Local Offline Knowledge & Tool Repository**
   - An offline-first reference hub containing cached game wikis, documentation, and networking cheat-sheets.
10. **Concept 10: Smart Game Launcher & Network Status Bar**
    - A custom game launcher (Steam/Roblox/Minecraft) that pre-checks network connectivity and server health before launching the game.

---

## 2. Comprehensive Scoring Matrix (0–10 Scale)

*Criteria definitions:*
- **C1:** Real Usefulness | **C2:** Relevance (2026 RU context) | **C3:** Youth/Teen Appeal | **C4:** Uniqueness | **C5:** Virality Potential
- **C6:** Engineering Feasibility (10 = optimal balance of high capability & feasible completion)
- **C7:** Zero Infra Cost (10 = $0 server bills, local-first)
- **C8:** Legal Viability (149-FZ compliance) | **C9:** App Store Viability | **C10:** Google Play Viability | **C11:** Windows Viability
- **C12:** Privacy (10 = zero tracking) | **C13:** Open-Source Suitability | **C14:** Long-term Maintainability
- **C15:** Network Resilience in RU (resists blocks) | **C16:** Free for User | **C17:** GitHub Distribution Fit

| Concept | C1 | C2 | C3 | C4 | C5 | C6 | C7 | C8 | C9 | C10 | C11 | C12 | C13 | C14 | C15 | C16 | C17 | **TOTAL (/170)** |
| :--- | :---: | :---: | :---: | :---: | :---: | :---: | :---: | :---: | :---: | :---: | :---: | :---: | :---: | :---: | :---: | :---: | :---: | :---: |
| **1. Resilience & Diagnostic Companion** | **10** | **10** | **9** | **9** | **9** | **9** | **10** | **10** | **9** | **10** | **10** | **10** | **10** | **9** | **10** | **10** | **10** | **164** |
| 2. Generic Multi-Protocol Proxy Client | 7 | 8 | 7 | 2 | 5 | 6 | 7 | 2 | 2 | 4 | 8 | 7 | 8 | 5 | 8 | 8 | 8 | 102 |
| 3. Local P2P Voice & Text Mesh | 6 | 7 | 6 | 7 | 5 | 5 | 8 | 9 | 7 | 8 | 8 | 9 | 8 | 6 | 8 | 9 | 8 | 124 |
| **4. Game Latency & Route Optimizer** | **8** | **9** | **10** | **8** | **8** | **8** | **10** | **10** | **7** | **8** | **10** | **10** | **9** | **8** | **9** | **10** | **9** | **152** |
| 5. Crowd-Sourced Outage Dashboard | 5 | 6 | 5 | 3 | 5 | 7 | 3 | 8 | 8 | 8 | 8 | 6 | 7 | 6 | 6 | 9 | 8 | 108 |
| 6. Automated DPI Bypass GUI (WinDivert) | 9 | 10 | 9 | 6 | 9 | 5 | 10 | 3 | 0 | 0 | 7 | 9 | 8 | 4 | 7 | 10 | 7 | 113 |
| **7. Encrypted DNS & DoH Resolver Suite**| **7** | **8** | **7** | **7** | **6** | **8** | **10** | **10** | **8** | **9** | **10** | **10** | **9** | **8** | **8** | **10** | **9** | **144** |
| 8. Censorship Measurement Probe (OONI clone)| 6 | 7 | 4 | 4 | 4 | 8 | 4 | 7 | 8 | 8 | 8 | 6 | 8 | 7 | 8 | 9 | 8 | 114 |
| 9. Local Offline Knowledge Hub | 5 | 5 | 4 | 5 | 4 | 8 | 10 | 10 | 9 | 9 | 9 | 10 | 8 | 8 | 9 | 10 | 9 | 131 |
| 10. Smart Game Launcher & Health Bar | 7 | 7 | 8 | 6 | 6 | 6 | 9 | 9 | 4 | 7 | 9 | 9 | 8 | 6 | 8 | 9 | 8 | 130 |

---

## 3. Detailed Comparison of Top 3 Concepts

The top 3 scoring concepts are:
1. **Concept 1: Internet Resilience & Diagnostic Companion** (Score: 164/170)
2. **Concept 4: Game Latency & Route Optimizer** (Score: 152/170)
3. **Concept 7: Encrypted DNS & DoH Resolver Suite** (Score: 144/170)

### Comparative Breakdown:

| Evaluation Dimension | Concept 1: Resilience & Diagnostic Companion | Concept 4: Game Latency & Route Optimizer | Concept 7: Encrypted DNS Resolver Suite |
| :--- | :--- | :--- | :--- |
| **Core Value Proposition** | Tells the user exactly why their services (Discord, YouTube, Steam, etc.) fail, provides root-cause classification, and offers immediate local health fixes. | Focuses strictly on ping, jitter, and packet loss for games (CS2, Dota 2, Roblox, Minecraft). | Manages DNS servers and DoH/DoT connections to stop DNS hijacking. |
| **Breadth of Appeal** | Extremely broad: covers gamers, video watchers, students, and junior developers. | High among competitive PC gamers; narrower for casual mobile users and students. | Moderate; DNS alone solves only a small fraction of Russian TSPU blocks. |
| **Legal & Regulatory Safety** | **100% compliant**. Diagnostics and network telemetry are entirely legal under 149-FZ. | **100% compliant**. Network routing optimization is universally legal. | **100% compliant**. Standard network configuration. |
| **App Store & Platform Reach** | Full cross-platform suitability (Windows, Android, iOS, macOS, Linux). | Primarily Windows desktop (games run on PC). | Android & Windows; limited on iOS without VPN profile entitlement. |
| **Synergy & Synthesis** | **Can seamlessly integrate Concept 4 and Concept 7 as specialized core modules!** | Too narrow to stand alone as a flagship project. | Too narrow; cannot diagnose TLS or video streaming throttling. |

---

## 4. Synthesis & Recommendation

Concept 1 (**Internet Resilience & Diagnostic Companion**) is the undisputed victor. Crucially, rather than treating Concept 4 (Gaming Latency & Route Optimizer) and Concept 7 (DNS & Network Tuning) as competitors, Concept 1 naturally **absorbs them as built-in subsystems**:
- **Subsystem A:** Real-time Service Reachability & Root-Cause Classifier (Discord, YouTube, Steam, Telegram, GitHub, OpenAI, etc.).
- **Subsystem B:** Tactical Game Server Ping, Jitter & Packet Loss Monitor (CS2, Dota 2, Valorant, Roblox, Minecraft).
- **Subsystem C:** Local Network Health & Resolver Inspector (DNS speed benchmark, DoH reachability, Gateway latency, MTU test).
- **Subsystem D:** Safe Diagnostic Export (Redacted telemetry logs for GitHub issues or community help).

This synthesis produces a category-defining product with immense real-world utility and zero legal friction.
