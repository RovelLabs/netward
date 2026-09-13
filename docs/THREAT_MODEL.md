# Threat Model & Security Architecture

**Document Metadata:**
- **Product:** NetWard
- **Document Version:** 1.0.0
- **Standard:** STRIDE & Threat Modeling Framework
- **Date:** September 13, 2026

---

## 1. System Scope & Trust Boundaries

```
[Untrusted Internet / ISP / TSPU]
              |
              v (Spoofed RST / Poisoned DNS / Throttled Packets)
+--------------------------------------------------------------+
| Host Machine OS (Windows 10/11)                              |
|                                                              |
|   +-------------------------------------------------------+  |
|   | NetWard Runtime Boundary                              |  |
|   |                                                       |  |
|   |  [UI Layer (WPF / XAML)]                              |  |
|   |          ^                                            |  |
|   |          | Safe MVVM Bindings                         |  |
|   |          v                                            |  |
|   |  [Core Diagnostic Engine]                             |  |
|   |    - Socket Prober (TCP/TLS/HTTP)                     |  |
|   |    - DoH Resolver Validator                           |  |
|   |    - Game Latency Tracker                             |  |
|   |    - Redacted Diagnostic Exporter                     |  |
|   |          |                                            |  |
|   |          v Isolated Local I/O                         |  |
|   |  [Local File Storage: %LOCALAPPDATA%\NetWard]         |  |
|   |    - config.json (Strict Schema Validation)           |  |
|   +-------------------------------------------------------+  |
+--------------------------------------------------------------+
```

---

## 2. Threat Analysis & Mitigations Matrix

| Threat Category & Vector | Description & Attack Scenario | Risk Level | Architectural Mitigation in NetWard |
| :--- | :--- | :--- | :--- |
| **1. Malicious Update Attack** | An attacker gains access to a CDN or distribution point and pushes a compromised update binary. | **CRITICAL** | Updates are **never executed silently or automatically**. Releases publish cryptographic SHA-256 hashes (`checksums.txt`) signed with GPG keys. Update checks verify HTTPS certificates and prompt user with exact SHA-256 verification instructions. |
| **2. Compromised GitHub Account** | Adversary compromises maintainer credentials to publish a malicious release tag. | **HIGH** | Strict 2FA with hardware FIDO2 keys mandated for maintainers; branch protection rules on `main`; signed git commits (`git commit -S`); multi-maintainer review required for any release tag trigger. |
| **3. MITM & Spoofed TCP Resets** | TSPU or local rogue network intercepts connections and injects fake packets (RST / HTTP 451). | **MEDIUM** | NetWard is designed precisely to *detect* this. The socket prober catches `WSAECONNRESET` and mismatched TLS certificates, correctly flagging them as TSPU interference rather than trusting the injected data. |
| **4. DNS Manipulation / Poisoning** | Local ISP DNS injects false IP addresses or NXDOMAIN for prohibited services. | **HIGH** | NetWard does not rely solely on system DNS (`Dns.GetHostAddressesAsync`). It queries trusted Encrypted DNS-over-HTTPS (DoH) endpoints (Cloudflare/Quad9) to cross-reference resolution against the local ISP DNS. Discrepancies are flagged as DNS Poisoning. |
| **5. Malicious Configuration Injection** | A malicious actor crafts a malformed `config.json` or registry file attempting path traversal or code injection. | **MEDIUM** | Strict JSON schema parsing with strongly-typed deserialization (`System.Text.Json`). File paths are sandboxed within `%LOCALAPPDATA%\NetWard`; external paths are rejected. Invalid configs gracefully fallback to immutable default definitions. |
| **6. Fake Releases / Phishing Mirrors** | Attackers create copycat repos or Telegram channels distributing malware under the NetWard name. | **HIGH** | Official GitHub repository is permanently anchored at `https://github.com/RovelLabs/new-prpect`. Release artifacts are published with reproducible build instructions and SHA-256 hashes. Security advisories warn against third-party mirrors. |
| **7. Tracking & Metadata Leakage** | User network activity or visited services leaked to third-party observers. | **HIGH** | Zero external telemetry servers. Diagnostic checks probe public endpoints directly without proxying. Export feature includes an automated redaction engine that masks local IP addresses, machine names, and private network prefixes before output. |
| **8. Corrupted Local Storage** | Sudden power loss or crash corrupts local configuration, causing startup freeze. | **LOW** | Atomic file writes: configuration writes to a temporary file (`config.json.tmp`) and performs an atomic replace. If deserialization fails, NetWard resets to default state and notifies the user safely. |
| **9. Supply-Chain Dependency Attack** | Malicious third-party NuGet package introduced into build pipeline. | **CRITICAL** | Zero dependency bloat philosophy: NetWard relies almost entirely on core .NET BCL (`System.Net`, `System.Text.Json`, `System.Threading`). Third-party dependencies are strictly pinned with SHA-512 package lockfiles (`packages.lock.json`) and scanned via GitHub Dependency Review & Dependabot. |
| **10. Leaked API Secrets** | Hardcoded developer secrets or cloud keys leaked in Git history. | **CRITICAL** | NetWard requires **zero proprietary backend APIs**. It has no cloud database, no API keys, and no secret tokens. Secret scanning workflows fail any build containing candidate credentials. |
| **11. Compromised Signing Key** | Code signing private key stolen by adversary. | **HIGH** | Signing keys are never committed to git or stored on developer workstations. CI uses ephemeral GitHub Secrets with hardware HSM abstraction when code signing is enabled. |

---

## 3. Vulnerability Disclosure Policy

Security vulnerabilities must be reported through GitHub Private Security Advisories or via email to `security@rovel.org`. Maintainers acknowledge reports within 48 hours and release patched security advisories within 7 days.
