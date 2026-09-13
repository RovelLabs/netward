# Architecture Decision Record (ADR 001): Technology Stack Selection

**Document Metadata:**
- **Status:** APPROVED
- **Date:** September 13, 2026
- **Decider:** Senior Software Architect & Engineering Lead

---

## 1. Context & Problem Statement

We are building **NetWard**, an open-source, local-first internet resilience and diagnostic companion for Russian teenagers (13–19) and power users. The application must satisfy rigorous technical and experiential constraints:
1. **Zero Browser / Non-Web Requirement:** Strictly prohibited from using generic Electron, web wrappers, or PWA disguised as native binaries.
2. **Deep Networking & OS Capabilities:** Requires low-level asynchronous socket control (`System.Net.Sockets`), precise nanosecond/millisecond timing for jitter and ping, raw TLS ClientHello inspection, custom encrypted DNS-over-HTTPS (DoH) querying, and packet loss measurement.
3. **Cross-Platform Vision:** Primary target is Windows desktop (the predominant platform for Russian PC gamers playing CS2, Dota 2, Roblox, Discord), with architectural design supporting mobile (Android/iOS) and Unix platforms (Linux/macOS).
4. **Performance & Footprint:** Cold start < 1.0 second, idle RAM < 50MB, zero lag at 60+ FPS animations, single-file deployable without requiring external runtime installations.
5. **Developer & Contributor Accessibility:** Clean build system, cross-platform CLI, extensive unit testing framework, and automated CI/CD pipeline on GitHub Actions.

---

## 2. Considered Alternatives

### Alternative A: Rust Core + Tauri / Native OS GUI
- **Pros:** Maximum memory safety, blazing speed, minimal binary size.
- **Cons:** Tauri uses system WebView (MSHTML / WebView2 / WebKit), violating the strict prompt directive against web wrappers ("Не используй Electron/Tauri/встроенный Chromium"). Native Rust GUIs (Iced, Slint, egui) lack mature accessibility, complex text layout, and native Windows touch/input paradigms.

### Alternative B: Flutter (Dart)
- **Pros:** Excellent cross-platform UI code sharing between mobile and desktop.
- **Cons:** Flutter desktop on Windows requires C++ CMake scaffolding, heavy binary bundle sizes, and Dart's asynchronous socket API offers limited control over low-level TCP connection states and TCP RST inspection. Furthermore, Flutter toolchain is not pre-installed in the current host environment.

### Alternative C: Kotlin Multiplatform (KMP) + Compose Multiplatform
- **Pros:** Native Android integration, sharing business logic with desktop.
- **Cons:** Heavy JVM dependency for desktop execution unless compiled via Kotlin/Native; complex Gradle multi-project setup with high memory consumption on developer machines.

### Alternative D: C# / .NET 8 / .NET 10 (Chosen Architecture)
- **Architecture:**
  * **Core Logic (`NetWard.Core`):** High-performance, cross-platform .NET library targeting `.NET 8.0 / .NET 10.0`. Runs natively on Windows, Linux, macOS, and mobile.
  * **Desktop Client (`NetWard.App`):** Hardware-accelerated native Windows Desktop UI (WPF / XAML) built with custom graphite design system, fluid vector motion, zero Electron overhead, and native accessibility.
  * **CLI Companion (`NetWard.Cli`):** Instant cross-platform terminal diagnostic tool for headless environments and developers.
  * **Test Suite (`NetWard.Tests`):** xUnit testing framework with network simulation mocks.
- **Why it wins:**
  * Standard in high-performance networking software (e.g. Shadowsocks-Windows, v2rayN, ASP.NET Core Kestrel).
  * Direct OS socket APIs (`SocketAsyncEventArgs`, raw TCP probes, TLS handshake timers).
  * Native single-file compilation (`PublishSingleFile=true`, self-contained) creates a portable, fast `.exe` with zero dependencies for the end user.
  * Host environment possesses first-class .NET SDK 8 and 10 with WindowsDesktop runtimes ready.

---

## 3. Decision Outcome

**Chosen Stack:** Modern **C# (.NET 8.0 LTS / .NET 10.0)** modular architecture:
- `NetWard.Core`: Independent, platform-agnostic business and network diagnostic engine.
- `NetWard.App`: Native Windows Presentation Foundation (WPF) GUI with dark graphite visual language.
- `NetWard.Cli`: Cross-platform CLI runner.
- `NetWard.Tests`: Comprehensive xUnit testing suite.

---

## 4. Pros and Cons of the Decision

### Positive Consequences
- **True Native Windows Experience:** Flawless rendering, standard Windows window frame handling, tray icon minimization, system DPI awareness, and zero Chromium bloat.
- **Precise Socket Control:** Capable of measuring TCP SYN handshake latency, catching raw `SocketException` error codes (such as `WSAECONNRESET` 10054 which signals TSPU RST injection), and measuring TLS SNI negotiation failure with millisecond accuracy.
- **Single-File Self-Contained Deployment:** Can be published as a self-contained single executable (`NetWard.exe`) that runs on any modern Windows 10/11 machine without asking the user to install any runtime.

### Negative Consequences & Mitigations
- *Cross-platform mobile UI requires separate frontend:* The `NetWard.Core` library is 100% portable to Android and iOS via .NET MAUI or Kotlin/Swift wrappers, but the desktop WPF UI is Windows-specific. This matches our prioritized roadmap where Windows PC gaming is the primary hotspot for Discord/YouTube blocks.
