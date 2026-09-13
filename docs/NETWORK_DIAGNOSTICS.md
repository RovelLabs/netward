# Technical Guide to NetWard Network Diagnostics

This document provides a deep-dive into how NetWard conducts multi-layer socket inspection, differentiates between failure modes, and detects Russian TSPU DPI filtering.

---

## 1. The 5-Layer Inspection Sequence

```
1. DNS Layer
   ├── Query System DNS (Dns.GetHostAddressesAsync)
   ├── Query Cloudflare DoH (1.1.1.1 / application/dns-json)
   └── Detect Loopback (127.0.0.1) or NXDOMAIN Poisoning

2. Transport Layer (TCP)
   ├── Construct raw TCP socket (SocketType.Stream, ProtocolType.Tcp)
   ├── Issue asynchronous ConnectAsync to port 443 with 3.0s timeout
   └── Measure exact SYN-ACK round-trip time (RTT)

3. Presentation / Cryptographic Layer (TLS)
   ├── Wrap NetworkStream in System.Net.Security.SslStream
   ├── Send TLS ClientHello with explicit TargetHost SNI string
   └── CATCH TSPU INJECTION: Inspect for SocketError.ConnectionReset (10054)

4. Application Layer (HTTP)
   ├── Send HTTP GET with standard browser User-Agent
   ├── Evaluate HTTP Status:
   │   ├── 451: Formal ISP regulatory block page
   │   ├── 403: Provider geo-restriction (OpenAI Cloudflare block)
   │   └── 500-504: Cloud service provider outage
   └── Measure TTFB (Time To First Byte)

5. Streaming Throughput Layer (Throttling Check)
   ├── Stream initial video chunk from CDN
   └── Flag THROTTLED if throughput drops below 250 kbps with latency > 1.5s
```

---

## 2. TSPU RST Injection: How It Is Detected

When an operator installs a TSPU (Технические средства противодействия угрозам) hardware filter on transit uplinks:
- The initial TCP 3-way handshake to the remote IP succeeds because TSPU allows the TCP SYN packet through.
- However, as soon as the client transmits the `TLS ClientHello` packet containing a domain like `discord.com` in the Server Name Indication (SNI) extension, the TSPU deep packet inspection engine triggers.
- Instead of silently dropping packets, TSPU immediately spoofs and transmits an out-of-band `TCP RST` (Reset) packet with spoofed sequence numbers to both the client and server.
- On Windows sockets, this manifests as `SocketError.ConnectionReset` (`WSAECONNRESET` error code `10054`).
- NetWard catches this exact error state and cross-references it with successful TCP SYN to conclude with **96%+ confidence** that TSPU filtering is active.
