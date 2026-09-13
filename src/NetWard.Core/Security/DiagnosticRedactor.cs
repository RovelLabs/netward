using System.Text;
using System.Text.RegularExpressions;
using NetWard.Core.Models;

namespace NetWard.Core.Security;

public interface IDiagnosticRedactor
{
    string RedactText(string input);
    string ExportReportToMarkdown(DiagnosticReport report);
}

public class DiagnosticRedactor : IDiagnosticRedactor
{
    private static readonly Regex Ipv4Regex = new(
        @"\b(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\b",
        RegexOptions.Compiled);

    private static readonly Regex UserPathRegex = new(
        @"(?i)(C:\\Users\\)[^\\]+(\\)",
        RegexOptions.Compiled);

    public string RedactText(string input)
    {
        if (string.IsNullOrEmpty(input)) return string.Empty;

        // 1. Redact username in local paths
        string result = UserPathRegex.Replace(input, "$1[USER]$2");

        // 2. Redact Machine Name if present
        try
        {
            string machineName = Environment.MachineName;
            if (!string.IsNullOrEmpty(machineName) && machineName.Length > 2)
            {
                result = result.Replace(machineName, "[REDACTED_HOST]", StringComparison.OrdinalIgnoreCase);
            }
        }
        catch { }

        // 3. Redact private IP addresses to preserve anonymity
        result = Ipv4Regex.Replace(result, match =>
        {
            string ip = match.Value;
            if (ip.StartsWith("127.")) return ip; // Keep loopback identifiable
            if (ip.StartsWith("192.168.") || ip.StartsWith("10.") || (ip.StartsWith("172.") && Is172Private(ip)))
            {
                return "[REDACTED_LAN_IP]";
            }
            // Mask 3rd and 4th octet of public IPs: 185.25.x.x
            var parts = ip.Split('.');
            if (parts.Length == 4)
            {
                return $"{parts[0]}.{parts[1]}.*.*";
            }
            return "[REDACTED_IP]";
        });

        return result;
    }

    public string ExportReportToMarkdown(DiagnosticReport report)
    {
        var sb = new StringBuilder();
        sb.AppendLine("# 🛡️ NetWard Diagnostic Telemetry Report");
        sb.AppendLine($"**Report ID:** `{report.ReportId}`  ");
        sb.AppendLine($"**Generated (UTC):** {report.GeneratedAt:yyyy-MM-dd HH:mm:ss}  ");
        sb.AppendLine($"**Host Environment:** {RedactText(report.AnonymizedHost)}  ");
        sb.AppendLine($"**Privacy Level:** 100% Anonymized & Redacted (Safe for GitHub issues)");
        sb.AppendLine();
        sb.AppendLine("---");
        sb.AppendLine();

        sb.AppendLine("## 1. Services Reachability Summary");
        sb.AppendLine("| Service | Category | Status | Primary Cause | Confidence | Latency |");
        sb.AppendLine("| :--- | :--- | :--- | :--- | :---: | :---: |");

        foreach (var v in report.Verdicts)
        {
            string statusIcon = v.Status switch
            {
                ServiceStatus.Healthy => "🟢 HEALTHY",
                ServiceStatus.Degraded => "🟡 DEGRADED",
                ServiceStatus.Blocked => "🔴 BLOCKED",
                ServiceStatus.Throttled => "🟣 THROTTLED",
                ServiceStatus.ServerOutage => "⚪ SERVER OUTAGE",
                _ => "⚪ UNKNOWN"
            };

            sb.AppendLine($"| **{v.DisplayName}** | {v.Category} | {statusIcon} | `{v.FailureCause}` | {v.ConfidencePercent}% | {v.OverallLatencyMs:F0} ms |");
        }

        sb.AppendLine();
        sb.AppendLine("## 2. Granular Service Inspection Details");

        foreach (var v in report.Verdicts)
        {
            sb.AppendLine($"### {RedactText(v.DisplayName)}");
            sb.AppendLine($"- **Summary (RU):** {RedactText(v.SummaryRu)}");
            sb.AppendLine($"- **Summary (EN):** {RedactText(v.SummaryEn)}");
            sb.AppendLine($"- **Technical Diagnosis:** {RedactText(v.TechnicalDetailsRu)}");
            sb.AppendLine($"- **Actionable Advice:** {RedactText(v.RecommendationRu)}");
            sb.AppendLine("- **Layer Probes:**");

            foreach (var lp in v.LayerResults)
            {
                string ok = lp.IsSuccess ? "✅ PASS" : "❌ FAIL";
                sb.AppendLine($"  * `[{lp.Layer}]` {ok} ({lp.LatencyMs:F1}ms) — {RedactText(lp.Details)}");
            }
            sb.AppendLine();
        }

        if (report.GamePings.Count > 0)
        {
            sb.AppendLine("## 3. Gaming Radar Telemetry");
            sb.AppendLine("| Game Target | Region | Min Ping | Avg Ping | Max Ping | Jitter | Packet Loss | Status |");
            sb.AppendLine("| :--- | :--- | :---: | :---: | :---: | :---: | :---: | :---: |");

            foreach (var g in report.GamePings)
            {
                string state = g.IsHealthy ? "🟢 Optimal" : "🔴 Degraded";
                sb.AppendLine($"| **{g.GameName}** | {g.Region} | {g.MinLatencyMs}ms | {g.AvgLatencyMs}ms | {g.MaxLatencyMs}ms | ±{g.JitterMs}ms | {g.PacketLossPct}% | {state} |");
            }
            sb.AppendLine();
        }

        if (report.DnsComparisons.Count > 0)
        {
            sb.AppendLine("## 4. DNS Dual-Resolution Verification");
            foreach (var dns in report.DnsComparisons)
            {
                string flag = dns.IsDivergent ? "⚠️ DIVERGENT (Possible Filtering)" : "✅ CONSISTENT";
                sb.AppendLine($"- **{dns.Hostname}:** {flag}");
                sb.AppendLine($"  * Explanation: {dns.Explanation}");
                sb.AppendLine($"  * Local DNS IPs: {string.Join(", ", dns.LocalDnsIps.Select(RedactText))}");
                sb.AppendLine($"  * DoH Cloudflare IPs: {string.Join(", ", dns.CloudflareDohIps.Select(RedactText))}");
            }
            sb.AppendLine();
        }

        sb.AppendLine("---");
        sb.AppendLine("*Generated automatically by NetWard — Open-Source Internet Resilience & Diagnostic Companion.*");

        return sb.ToString();
    }

    private static bool Is172Private(string ip)
    {
        var parts = ip.Split('.');
        if (parts.Length > 1 && int.TryParse(parts[1], out int second))
        {
            return second >= 16 && second <= 31;
        }
        return false;
    }
}
