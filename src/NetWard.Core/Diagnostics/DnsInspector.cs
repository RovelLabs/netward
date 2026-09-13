using System.Net;
using System.Text.Json;
using NetWard.Core.Models;

namespace NetWard.Core.Diagnostics;

public interface IDnsInspector
{
    Task<DnsComparisonResult> CompareDnsResolutionAsync(string hostname, CancellationToken ct = default);
}

public class DnsInspector : IDnsInspector
{
    private readonly HttpClient _httpClient;

    public DnsInspector(HttpClient? httpClient = null)
    {
        _httpClient = httpClient ?? new HttpClient(new SocketsHttpHandler
        {
            ConnectTimeout = TimeSpan.FromSeconds(4)
        });
    }

    public async Task<DnsComparisonResult> CompareDnsResolutionAsync(string hostname, CancellationToken ct = default)
    {
        var result = new DnsComparisonResult { Hostname = hostname };

        // 1. Local system DNS
        try
        {
            var addresses = await Dns.GetHostAddressesAsync(hostname, ct);
            result.LocalDnsIps = addresses.Select(a => a.ToString()).Distinct().ToList();
        }
        catch
        {
            result.LocalDnsIps = new List<string>();
        }

        // 2. Cloudflare DoH (1.1.1.1)
        result.CloudflareDohIps = await QueryDohJsonAsync("https://cloudflare-dns.com/dns-query", hostname, ct);

        // 3. Quad9 DoH (9.9.9.9)
        result.Quad9DohIps = await QueryDohJsonAsync("https://dns.quad9.net/dns-query", hostname, ct);

        // Analysis
        bool localHasLoopback = result.LocalDnsIps.Any(ip => ip.StartsWith("127.") || ip == "0.0.0.0");
        bool localEmptyWhileDohHasIps = result.LocalDnsIps.Count == 0 && (result.CloudflareDohIps.Count > 0 || result.Quad9DohIps.Count > 0);

        if (localHasLoopback)
        {
            result.IsDivergent = true;
            result.Explanation = "Local DNS returned loopback/null IP (127.0.0.1 / 0.0.0.0), indicating active DNS spoofing.";
        }
        else if (localEmptyWhileDohHasIps)
        {
            result.IsDivergent = true;
            result.Explanation = "Local DNS failed to resolve hostname, while encrypted DoH successfully resolved valid addresses.";
        }
        else
        {
            result.IsDivergent = false;
            result.Explanation = "DNS resolution appears consistent between local resolver and encrypted public resolvers.";
        }

        return result;
    }

    private async Task<List<string>> QueryDohJsonAsync(string dohEndpoint, string hostname, CancellationToken ct)
    {
        try
        {
            using var req = new HttpRequestMessage(HttpMethod.Get, $"{dohEndpoint}?name={Uri.EscapeDataString(hostname)}&type=A");
            req.Headers.Add("Accept", "application/dns-json");

            using var resp = await _httpClient.SendAsync(req, ct);
            if (!resp.IsSuccessStatusCode)
                return new List<string>();

            string json = await resp.Content.ReadAsStringAsync(ct);
            using var doc = JsonDocument.Parse(json);

            var ips = new List<string>();
            if (doc.RootElement.TryGetProperty("Answer", out var answerElement) && answerElement.ValueKind == JsonValueKind.Array)
            {
                foreach (var ans in answerElement.EnumerateArray())
                {
                    if (ans.TryGetProperty("type", out var typeProp) && typeProp.GetInt32() == 1) // Type 1 = A record
                    {
                        if (ans.TryGetProperty("data", out var dataProp))
                        {
                            string? ip = dataProp.GetString();
                            if (!string.IsNullOrEmpty(ip))
                            {
                                ips.Add(ip);
                            }
                        }
                    }
                }
            }
            return ips;
        }
        catch
        {
            return new List<string>();
        }
    }
}
