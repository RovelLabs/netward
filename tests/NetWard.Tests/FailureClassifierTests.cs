using NetWard.Core.Diagnostics;
using NetWard.Core.Models;
using Xunit;

namespace NetWard.Tests;

public class FailureClassifierTests
{
    private readonly FailureClassifier _classifier = new();

    [Fact]
    public void Classify_WhenTlsThrows10054_ReturnsTspuSniBlock()
    {
        var profile = new ServiceProfile { Id = "discord", DisplayName = "Discord", Category = ServiceCategory.Voice };
        var layers = new List<LayerProbeResult>
        {
            new() { Layer = ProbeLayer.Dns, IsSuccess = true, LatencyMs = 15, ResolvedIps = new() { "162.159.130.233" } },
            new() { Layer = ProbeLayer.TcpHandshake, IsSuccess = true, LatencyMs = 30 },
            new() { Layer = ProbeLayer.TlsHandshake, IsSuccess = false, LatencyMs = 40, SocketErrorCode = 10054, ErrorMessage = "Connection reset by peer" }
        };

        var verdict = _classifier.Classify(profile, layers);

        Assert.Equal(ServiceStatus.Blocked, verdict.Status);
        Assert.Equal(FailureCause.TspuSniBlock, verdict.FailureCause);
        Assert.True(verdict.ConfidencePercent >= 95);
        Assert.Contains("ТСПУ", verdict.SummaryRu);
        Assert.Contains("TSPU", verdict.SummaryEn);
    }

    [Fact]
    public void Classify_WhenDnsReturnsLoopback_ReturnsDnsPoisoned()
    {
        var profile = new ServiceProfile { Id = "testsite", DisplayName = "Test Site" };
        var layers = new List<LayerProbeResult>
        {
            new() { Layer = ProbeLayer.Dns, IsSuccess = true, ResolvedIps = new() { "127.0.0.1" } }
        };

        var verdict = _classifier.Classify(profile, layers);

        Assert.Equal(ServiceStatus.Blocked, verdict.Status);
        Assert.Equal(FailureCause.DnsPoisoned, verdict.FailureCause);
        Assert.Contains("DNS-подмена", verdict.SummaryRu);
    }

    [Fact]
    public void Classify_WhenHttpReturns451_ReturnsIspBlockPage()
    {
        var profile = new ServiceProfile { Id = "blockedurl", DisplayName = "Blocked URL" };
        var layers = new List<LayerProbeResult>
        {
            new() { Layer = ProbeLayer.Dns, IsSuccess = true, ResolvedIps = new() { "1.2.3.4" } },
            new() { Layer = ProbeLayer.TcpHandshake, IsSuccess = true, LatencyMs = 25 },
            new() { Layer = ProbeLayer.TlsHandshake, IsSuccess = true, LatencyMs = 35 },
            new() { Layer = ProbeLayer.HttpLayer, IsSuccess = false, HttpStatusCode = 451 }
        };

        var verdict = _classifier.Classify(profile, layers);

        Assert.Equal(ServiceStatus.Blocked, verdict.Status);
        Assert.Equal(FailureCause.IspBlockPage, verdict.FailureCause);
    }

    [Fact]
    public void Classify_WhenHttpReturns503_ReturnsGlobalServerOutage()
    {
        var profile = new ServiceProfile { Id = "steam", DisplayName = "Steam" };
        var layers = new List<LayerProbeResult>
        {
            new() { Layer = ProbeLayer.Dns, IsSuccess = true, ResolvedIps = new() { "1.2.3.4" } },
            new() { Layer = ProbeLayer.TcpHandshake, IsSuccess = true, LatencyMs = 30 },
            new() { Layer = ProbeLayer.TlsHandshake, IsSuccess = true, LatencyMs = 40 },
            new() { Layer = ProbeLayer.HttpLayer, IsSuccess = false, HttpStatusCode = 503 }
        };

        var verdict = _classifier.Classify(profile, layers);

        Assert.Equal(ServiceStatus.ServerOutage, verdict.Status);
        Assert.Equal(FailureCause.GlobalServerOutage, verdict.FailureCause);
        Assert.Contains("серверах", verdict.SummaryRu);
    }

    [Fact]
    public void Classify_WhenStreamThroughputIsPoliced_ReturnsThrottled()
    {
        var profile = new ServiceProfile { Id = "youtube", DisplayName = "YouTube", Category = ServiceCategory.Video };
        var layers = new List<LayerProbeResult>
        {
            new() { Layer = ProbeLayer.Dns, IsSuccess = true, ResolvedIps = new() { "142.250.186.206" } },
            new() { Layer = ProbeLayer.TcpHandshake, IsSuccess = true, LatencyMs = 28 },
            new() { Layer = ProbeLayer.TlsHandshake, IsSuccess = true, LatencyMs = 35 },
            new() { Layer = ProbeLayer.HttpLayer, IsSuccess = true, HttpStatusCode = 200 },
            new() { Layer = ProbeLayer.ContentStream, IsSuccess = false, ThroughputKbps = 140.0, LatencyMs = 2500 }
        };

        var verdict = _classifier.Classify(profile, layers);

        Assert.Equal(ServiceStatus.Throttled, verdict.Status);
        Assert.Equal(FailureCause.ThrottledBandwidth, verdict.FailureCause);
        Assert.Contains("замедление", verdict.SummaryRu, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Classify_WhenAllLayersPassWithLowPing_ReturnsHealthy()
    {
        var profile = new ServiceProfile { Id = "github", DisplayName = "GitHub" };
        var layers = new List<LayerProbeResult>
        {
            new() { Layer = ProbeLayer.Dns, IsSuccess = true, ResolvedIps = new() { "140.82.121.3" } },
            new() { Layer = ProbeLayer.TcpHandshake, IsSuccess = true, LatencyMs = 35 },
            new() { Layer = ProbeLayer.TlsHandshake, IsSuccess = true, LatencyMs = 45 },
            new() { Layer = ProbeLayer.HttpLayer, IsSuccess = true, HttpStatusCode = 200 }
        };

        var verdict = _classifier.Classify(profile, layers);

        Assert.Equal(ServiceStatus.Healthy, verdict.Status);
        Assert.Equal(FailureCause.None, verdict.FailureCause);
    }
}
