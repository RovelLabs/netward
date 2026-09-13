# NetWard Design System: Graphite & Pulse

**Document Metadata:**
- **Design Language:** Graphite Dark / Tactical Minimal
- **Primary Viewports:** Desktop (1280x800 base, scalable to 4K), Mobile Responsive (390x844 base)
- **Target Aesthetic:** Restrained, dark graphite, tactile, confident, high-tech, modern youth (reminiscent of Discord / Steam Deck / Linear / Raycast)
- **Version:** 1.0.0

---

## 1. Color Palette & Semantic Tokens

NetWard shuns clichéd neon green "hacker" terminals and generic cyan corporate SaaS. Instead, it utilizes deep carbon and graphite surfaces with high-precision optical status accents.

```
+--------------------------------------------------------------------------+
| SURFACE TOKENS                                                           |
| Background Primary:    #0D0E11  (Deep Obsidian Void)                     |
| Surface Elevated:      #16181D  (Graphite Panel Surface)                 |
| Surface Card:          #1D2027  (Interactive Card Container)             |
| Surface Hover:         #262A34  (Hover State Card Highlight)             |
| Border Subtle:         #2A2E39  (1px Structural Separation Border)       |
| Border Focus:          #3E4453  (Active Focus / Interactive Stroke)      |
+--------------------------------------------------------------------------+
| STATUS & ACCENT TOKENS                                                   |
| Status Healthy (Up):   #00E599  (Crisp Emerald — 100% Reachable)         |
| Status Degraded:       #FFB300  (Amber Glow — High Ping / Jitter / Slow) |
| Status Blocked (TSPU): #FF334B  (Vibrant Crimson — RST / SNI Block)      |
| Status Throttled:      #C054FF  (Electric Violet — Traffic Policed)      |
| Status Unknown/Offline:#6B7280  (Muted Slate — Unchecked / No Route)     |
| Text Primary:          #F3F4F6  (Near White, 95% Contrast)               |
| Text Secondary:        #9CA3AF  (Tactical Gray, 70% Contrast)            |
| Text Tertiary/Muted:   #606774  (Timestamp & Metadata Gray)              |
+--------------------------------------------------------------------------+
```

---

## 2. Typography System

- **Primary Font Family:** System Modern Sans (`Segoe UI Variable Text`, `Inter`, `-apple-system`, `Roboto`).
- **Monospace Font Family (Telemetry & Latency):** `Consolas`, `JetBrains Mono`, `Cascadia Code`.

### Scale & Hierarchy:
- **Display Header:** 24px / Bold (700) / Tracking: -0.5px (Section titles, app title)
- **Card Title:** 16px / SemiBold (600) / Tracking: -0.2px (Service names: Discord, YouTube)
- **Body Regular:** 14px / Regular (400) / Line Height: 20px (Diagnostic descriptions, tips)
- **Badge & Metric:** 12px / Medium (500) / Uppercase / Tracking: +0.5px (Status pills, pings)
- **Code / Monospace:** 12px / Regular (400) / Tabular figures (Latency: `34 ms`, Jitter: `2.1 ms`)

---

## 3. Spatial System & Corner Radii

- **Grid Unit:** 4px baseline grid.
  - Spacing Scale: `4px` (xxs), `8px` (xs), `12px` (sm), `16px` (md), `24px` (lg), `32px` (xl), `48px` (2xl).
- **Radius System:**
  - `Radius-Sm`: 6px (Badges, small status pills, tooltips)
  - `Radius-Md`: 10px (Buttons, input fields, control toggles)
  - `Radius-Lg`: 16px (Primary service cards, modal dialogs, gaming radar panels)
  - `Radius-Full`: 9999px (Circular status indicators, avatar pills)

---

## 4. UI Components & Patterns

### 4.1 The Service Status Card (`ServiceCard`)
- **Layout:** Horizontal split or vertical card container with 1px `#2A2E39` border, `#1D2027` background.
- **Left:** High-resolution vector platform glyph + Service Name + Category Tag (Voice, Video, Gaming, Dev).
- **Right:** Optical Status Pill + Latency Metric (`38 ms`) + Chevron to expand Technical Details.
- **Expanded Details Drawer:** Displays the 4-layer inspection summary:
  * *DNS:* Resolved `162.159.130.233` via DoH Cloudflare (0.8ms)
  * *TCP SYN:* Connected on port 443 (28ms)
  * *TLS Handshake:* RST packet injected by TSPU (WSAECONNRESET 10054)
  * *Diagnosis:* **Blocked by ISP TSPU via SNI filtering**.

### 4.2 The Gaming Latency Radar (`GamingRadarCard`)
- Displays real-time ping sparklines for popular game server regions:
  * CS2 (Stockholm / Warsaw / Frankfurt)
  * Dota 2 (Europe East / Europe West)
  * Roblox (EU Primary Gateway)
  * Minecraft (Hypixel / Local RU Nodes)
- Visualized with high-contrast sparkline charts and instant packet loss percentage badges.

### 4.3 Tactical Actions Bar
- Instant actions:
  - **"Полная проверка" (Full Diagnosis):** Runs parallel asynchronous health probes across all services.
  - **"Тест игр" (Game Radar):** Continuous ping stream to gaming servers.
  - **"Экспорт отчёта" (Export Report):** Generates redacted diagnostic markdown log.
  - **"Очистить DNS-кэш" (Flush DNS Cache):** Fast local network maintenance.

---

## 5. Motion, Physics & Responsiveness

- **Target Framerate:** Constant 60 FPS on standard hardware.
- **Timing Functions:** Cubic Bezier `(0.16, 1, 0.3, 1)` (Fluid Apple/Linear-style ease-out).
- **Durations:**
  - Card hover scale & glow: 150ms.
  - Drawer accordion expand: 250ms.
  - Status pulse animation: 1.8s looping opacity breath (`0.6` -> `1.0` -> `0.6`) when actively probing.
- **Accessibility:** Full respect for `ReducedMotion` OS flag. When enabled, transitions snap instantaneously without spring animations.
