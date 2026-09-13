using System.Text.Json.Serialization;

namespace NetWard.Core.Models;

public class ServiceEndpoint
{
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; } = 443;
    public bool UseTls { get; set; } = true;
    public string HttpPath { get; set; } = "/";
    public int ExpectedHttpStatus { get; set; } = 200;
    public bool CheckStreaming { get; set; } = false;
    public string Purpose { get; set; } = "Primary Gateway";
}

public class ServiceProfile
{
    public string Id { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public ServiceCategory Category { get; set; } = ServiceCategory.Custom;
    public string IconGlyph { get; set; } = "🌐";
    public string Description { get; set; } = string.Empty;
    public string KnownRestrictions { get; set; } = string.Empty;
    public List<ServiceEndpoint> Endpoints { get; set; } = new();
    public bool IsBuiltIn { get; set; } = true;
}

public class LayerProbeResult
{
    public ProbeLayer Layer { get; set; }
    public bool IsSuccess { get; set; }
    public double LatencyMs { get; set; }
    public string? ErrorMessage { get; set; }
    public int? HttpStatusCode { get; set; }
    public int? SocketErrorCode { get; set; }
    public List<string> ResolvedIps { get; set; } = new();
    public double? ThroughputKbps { get; set; }
    public string Details { get; set; } = string.Empty;
}

public class ServiceHealthVerdict
{
    public string ServiceId { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public ServiceCategory Category { get; set; }
    public ServiceStatus Status { get; set; } = ServiceStatus.Unknown;
    public FailureCause FailureCause { get; set; } = FailureCause.None;
    public int ConfidencePercent { get; set; } = 0;
    public double OverallLatencyMs { get; set; }
    public string SummaryRu { get; set; } = string.Empty;
    public string SummaryEn { get; set; } = string.Empty;
    public string TechnicalDetailsRu { get; set; } = string.Empty;
    public string TechnicalDetailsEn { get; set; } = string.Empty;
    public string RecommendationRu { get; set; } = string.Empty;
    public string RecommendationEn { get; set; } = string.Empty;
    public List<LayerProbeResult> LayerResults { get; set; } = new();
    public DateTime LastChecked { get; set; } = DateTime.UtcNow;
}

public class GameServerTarget
{
    public string GameName { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public string Hostname { get; set; } = string.Empty;
    public int Port { get; set; } = 27015;
    public string DisplayName => $"{GameName} ({Region})";
}

public class GamePingResult
{
    public string GameName { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public double MinLatencyMs { get; set; }
    public double AvgLatencyMs { get; set; }
    public double MaxLatencyMs { get; set; }
    public double JitterMs { get; set; }
    public double PacketLossPct { get; set; }
    public bool IsHealthy => PacketLossPct < 5.0 && AvgLatencyMs > 0 && AvgLatencyMs < 120.0;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

public class DnsComparisonResult
{
    public string Hostname { get; set; } = string.Empty;
    public List<string> LocalDnsIps { get; set; } = new();
    public List<string> CloudflareDohIps { get; set; } = new();
    public List<string> Quad9DohIps { get; set; } = new();
    public bool IsDivergent { get; set; }
    public string Explanation { get; set; } = string.Empty;
}

public class DiagnosticReport
{
    public string ReportId { get; set; } = Guid.NewGuid().ToString("N");
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    public string AnonymizedHost { get; set; } = "[REDACTED_HOST]";
    public string AnonymizedGateway { get; set; } = "[REDACTED_GATEWAY]";
    public List<string> DnsServers { get; set; } = new();
    public List<ServiceHealthVerdict> Verdicts { get; set; } = new();
    public List<GamePingResult> GamePings { get; set; } = new();
    public List<DnsComparisonResult> DnsComparisons { get; set; } = new();
    public List<string> OverallRecommendationsRu { get; set; } = new();
    public List<string> OverallRecommendationsEn { get; set; } = new();
}
