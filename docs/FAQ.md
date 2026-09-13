# Frequently Asked Questions (FAQ)

### Q: Is NetWard a VPN or proxy?
**A:** No. NetWard is an **Internet Resilience & Diagnostics Companion**. It does not route your traffic through foreign servers or act as a VPN. Instead, it tests and explains why services are failing, monitors gaming ping, and helps maintain local network settings.

### Q: Is NetWard legal under Russian law?
**A:** Yes, 100%. Russian Federal Law No. 149-FZ and Roskomnadzor Order No. 168 prohibit the advertisement and distribution of tools and instructions specifically designed to bypass official blocks. Diagnostic utilities, latency monitors, and network measurement tools (such as *PingPlotter*, *Wireshark*, *Speedtest*, and *NetWard*) are standard network engineering software and fully legal.

### Q: Does NetWard collect my browsing history or personal data?
**A:** Never. NetWard has **zero tracking SDKs, zero analytics servers, and zero accounts**. All telemetry is generated locally and stored exclusively on your device. When you export a diagnostic report, NetWard automatically sanitizes and redacts all private and public IP addresses.

### Q: Why does YouTube say "Throttled" while other sites work?
**A:** Russian telecommunication operators subject Google Video CDN nodes to artificial traffic policing (dropping packets above ~128-250 kbps). NetWard measures the actual transfer bitrate and detects when throughput has been policed below streaming quality thresholds.

### Q: Can NetWard fix a blocked service automatically?
**A:** NetWard provides honest advice. If a service is blocked by TSPU via SNI drops, NetWard tells you honestly that the failure is at the ISP level and that restarting your Wi-Fi router will not solve it. For DNS-level issues, NetWard provides a one-click button to flush the DNS resolver cache.

### Q: How do I test NetWard if I am not currently in Russia?
**A:** NetWard has built-in **Simulation Modes**! You can select "Симуляция: Блок Discord (ТСПУ)" or "Симуляция: Замедление YouTube" in the UI dropdown or run `netward simulate discord` in the CLI to see the exact diagnostic workflow.
