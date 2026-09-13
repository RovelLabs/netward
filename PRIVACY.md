# NetWard Privacy Policy & Guarantees

**Last Updated:** September 13, 2026  
**Version:** 1.0.0  
**Commitment:** 100% Local-First, Zero-Tracking, Zero-Bullshit.

---

## 1. The Core Privacy Principle

NetWard is designed specifically for individuals—including teenagers and students—whose digital rights and privacy are constantly under threat. We believe that a network diagnostic and resilience tool should **never** monitor, profile, track, or commercialize the people it exists to protect.

```
+-------------------------------------------------------------+
|                     NETWARD PRIVACY SHIELD                  |
|                                                             |
|  [NO ADS]           [NO TRACKERS]        [NO TELEMETRY]     |
|  [NO ACCOUNTS]      [NO FINGERPRINTS]    [NO SELLING DATA]  |
+-------------------------------------------------------------+
```

---

## 2. Explicit Commitments

### 2.1 Zero Advertising
NetWard contains **no advertisements, no sponsored banners, no affiliate referrals, and no commercial SDKs**. We do not partner with ad networks or data brokers.

### 2.2 Zero Tracking & Analytics
NetWard does **not** include Google Analytics, Firebase, Yandex Metrica, AppCenter, Sentry, or any background telemetry library. The application does not phone home to any analytics server.

### 2.3 Zero Account Registration
You do not need to provide an email, phone number, username, password, or Discord ID to use NetWard. There is no login screen, no registration gate, and no cloud profile.

### 2.4 Zero Device Fingerprinting
NetWard does not collect or inspect your MAC address, Motherboard UUID, IMEI, Windows Product Key, or browser cookies.

### 2.5 Local-First Storage
All application data (custom service endpoints, gaming latency history, user preferences) is stored **strictly on your local device** in human-readable JSON format inside your local application directory (`%LOCALAPPDATA%\NetWard`). Nothing is synced to any remote server.

---

## 3. Network Activity Transparency

To provide accurate diagnostics, NetWard sends targeted network requests from your device. Here is complete transparency on what network activity occurs:

1. **Service Probing (DNS, TCP SYN, TLS Handshake, HTTP):**
   - When you click "Diagnose" or enable auto-monitoring, NetWard initiates network connections to official public endpoints of the monitored services (e.g., `discord.com`, `gateway.discord.gg`, `youtube.com`, `googlevideo.com`, `steamcommunity.com`, `api.github.com`).
   - These requests originate from your machine directly to the public service endpoints. No intermediary proxy server intercepts your traffic.
2. **Gaming Server Ping:**
   - Standard ICMP or TCP ping packets sent to official game server clusters (Valve CS2/Dota, Roblox, Minecraft) to calculate real-time latency and packet loss.
3. **Encrypted DNS Probing (DoH):**
   - When diagnosing DNS health, NetWard queries trusted public recursive resolvers (Cloudflare `1.1.1.1`, Quad9 `9.9.9.9`, Google `8.8.8.8`) over HTTPS to compare against your ISP's local DNS response.
4. **Diagnostic Export Safety:**
   - If you choose to use the "Export Diagnostics" feature, NetWard generates a local report. Before export, NetWard automatically redacts your local IP addresses (e.g., replacing `192.168.x.x` or public IPv4 with `[REDACTED_IP]`) to ensure you can safely paste the report into a GitHub issue without leaking your network identity.

---

## 4. Compliance with Laws Protecting Minors

NetWard complies fully with the principles of the Convention on the Rights of the Child, Federal Law No. 152-FZ, and Federal Law No. 436-FZ. By collecting **zero personal data**, NetWard completely eliminates the risk of data breaches, unauthorized commercial profiling, or surveillance of underage users.

---

## 5. Auditability

NetWard is 100% free open-source software under the Apache-2.0 license. Every line of source code, build script, and dependency manifest is publicly auditable in our GitHub repository:  
`https://github.com/RovelLabs/new-prpect`
