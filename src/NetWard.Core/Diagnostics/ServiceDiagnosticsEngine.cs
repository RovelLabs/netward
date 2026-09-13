using NetWard.Core.Models;
using NetWard.Core.Services;

namespace NetWard.Core.Diagnostics;

public interface IServiceDiagnosticsEngine
{
    ISocketProber SocketProber { get; set; }
    Task<ServiceHealthVerdict> DiagnoseServiceAsync(ServiceProfile profile, CancellationToken ct = default);
    Task<List<ServiceHealthVerdict>> DiagnoseAllAsync(IEnumerable<ServiceProfile> profiles, IProgress<int>? progress = null, CancellationToken ct = default);
}

public class ServiceDiagnosticsEngine : IServiceDiagnosticsEngine
{
    private readonly IFailureClassifier _classifier;
    public ISocketProber SocketProber { get; set; }

    public ServiceDiagnosticsEngine(ISocketProber? socketProber = null, IFailureClassifier? classifier = null)
    {
        SocketProber = socketProber ?? new SocketProber();
        _classifier = classifier ?? new FailureClassifier();
    }

    public async Task<ServiceHealthVerdict> DiagnoseServiceAsync(ServiceProfile profile, CancellationToken ct = default)
    {
        var layers = new List<LayerProbeResult>();
        var primaryEndpoint = profile.Endpoints.FirstOrDefault() ?? new ServiceEndpoint { Host = $"{profile.Id}.com", Port = 443 };

        // 1. DNS Probe
        var dnsResult = await SocketProber.ProbeDnsAsync(primaryEndpoint.Host, ct);
        layers.Add(dnsResult);

        // If DNS completely fails with offline error, return verdict immediately
        if (!dnsResult.IsSuccess && (dnsResult.ErrorMessage?.Contains("No network", StringComparison.OrdinalIgnoreCase) == true ||
                                     dnsResult.Details?.Contains("offline", StringComparison.OrdinalIgnoreCase) == true))
        {
            return _classifier.Classify(profile, layers);
        }

        // 2. TCP Probe
        var tcpResult = await SocketProber.ProbeTcpHandshakeAsync(primaryEndpoint.Host, primaryEndpoint.Port, TimeSpan.FromSeconds(3), ct);
        layers.Add(tcpResult);

        if (tcpResult.IsSuccess)
        {
            // 3. TLS Probe
            if (primaryEndpoint.UseTls)
            {
                var tlsResult = await SocketProber.ProbeTlsHandshakeAsync(primaryEndpoint.Host, primaryEndpoint.Port, TimeSpan.FromSeconds(4), ct);
                layers.Add(tlsResult);
            }

            // 4. HTTP Probe
            var httpResult = await SocketProber.ProbeHttpAsync(
                primaryEndpoint.Host,
                primaryEndpoint.Port,
                primaryEndpoint.UseTls,
                primaryEndpoint.HttpPath,
                TimeSpan.FromSeconds(4),
                ct);
            layers.Add(httpResult);

            // 5. Streaming Probe (for YouTube or video services)
            if (primaryEndpoint.CheckStreaming || profile.Category == ServiceCategory.Video)
            {
                var streamResult = await SocketProber.ProbeStreamingThroughputAsync(primaryEndpoint.Host, TimeSpan.FromSeconds(5), ct);
                layers.Add(streamResult);
            }
        }

        return _classifier.Classify(profile, layers);
    }

    public async Task<List<ServiceHealthVerdict>> DiagnoseAllAsync(
        IEnumerable<ServiceProfile> profiles,
        IProgress<int>? progress = null,
        CancellationToken ct = default)
    {
        var profileList = profiles.ToList();
        var verdicts = new List<ServiceHealthVerdict>();
        using var semaphore = new SemaphoreSlim(3); // Conservative parallel probe concurrency

        int completed = 0;
        var tasks = profileList.Select(async profile =>
        {
            await semaphore.WaitAsync(ct);
            try
            {
                var verdict = await DiagnoseServiceAsync(profile, ct);
                lock (verdicts)
                {
                    verdicts.Add(verdict);
                }
                int count = Interlocked.Increment(ref completed);
                progress?.Report((int)((double)count / profileList.Count * 100.0));
            }
            finally
            {
                semaphore.Release();
            }
        });

        await Task.WhenAll(tasks);
        return verdicts.OrderBy(v => v.DisplayName).ToList();
    }
}
