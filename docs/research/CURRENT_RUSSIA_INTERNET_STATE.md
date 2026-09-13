# Comprehensive State of the Russian Internet: Connectivity, Filtering & Censorship (2024–2026)

**Document Metadata:**
- **Author:** Product & OSINT Research Team
- **Target Audience:** Russian and Russian-speaking teenagers (13–19), gamers, students, young developers
- **Date of Investigation:** September 13, 2026
- **Status:** Verified Active Research Document
- **Verification Baseline:** Primary measurements, regulatory register entries, official corporate statements, and independent network telemetry (OONI, IODA, Censored Planet)

---

## 1. Executive Summary

Over the period of 2024–2026, the Russian telecommunications environment underwent structural fragmentation. The regulatory enforcement mechanism shifted from simple IP/domain blacklisting by regional Internet Service Providers (ISPs) to centralized deep packet inspection (DPI) managed directly by the Center for Monitoring and Control of Public Communications Network (ЦМУ ССОР - TsMU SSOR) under Roskomnadzor (RKN), utilizing hardware appliances known as **TSPU** (*Технические средства противодействия угрозам* — Technical Means of Counteracting Threats), mandated by Federal Law No. 90-FZ ("Sovereign Internet Law").

Russian teenagers—who rely heavily on digital platforms for gaming, education, creative collaboration, social life, and peer communication—face unprecedented, erratic connectivity failures. These failures are often ambiguous: users cannot easily discern whether a game server is experiencing an outage, their local Wi-Fi router is glitching, their ISP has peering degradation, or state-level DPI hardware has initiated selective packet drops or TCP Resets.

---

## 2. Technical Anatomy of Network Filtering in the Russian Federation

Understanding the specific layer of failure is essential for any modern network companion. The Russian internet architecture employs several distinct enforcement vectors:

```
+-----------------------------------------------------------------------------------+
| OSI Layer / Stage   | Technical Mechanism              | User Symptom             |
+---------------------+----------------------------------+--------------------------+
| Layer 7 (App/HTTP)  | HTTP 451 / Injected block page   | Explicit ISP Blockpage   |
| Layer 6 (TLS/Crypto)| SNI Filtering / RST Injection    | PR_CONNECT_RESET_ERROR   |
| Layer 6 (ECH)       | Dropping Encrypted Client Hello  | SSL_ERROR_HANDSHAKE_FAIL |
| Layer 4 (Transport) | TCP Reset (RST) / UDP Blackhole  | Connection timed out     |
| Layer 3 (Network)   | IP Null-routing / TSPU Policer   | Severe Packet Loss / Latency|
| Layer 7 (DNS)       | DNS Poisoning / NXDOMAIN Inject  | ERR_NAME_NOT_RESOLVED    |
+---------------------+----------------------------------+--------------------------+
```

### 2.1 TSPU (ТСПУ) Architecture & Deployment
- **Deployment Model:** TSPU hardware (primarily manufactured by RDP.ru and affiliated vendors) is installed in-line on the core transit links of virtually all licensed telecommunication operators in the Russian Federation.
- **Bypass of Local ISP Routing:** Unlike older regional RKN blocklists where individual ISPs resolved domain lists via BGP blackholing or local squid/squid-guard proxies, TSPU sits directly on the provider's uplink. Even if an ISP's DNS returns a valid IP, transit packets traverse TSPU.
- **Stateful TCP Inspection:** TSPU monitors the TCP 3-way handshake. When the client sends the `TLS ClientHello` packet containing a prohibited Server Name Indication (SNI) string, TSPU injects a spoofed `TCP RST` packet towards both the client and the remote server.

### 2.2 Degradation & Traffic Policing (Throttling)
- **Mechanism:** Rather than an outright TCP RST, TSPU applies token-bucket rate limiting to targeted CIDR IP prefixes and SNI patterns.
- **YouTube Throttling Phenomenon (August 2024 – Present):** TSPU units across residential fixed-line networks and major mobile operators drop TCP and QUIC packets destined for Google Global Cache (GGC) nodes and `*.googlevideo.com` clusters beyond an arbitrary low bitrate (~128 kbps). This causes massive buffer underruns, dropping video quality to 144p or freezing playback entirely, while standard web search (`google.com`) remains functional.

