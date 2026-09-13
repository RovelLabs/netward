using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace NetWard.Core.LocalResilience;

public interface INetworkTuner
{
    Task<bool> FlushDnsCacheAsync();
    Task<double?> PingGatewayAsync(TimeSpan timeout);
    Task<LocalNetworkStatus> GetLocalStatusAsync();
}

public class LocalNetworkStatus
{
    public bool IsConnected { get; set; }
    public string? GatewayIp { get; set; }
    public double? GatewayPingMs { get; set; }
    public List<string> DnsServers { get; set; } = new();
    public string InterfaceName { get; set; } = string.Empty;
    public NetworkInterfaceType InterfaceType { get; set; }
    public long SpeedMbps { get; set; }
}

public class NetworkTuner : INetworkTuner
{
    [DllImport("dnsapi.dll", EntryPoint = "DnsFlushResolverCache")]
    private static extern int DnsFlushResolverCache();

    public async Task<bool> FlushDnsCacheAsync()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            try
            {
                int result = DnsFlushResolverCache();
                if (result == 0) return true;
            }
            catch { }

            // Fallback to ipconfig /flushdns
            try
            {
                using var proc = Process.Start(new ProcessStartInfo
                {
                    FileName = "ipconfig",
                    Arguments = "/flushdns",
                    CreateNoWindow = true,
                    UseShellExecute = false
                });
                if (proc != null)
                {
                    await proc.WaitForExitAsync();
                    return proc.ExitCode == 0;
                }
            }
            catch { }
        }
        return false;
    }

    public async Task<double?> PingGatewayAsync(TimeSpan timeout)
    {
        var gateway = GetDefaultGateway();
        if (gateway == null) return null;

        var sw = Stopwatch.StartNew();
        using var ping = new Ping();
        try
        {
            var reply = await ping.SendPingAsync(gateway, (int)timeout.TotalMilliseconds);
            sw.Stop();
            if (reply.Status == IPStatus.Success)
            {
                return reply.RoundtripTime;
            }
        }
        catch { }

        // Fallback TCP probe to gateway port 53 or 80
        try
        {
            using var socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            using var cts = new CancellationTokenSource(timeout);
            sw.Restart();
            await socket.ConnectAsync(gateway, 53, cts.Token);
            sw.Stop();
            return sw.Elapsed.TotalMilliseconds;
        }
        catch
        {
            return null;
        }
    }

    public async Task<LocalNetworkStatus> GetLocalStatusAsync()
    {
        var status = new LocalNetworkStatus();
        var nic = NetworkInterface.GetAllNetworkInterfaces()
            .FirstOrDefault(n => n.OperationalStatus == OperationalStatus.Up &&
                                 n.NetworkInterfaceType != NetworkInterfaceType.Loopback);

        if (nic != null)
        {
            status.IsConnected = true;
            status.InterfaceName = nic.Name;
            status.InterfaceType = nic.NetworkInterfaceType;
            status.SpeedMbps = nic.Speed > 0 ? nic.Speed / 1_000_000 : 0;

            var ipProps = nic.GetIPProperties();
            var gw = ipProps.GatewayAddresses.FirstOrDefault();
            if (gw != null && gw.Address != null)
            {
                status.GatewayIp = gw.Address.ToString();
            }

            status.DnsServers = ipProps.DnsAddresses
                .Select(d => d.ToString())
                .Distinct()
                .ToList();
        }

        status.GatewayPingMs = await PingGatewayAsync(TimeSpan.FromMilliseconds(500));
        return status;
    }

    private static IPAddress? GetDefaultGateway()
    {
        try
        {
            return NetworkInterface.GetAllNetworkInterfaces()
                .Where(n => n.OperationalStatus == OperationalStatus.Up && n.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                .SelectMany(n => n.GetIPProperties().GatewayAddresses)
                .Select(g => g?.Address)
                .FirstOrDefault(a => a != null && a.AddressFamily == AddressFamily.InterNetwork);
        }
        catch
        {
            return null;
        }
    }
}
