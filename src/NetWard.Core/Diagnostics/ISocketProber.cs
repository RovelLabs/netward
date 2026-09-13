using NetWard.Core.Models;

namespace NetWard.Core.Diagnostics;

public interface ISocketProber
{
    Task<LayerProbeResult> ProbeDnsAsync(string host, CancellationToken ct = default);
    Task<LayerProbeResult> ProbeTcpHandshakeAsync(string host, int port, TimeSpan timeout, CancellationToken ct = default);
    Task<LayerProbeResult> ProbeTlsHandshakeAsync(string host, int port, TimeSpan timeout, CancellationToken ct = default);
    Task<LayerProbeResult> ProbeHttpAsync(string host, int port, bool useTls, string path, TimeSpan timeout, CancellationToken ct = default);
    Task<LayerProbeResult> ProbeStreamingThroughputAsync(string host, TimeSpan timeout, CancellationToken ct = default);
}