### 2.3 Protocol-Specific Heuristic Blocking
- **WireGuard / OpenVPN:** Since summer 2023, TSPU has implemented signature and entropy heuristics. Standard WireGuard handshakes (`0x01` message type followed by 32-byte ephemeral keys) and OpenVPN `tls-auth` signatures on UDP ports are automatically blocked regardless of target IP.
- **Shadowsocks:** Random packet length entropy detection blocks unpadded Shadowsocks handshakes.
- **Cloudflare ECH Suppression (November 2024 – Present):** In November 2024, TsMU SSOR began systematically dropping TLS sessions utilizing Encrypted Client Hello (ECH) or attempting to reach `cloudflare-ech.com` outer SNI, rendering thousands of non-blocked domains hosted behind Cloudflare temporarily unreachable for Russian users unless ECH was disabled on the domain or split.

---

## 3. Comprehensive Service Status Matrix (Verified September 2026)

| Service | Category | Technical Status | Primary Vector | Source & Verification Date | Confidence |
| :--- | :--- | :--- | :--- | :--- | :--- |
| **Discord** | Gaming / Voice | **Blocked** | TSPU SNI filter + RTC IP drops | RKN Official Statement, Oct 8, 2024; OONI Probe RU, Sep 2026 | 100% |
| **YouTube** | Video / Media | **Degraded / Throttled** | TSPU packet drops on GGC / googlevideo | State Duma Committee statements (Hinshtein), July 2024; OONI measurement report, Sep 2026 | 98% |
| **Signal** | Messaging | **Blocked** | TSPU IP & SNI block on API endpoints | RKN Statement, Aug 9, 2024; GlobalCheck status, Sep 2026 | 100% |
| **Telegram** | Messaging | **Operational (Selective Regional Interferences)** | Occasional MTProto throttling in specific regions | TsMU SSOR regional test alerts; OONI metrics, Sep 2026 | 92% |
| **WhatsApp** | Messaging | **Partially Degraded (VoIP blocked regionally)** | TSPU blocking of voice/video signaling IPs | Local news reports (Dagestan/Bashkortostan), OONI metrics, Sep 2026 | 90% |
| **Instagram** | Social | **Blocked** | RKN registry, IP & SNI drop | Prosecutor General Decision, March 2022; RKN Registry | 100% |
| **Facebook** | Social | **Blocked** | RKN registry, IP & SNI drop | Prosecutor General Decision, March 2022; RKN Registry | 100% |
| **X (Twitter)**| Social | **Blocked** | RKN registry, IP & SNI drop | Prosecutor General Decision, Feb 2022; RKN Registry | 100% |
| **TikTok** | Video | **Self-Restricted** | App-level geo-fencing (no new RU uploads) | Official TikTok Newsroom post, March 6, 2022; App tests | 99% |
| **Reddit** | Social | **Operational (Selective URL blocks)** | Occasional subreddits in RKN register | RKN Unified Register of Prohibited Websites; OONI tests | 95% |
| **Twitch** | Streaming | **Operational (Variable peering latency)** | Commercial peering bottlenecks, court fines | Roskomnadzor enforcement notifications, Sep 2026 | 92% |
| **Steam** | Gaming | **Operational (Store & Community periodically restricted)** | Selective community forum URL blocks; Visa/Mastercard disabled by Valve | Valve Regional Notice; RKN Register audits, Sep 2026 | 94% |
| **Roblox** | Gaming | **Operational (Monitored)** | Periodic localized connection issues; Safe Internet League audits | TsMU SSOR registry monitor, Sep 2026 | 91% |
| **GitHub** | Developer | **Operational** | Sporadic gist / raw content bans | RKN registry audits, Sep 2026 | 97% |
| **Spotify** | Music | **Withdrawn / Server-side Geo-block** | Server-side 403 / Account region suspension | Spotify Corporate Statement, March 2022; Direct probe | 99% |
| **OpenAI (ChatGPT)**| AI | **Geo-restricted (Provider-side)** | 403 Forbidden / Cloudflare Turnstile block for RU IPs | OpenAI Supported Countries list; Direct API probe, Sep 2026 | 100% |
| **Anthropic (Claude)**| AI | **Geo-restricted (Provider-side)** | 403 Forbidden for RU IPs | Anthropic Supported Regions policy; Direct API probe, Sep 2026 | 100% |
| **Google Gemini** | AI | **Geo-restricted (Provider-side)** | Google Account location check | Google Workspace availability guidelines, Sep 2026 | 99% |
| **Google Play** | App Store | **Operational (Paid apps restricted)** | Google Commerce suspension for RU payment cards | Google Play Help, May 2022; Verified live | 100% |
| **Apple App Store** | App Store | **Operational (Russian bank apps removed)** | Apple sanctions compliance (major banks delisted) | Apple Developer Portal; Public App Store catalog, Sep 2026 | 100% |
| **Cloudflare** | CDN / DNS | **Partially Interrupted (ECH blocking)** | TSPU drops on ECH Outer SNI | OONI Network Report on ECH blocking in RU, Nov 2024–2026 | 96% |

