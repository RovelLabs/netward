using System.Diagnostics;
using System.Net.Sockets;
using NetWard.Core.Models;

namespace NetWard.Core.Diagnostics;

public interface IGameRadar
{
    Task<GamePingResult> PingTargetAsync(GameServerTarget target, int sampleCount = 4, CancellationToken ct = default);
    Task<List<GamePingResult>> PingAllTargetsAsync(IEnumerable<GameServerTarget> targets, CancellationToken ct = default);
}

public class GameRadar : IGameRadar
{
    private readonly TimeSpan _sampleTimeout = TimeSpan.FromMilliseconds(800);

    public async Task<GamePingResult> PingTargetAsync(GameServerTarget target, int sampleCount = 4, CancellationToken ct = default)
    {
        var samples = new List<double>();
        int failedSamples = 0;

        for (int i = 0; i < sampleCount; i++)
        {
            if (ct.IsCancellationRequested) break;

            double? latency = await MeasureSingleSampleAsync(target.Hostname, target.Port, _sampleTimeout, ct);
            if (latency.HasValue)
            {
                samples.Add(latency.Value);
            }
            else
            {
                failedSamples++;
            }

            // Brief pacing between packet bursts
            await Task.Delay(30, ct).ConfigureAwait(false);
        }

        double lossPct = (double)failedSamples / sampleCount * 100.0;
        double min = samples.Count > 0 ? samples.Min() : 0.0;
        double max = samples.Count > 0 ? samples.Max() : 0.0;
        double avg = samples.Count > 0 ? samples.Average() : 0.0;

        // Calculate jitter (mean deviation from average)
        double jitter = 0.0;
        if (samples.Count > 1)
        {
            jitter = samples.Select(s => Math.Abs(s - avg)).Average();
        }

        return new GamePingResult
        {
            GameName = target.GameName,
            Region = target.Region,
            MinLatencyMs = Math.Round(min, 1),
            AvgLatencyMs = Math.Round(avg, 1),
            MaxLatencyMs = Math.Round(max, 1),
            JitterMs = Math.Round(jitter, 1),
            PacketLossPct = Math.Round(lossPct, 1),
            Timestamp = DateTime.UtcNow
        };
    }

    public async Task<List<GamePingResult>> PingAllTargetsAsync(IEnumerable<GameServerTarget> targets, CancellationToken ct = default)
    {
        var tasks = targets.Select(t => PingTargetAsync(t, 4, ct)).ToList();
        var results = await Task.WhenAll(tasks);
        return results.ToList();
    }

    private async Task<double?> MeasureSingleSampleAsync(string host, int port, TimeSpan timeout, CancellationToken ct)
    {
        var sw = Stopwatch.StartNew();
        using var socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
        cts.CancelAfter(timeout);

        try
        {
            await socket.ConnectAsync(host, port, cts.Token);
            sw.Stop();
            return sw.Elapsed.TotalMilliseconds;
        }
        catch
        {
            return null;
        }
    }
}
