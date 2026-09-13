# Product Strategy & Architecture Decision: NetWard

**Document Metadata:**
- **Project Codename:** NetWard (НетВард)
- **Tagline:** The Open-Source Internet Resilience & Diagnostics Companion for the Next Generation
- **Author:** Autonomous Product & Engineering Lead
- **Date:** September 13, 2026
- **Status:** APPROVED & BINDING

---

## 1. The Chosen Product: NetWard (НетВард)

### 1.1 Brand Identity & Cultural Metaphor
In competitive multiplayer games (*Dota 2, League of Legends, tactical shooters*), a **Ward** is an essential item placed by players to pierce the "fog of war" and reveal hidden terrain, ambush points, and real conditions on the map.

In the 2026 Russian telecommunications reality, users operate in a digital **fog of war**:
- *Why did Discord voice disconnect?* (Is it Discord? My Wi-Fi? My ISP? Or Roskomnadzor's TSPU?)
- *Why is YouTube buffering at 144p?* (Is Google down? Is my router overheating? Or is TSPU dropping GGC packets?)
- *Why did my CS2 / Dota 2 ping spike to 300ms?* (Is the Vienna game server lagging? Or is my ISP routing transit packets through a congested peering link?)

**NetWard** is the digital "ward" for your connection: a sleek, graphite-styled, high-performance desktop and mobile companion that dispels the fog of war, accurately identifies the root cause of network degradation across OSI Layers 3–7, monitors game server health, and provides actionable local network resilience tips—all with **zero ads, zero tracking, and zero subscription costs**.

---

## 2. Core Value Pillars

1. **Honest Multi-Layer Root-Cause Engine:**
   - Evaluates reachability across 4 distinct inspection layers:
     * **Layer 3/4:** ICMP / TCP SYN Ping & Round-Trip Jitter
     * **Layer 5/6:** TLS ClientHello Handshake & SNI filtering detection (identifies TSPU TCP RST injection)
     * **Layer 7:** HTTP Response Codes & Content Length verification (detects ISP block pages vs server 500 errors)
     * **DNS Health:** Local UDP 53 vs Encrypted DoH (Cloudflare/Quad9/Google) to detect DNS poisoning
2. **Tactical Gaming Radar:**
   - Real-time latency, jitter, and packet loss benchmarking for major competitive gaming servers (*Counter-Strike 2, Dota 2, Roblox, Minecraft, Apex Legends, Valorant*).
3. **Youth-Centric Service Monitor:**
   - Dedicated, continuous telemetry for the platforms that teenagers actually care about:
     * **Discord** (Gateway API + Voice RTC endpoints)
     * **YouTube** (Web portal + Google Video GGC CDN stream latency)
     * **Steam** (Store API + Community hub reachability)
     * **Telegram** (DC1–DC5 MTProto edge endpoints)
     * **GitHub** (API + raw content delivery)
     * **Roblox** (Authentication + Game asset CDN)
     * **AI Hub** (OpenAI, Claude, Gemini reachability status)
4. **Local Network Health & Tuning:**
   - Diagnostic tools for local gateway latency, Wi-Fi signal quality, MTU optimization guidance, and DNS cache flushing.
5. **Safe Diagnostic Export:**
   - One-click export of an anonymized, redacted Markdown / JSON diagnostic report to attach to GitHub issues, ISP support tickets, or share with tech-savvy friends.

---

## 3. Why NetWard Wins

| Critical Criterion | NetWard | Traditional Proxy / VPN Apps | OONI Probe / Wireshark |
| :--- | :--- | :--- | :--- |
| **Legal Compliance (149-FZ)** | **100% Safe**. Completely compliant with Russian laws prohibiting the popularization of circumvention tools. | High risk of domain blocking and app store delisting. | Safe, but complex and academic. |
| **Monetization & Ads** | **100% Free & Open Source**. No subscriptions, no affiliate links, no banner ads. | Freemium upsells, full-screen video ads, data resale. | Free, but non-commercial academic. |
| **Ease of Use for Teens** | **30-second comprehension**. Beautiful dark graphite cards, human-readable status, zero terminal commands. | Confusing configs, VLESS links, broken keys. | Extreme learning curve; raw packet hexes. |
| **Resource Footprint** | **Ultra-lightweight**. Negligible background CPU and RAM usage (<40MB RAM). | Heavy tunnel drivers, battery drain. | Moderate. |

---

## 4. Product Roadmap & Deliverables

- **v0.1.0 (Current MVP Release):**
  - High-performance Core Engine (Multi-layered DNS, TCP, TLS, and HTTP probing).
  - Service Registry with out-of-the-box definitions for Discord, YouTube, Steam, Telegram, GitHub, Roblox, OpenAI, and Twitch.
  - Gaming Latency Radar (CS2 Europe East/West, Dota 2 Stockholm/Vienna, Roblox EU, Minecraft Hypixel).
  - Failure Classifier (Distinguishes Server Down vs Local DNS vs TCP RST / TSPU vs Throttled).
  - Anonymized Diagnostic Export (JSON & Markdown).
  - Modern Graphite Desktop & Mobile-ready architecture with clean Russian & English localization.
  - Comprehensive Test Suite, CI Workflows, and Release Packaging.
- **v0.2.0:**
  - Automated Local DNS over HTTPS (DoH) local proxy helper.
  - Background tray notification when a monitored service transitions from Down to Up.
- **v0.5.0:**
  - Extended P2P diagnostic sharing between friends on the same ISP.
- **v1.0.0:**
  - Multi-platform parity (Windows native installer, Android APK, macOS & Linux packages).
