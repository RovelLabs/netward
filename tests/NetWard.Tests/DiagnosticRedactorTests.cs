using NetWard.Core.Models;
using NetWard.Core.Security;
using Xunit;

namespace NetWard.Tests;

public class DiagnosticRedactorTests
{
    private readonly DiagnosticRedactor _redactor = new();

    [Fact]
    public void RedactText_MasksPrivateIps()
    {
        string raw = "Connecting to 192.168.1.1 and 10.0.0.5 for router stats";
        string redacted = _redactor.RedactText(raw);

        Assert.DoesNotContain("192.168.1.1", redacted);
        Assert.DoesNotContain("10.0.0.5", redacted);
        Assert.Contains("[REDACTED_LAN_IP]", redacted);
    }

    [Fact]
    public void RedactText_MasksPublicIpsPartially()
    {
        string raw = "Connected to remote server 185.25.182.1:27015";
        string redacted = _redactor.RedactText(raw);

        Assert.DoesNotContain("185.25.182.1", redacted);
        Assert.Contains("185.25.*.*", redacted);
    }

    [Fact]
    public void RedactText_MasksWindowsUsernames()
    {
        string raw = "Config loaded from C:\\Users\\IvanPetrov\\AppData\\Local\\NetWard\\settings.json";
        string redacted = _redactor.RedactText(raw);

        Assert.DoesNotContain("IvanPetrov", redacted);
        Assert.Contains("C:\\Users\\[USER]\\AppData", redacted);
    }

    [Fact]
    public void ExportReportToMarkdown_GeneratesValidMarkdownWithRedaction()
    {
        var report = new DiagnosticReport
        {
            AnonymizedHost = "DESKTOP-ABC1234",
            Verdicts = new List<ServiceHealthVerdict>
            {
                new()
                {
                    DisplayName = "Discord",
                    Category = ServiceCategory.Voice,
                    Status = ServiceStatus.Blocked,
                    FailureCause = FailureCause.TspuSniBlock,
                    ConfidencePercent = 96,
                    SummaryRu = "Сетевая блокировка ТСПУ",
                    SummaryEn = "TSPU filtering active",
                    TechnicalDetailsRu = "Сброс TCP RST на 192.168.1.10",
                    LayerResults = new List<LayerProbeResult>
                    {
                        new() { Layer = ProbeLayer.TcpHandshake, IsSuccess = true, LatencyMs = 30 },
                        new() { Layer = ProbeLayer.TlsHandshake, IsSuccess = false, LatencyMs = 40, Details = "RST on 192.168.1.10" }
                    }
                }
            },
            GamePings = new List<GamePingResult>
            {
                new() { GameName = "CS2", Region = "EU East", AvgLatencyMs = 35.2, JitterMs = 2.1, PacketLossPct = 0 }
            }
        };

        string md = _redactor.ExportReportToMarkdown(report);

        Assert.Contains("# 🛡️ NetWard Diagnostic Telemetry Report", md);
        Assert.Contains("Discord", md);
        Assert.Contains("CS2", md);
        Assert.DoesNotContain("192.168.1.10", md);
        Assert.Contains("[REDACTED_LAN_IP]", md);
    }
}
