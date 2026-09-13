using System.Net.Sockets;
using NetWard.Core.Models;

namespace NetWard.Core.Diagnostics;

public class SimulatedSocketProber : ISocketProber
{
    public SimulationMode Mode { get; set; } = SimulationMode.RealNetwork;

    public Task<LayerProbeResult> ProbeDnsAsync(string host, CancellationToken ct = default)
    {
        switch (Mode)
        {
            case SimulationMode.SimulateOffline:
                return Task.FromResult(new LayerProbeResult
                {
                    Layer = ProbeLayer.Dns,
                    IsSuccess = false,
                    LatencyMs = 2000,
                    ErrorMessage = "No network interfaces available",
                    Details = "Network interface offline"
                });

            case SimulationMode.SimulateDnsPoisoning:
                return Task.FromResult(new LayerProbeResult
                {
                    Layer = ProbeLayer.Dns,
                    IsSuccess = true,
                    LatencyMs = 12.0,
                    ResolvedIps = new List<string> { "127.0.0.1", "0.0.0.0" },
                    Details = "Poisoned DNS record returning localhost loopback"
                });

            default:
                return Task.FromResult(new LayerProbeResult
                {
                    Layer = ProbeLayer.Dns,
                    IsSuccess = true,
                    LatencyMs = 15.0,
                    ResolvedIps = new List<string> { "162.159.130.233", "162.159.133.233" },
                    Details = "Resolved 2 global Anycast IPs"
                });
        }
    }

    public Task<LayerProbeResult> ProbeTcpHandshakeAsync(string host, int port, TimeSpan timeout, CancellationToken ct = default)
    {
        switch (Mode)
        {
            case SimulationMode.SimulateOffline:
                return Task.FromResult(new LayerProbeResult
                {
                    Layer = ProbeLayer.TcpHandshake,
                    IsSuccess = false,
                    LatencyMs = timeout.TotalMilliseconds,
                    SocketErrorCode = (int)SocketError.NetworkUnreachable,
                    ErrorMessage = "Network unreachable",
                    Details = "Local network is offline"
                });

            default:
                return Task.FromResult(new LayerProbeResult
                {
                    Layer = ProbeLayer.TcpHandshake,
                    IsSuccess = true,
                    LatencyMs = 32.0,
                    Details = $"TCP SYN-ACK established with {host}:{port}"
                });
        }
    }

    public Task<LayerProbeResult> ProbeTlsHandshakeAsync(string host, int port, TimeSpan timeout, CancellationToken ct = default)
    {
        switch (Mode)
        {
            case SimulationMode.SimulateOffline:
                return Task.FromResult(new LayerProbeResult
                {
                    Layer = ProbeLayer.TlsHandshake,
                    IsSuccess = false,
                    LatencyMs = timeout.TotalMilliseconds,
                    ErrorMessage = "Network down"
                });

            case SimulationMode.SimulateDiscordBlocked when host.Contains("discord", StringComparison.OrdinalIgnoreCase):
                return Task.FromResult(new LayerProbeResult
                {
                    Layer = ProbeLayer.TlsHandshake,
                    IsSuccess = false,
                    LatencyMs = 45.0,
                    SocketErrorCode = (int)SocketError.ConnectionReset,
                    ErrorMessage = "An existing connection was forcibly closed by the remote host. (10054)",
                    Details = "TCP RST received during TLS ClientHello (Classic TSPU / SNI injection pattern)"
                });

            default:
                return Task.FromResult(new LayerProbeResult
                {
                    Layer = ProbeLayer.TlsHandshake,
                    IsSuccess = true,
                    LatencyMs = 38.0,
                    Details = $"TLS 1.3 handshake completed with SNI '{host}'"
                });
        }
    }

    public Task<LayerProbeResult> ProbeHttpAsync(string host, int port, bool useTls, string path, TimeSpan timeout, CancellationToken ct = default)
    {
        switch (Mode)
        {
            case SimulationMode.SimulateServerOutage:
                return Task.FromResult(new LayerProbeResult
                {
                    Layer = ProbeLayer.HttpLayer,
                    IsSuccess = false,
                    LatencyMs = 65.0,
                    HttpStatusCode = 503,
                    Details = "HTTP 503 Service Unavailable (Remote Cloud Outage)"
                });

            case SimulationMode.SimulateDiscordBlocked when host.Contains("discord", StringComparison.OrdinalIgnoreCase):
                return Task.FromResult(new LayerProbeResult
                {
                    Layer = ProbeLayer.HttpLayer,
                    IsSuccess = false,
                    LatencyMs = 40.0,
                    ErrorMessage = "Connection reset by peer",
                    Details = "HTTP failed due to underlying TLS RST"
                });

            default:
                return Task.FromResult(new LayerProbeResult
                {
                    Layer = ProbeLayer.HttpLayer,
                    IsSuccess = true,
                    LatencyMs = 48.0,
                    HttpStatusCode = 200,
                    Details = "HTTP 200 OK"
                });
        }
    }

    public Task<LayerProbeResult> ProbeStreamingThroughputAsync(string host, TimeSpan timeout, CancellationToken ct = default)
    {
        switch (Mode)
        {
            case SimulationMode.SimulateYouTubeThrottled:
                return Task.FromResult(new LayerProbeResult
                {
                    Layer = ProbeLayer.ContentStream,
                    IsSuccess = false,
                    LatencyMs = 2800.0,
                    ThroughputKbps = 118.0,
                    Details = "Severe throughput policing detected: 118 kbps (Below minimum 250 kbps threshold)"
                });

            default:
                return Task.FromResult(new LayerProbeResult
                {
                    Layer = ProbeLayer.ContentStream,
                    IsSuccess = true,
                    LatencyMs = 120.0,
                    ThroughputKbps = 4500.0,
                    Details = "Optimal streaming throughput: 4.5 Mbps"
                });
        }
    }
}