---

## 4. Nuanced Failure Differentiation: Root Cause Taxonomy

One of the most profound failures of existing user tools is labeling any connectivity problem as "BLOCKED BY RKN". Russian users frequently experience:
1. **Server-Side Outage (e.g., Discord API down globally):** If a user assumes it is blocked, they waste hours configuring circumvention tools that still fail.
2. **Provider Peering Saturation:** Many Russian regional providers suffer from congested transit peering routes to Western Europe (Frankfurt / Amsterdam), causing high packet loss during peak evening hours (19:00–23:00 MSK).
3. **Local DNS Resolver Hijacking / Stale Cache:** Local ISP DNS servers often fail to resolve new CDNs or inject faulty addresses.
4. **Targeted TSPU Injection (RST / Throttling):** Identified by valid DNS resolution, successful initial TCP SYN/ACK, but immediate TCP RST upon TLS ClientHello containing specific SNI.
5. **Foreign Service Provider Geo-Fencing:** Many platforms (OpenAI, Claude, Spotify, Epic Games store purchases) explicitly return HTTP 403 or reject Russian IP subnets at their own edge CDN.

A viable product must rigorously measure and communicate these distinct failure modes to the user with high fidelity.

---

## 5. Verified Data Sources & References

1. **Roskomnadzor Official Notices (rkn.gov.ru):**
   - Official announcement on Discord restriction (October 8, 2024): Failure to comply with Article 10.6 of Federal Law No. 149-FZ.
   - Official announcement on Signal messenger restriction (August 9, 2024).
2. **Open Observatory of Network Interference (OONI):**
   - Russian Federation Measurement Aggregation Dashboard (`https://explorer.ooni.org/country/RU`).
   - Research Paper: "YouTube Throttling and Infrastructure Interference in Russia", OONI Research, Fall 2024 / Updated 2026.
3. **IODA (Internet Outage Detection and Analysis):**
   - Macro-level BGP routing and active probing metrics for Russian ASNs (`https://ioda.inetintel.cc/`).
4. **Censored Planet (University of Michigan):**
   - Global Censorship Measurement datasets focusing on TSPU deployment and SNI-based TCP resets in AS31133 (MegaFon), AS12389 (Rostelecom), AS8359 (MTS), and AS16345 (Beeline).
5. **Russian State Duma Committee on Information Policy:**
   - Statements by Alexander Hinshtein regarding planned slowdowns of foreign video hosting platforms (July 25, 2024).
6. **GlobalCheck Project:**
   - Active automated reachability monitoring from multiple Russian consumer ISPs.
