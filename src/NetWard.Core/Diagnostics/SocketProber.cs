using System.Diagnostics;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using NetWard.Core.Models;

namespace NetWard.Core.Diagnostics;

public class SocketProber : ISocketProber
{
    private readonly HttpClient _httpClient;

    public SocketProber(HttpClient? httpClient = null)
    {
        _httpClient = httpClient ?? new HttpClient(new SocketsHttpHandler
        {
            AllowAutoRedirect = false,
            ConnectTimeout = TimeSpan.FromSeconds(5)
        });
    }

    public async Task<LayerProbeResult> ProbeDnsAsync(string host, CancellationToken ct = default)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            var addresses = await Dns.GetHostAddressesAsync(host, ct);
            sw.Stop();

            var ips = addresses.Select(a => a.ToString()).ToList();
            if (ips.Count == 0)
            {
                return new LayerProbeResult
                {
                    Layer = ProbeLayer.Dns,
                    IsSuccess = false,
                    LatencyMs = sw.Elapsed.TotalMilliseconds,
                    ErrorMessage = "No IP addresses resolved for hostname",
                    Details = "DNS query returned 0 records."
                };
            }

            return new LayerProbeResult
            {
                Layer = ProbeLayer.Dns,
                IsSuccess = true,
                LatencyMs = sw.Elapsed.TotalMilliseconds,
                ResolvedIps = ips,
                Details = $"Resolved {ips.Count} IP addresses ({string.Join(", ", ips.Take(3))})"
            };
        }
        catch (Exception ex)
        {
            sw.Stop();
            return new LayerProbeResult
            {
                Layer = ProbeLayer.Dns,
                IsSuccess = false,
                LatencyMs = sw.Elapsed.TotalMilliseconds,
                ErrorMessage = ex.Message,
                Details = $"DNS resolution failed: {ex.GetType().Name}"
            };
        }
    }

    public async Task<LayerProbeResult> ProbeTcpHandshakeAsync(string host, int port, TimeSpan timeout, CancellationToken ct = default)
    {
        var sw = Stopwatch.StartNew();
        using var socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
        cts.CancelAfter(timeout);

        try
        {
            await socket.ConnectAsync(host, port, cts.Token);
            sw.Stop();

            return new LayerProbeResult
            {
                Layer = ProbeLayer.TcpHandshake,
                IsSuccess = true,
                LatencyMs = sw.Elapsed.TotalMilliseconds,
                Details = $"TCP SYN-ACK established with {host}:{port}"
            };
        }
        catch (SocketException sex)
        {
            sw.Stop();
            return new LayerProbeResult
            {
                Layer = ProbeLayer.TcpHandshake,
                IsSuccess = false,
                LatencyMs = sw.Elapsed.TotalMilliseconds,
                SocketErrorCode = (int)sex.SocketErrorCode,
                ErrorMessage = sex.Message,
                Details = $"TCP connection failed: {sex.SocketErrorCode} ({(int)sex.SocketErrorCode})"
            };
        }
        catch (OperationCanceledException)
        {
            sw.Stop();
            return new LayerProbeResult
            {
                Layer = ProbeLayer.TcpHandshake,
                IsSuccess = false,
                LatencyMs = sw.Elapsed.TotalMilliseconds,
                SocketErrorCode = (int)SocketError.TimedOut,
                ErrorMessage = "Connection timed out",
                Details = $"TCP connection timed out after {timeout.TotalSeconds:F1}s"
            };
        }
        catch (Exception ex)
        {
            sw.Stop();
            return new LayerProbeResult
            {
                Layer = ProbeLayer.TcpHandshake,
                IsSuccess = false,
                LatencyMs = sw.Elapsed.TotalMilliseconds,
                ErrorMessage = ex.Message,
                Details = $"TCP error: {ex.Message}"
            };
        }
    }

    public async Task<LayerProbeResult> ProbeTlsHandshakeAsync(string host, int port, TimeSpan timeout, CancellationToken ct = default)
    {
        var sw = Stopwatch.StartNew();
        using var socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
        cts.CancelAfter(timeout);

        try
        {
            await socket.ConnectAsync(host, port, cts.Token);
            using var networkStream = new NetworkStream(socket, ownsSocket: false);
            using var sslStream = new SslStream(networkStream, false, (sender, cert, chain, errors) => true);

            await sslStream.AuthenticateAsClientAsync(new SslClientAuthenticationOptions
            {
                TargetHost = host,
                EnabledSslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13
            }, cts.Token);

            sw.Stop();
            return new LayerProbeResult
            {
                Layer = ProbeLayer.TlsHandshake,
                IsSuccess = true,
                LatencyMs = sw.Elapsed.TotalMilliseconds,
                Details = $"TLS {sslStream.SslProtocol} handshake completed with SNI '{host}'"
            };
        }
        catch (Exception ex)
        {
            sw.Stop();
            int? socketError = null;
            if (ex is SocketException se)
            {
                socketError = (int)se.SocketErrorCode;
            }
            else if (ex.InnerException is SocketException ise)
            {
                socketError = (int)ise.SocketErrorCode;
            }

            bool isRst = socketError == (int)SocketError.ConnectionReset ||
                         ex.Message.Contains("forcibly closed", StringComparison.OrdinalIgnoreCase) ||
                         ex.Message.Contains("10054");

            string details = isRst
                ? "TCP RST received during TLS ClientHello (Classic TSPU / SNI injection pattern)"
                : $"TLS negotiation failed: {ex.Message}";

            return new LayerProbeResult
            {
                Layer = ProbeLayer.TlsHandshake,
                IsSuccess = false,
                LatencyMs = sw.Elapsed.TotalMilliseconds,
                SocketErrorCode = socketError,
                ErrorMessage = ex.Message,
                Details = details
            };
        }
    }

    public async Task<LayerProbeResult> ProbeHttpAsync(string host, int port, bool useTls, string path, TimeSpan timeout, CancellationToken ct = default)
    {
        var sw = Stopwatch.StartNew();
        string scheme = useTls ? "https" : "http";
        string url = $"{scheme}://{host}:{port}{path}";

        using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
        cts.CancelAfter(timeout);

        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) NetWard/1.0");

            using var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cts.Token);
            sw.Stop();

            int code = (int)response.StatusCode;
            bool isSuccess = code < 400 || code == 404 || code == 403;

            return new LayerProbeResult
            {
                Layer = ProbeLayer.HttpLayer,
                IsSuccess = isSuccess,
                LatencyMs = sw.Elapsed.TotalMilliseconds,
                HttpStatusCode = code,
                Details = $"HTTP {code} {response.ReasonPhrase}"
            };
        }
        catch (Exception ex)
        {
            sw.Stop();
            return new LayerProbeResult
            {
                Layer = ProbeLayer.HttpLayer,
                IsSuccess = false,
                LatencyMs = sw.Elapsed.TotalMilliseconds,
                ErrorMessage = ex.Message,
                Details = $"HTTP request failed: {ex.Message}"
            };
        }
    }

    public async Task<LayerProbeResult> ProbeStreamingThroughputAsync(string host, TimeSpan timeout, CancellationToken ct = default)
    {
        var sw = Stopwatch.StartNew();
        string url = $"https://{host}/";

        using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
        cts.CancelAfter(timeout);

        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) NetWard/1.0");

            using var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead, cts.Token);
            byte[] data = await response.Content.ReadAsByteArrayAsync(cts.Token);
            sw.Stop();

            double seconds = Math.Max(0.01, sw.Elapsed.TotalSeconds);
            double kbps = (data.Length * 8.0) / (seconds * 1000.0);

            bool throttled = kbps < 250.0 && seconds > 1.5;

            return new LayerProbeResult
            {
                Layer = ProbeLayer.ContentStream,
                IsSuccess = !throttled,
                LatencyMs = sw.Elapsed.TotalMilliseconds,
                ThroughputKbps = kbps,
                Details = $"Transferred {data.Length} bytes at {kbps:F1} kbps"
            };
        }
        catch (Exception ex)
        {
            sw.Stop();
            return new LayerProbeResult
            {
                Layer = ProbeLayer.ContentStream,
                IsSuccess = false,
                LatencyMs = sw.Elapsed.TotalMilliseconds,
                ErrorMessage = ex.Message,
                Details = $"Stream probe error: {ex.Message}"
            };
        }
    }
}
