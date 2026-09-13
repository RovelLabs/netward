using System.Text.Json;
using NetWard.Core.Diagnostics;
using NetWard.Core.LocalResilience;
using NetWard.Core.Models;
using NetWard.Core.Security;
using NetWard.Core.Services;

namespace NetWard.Cli;

public class Program
{
    public static async Task<int> Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        var registry = new DefaultServiceRegistry();
        var prober = new SocketProber();
        var engine = new ServiceDiagnosticsEngine(prober);
        var gameRadar = new GameRadar();
        var dnsInspector = new DnsInspector();
        var tuner = new NetworkTuner();
        var redactor = new DiagnosticRedactor();

        string command = args.Length > 0 ? args[0].ToLowerInvariant() : "check";
        bool jsonOutput = args.Any(a => a.Equals("--json", StringComparison.OrdinalIgnoreCase));

        if (command == "help" || command == "--help" || command == "-h")
        {
            PrintHelp();
            return 0;
        }

        if (command == "flush-dns")
        {
            Console.WriteLine("🔄 Flushing local DNS resolver cache...");
            bool ok = await tuner.FlushDnsCacheAsync();
            if (ok)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("✅ DNS resolver cache flushed successfully.");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("⚠️ Unable to flush DNS automatically (administrative privileges may be needed).");
            }
            Console.ResetColor();
            return ok ? 0 : 1;
        }

        if (command == "game")
        {
            Console.WriteLine("🎮 Running NetWard Gaming Latency Radar...\n");
            var targets = registry.GetDefaultGameTargets();
            var results = await gameRadar.PingAllTargetsAsync(targets);

            if (jsonOutput)
            {
                Console.WriteLine(JsonSerializer.Serialize(results, new JsonSerializerOptions { WriteIndented = true }));
                return 0;
            }

            Console.WriteLine("{0,-20} {1,-22} {2,8} {3,8} {4,8} {5,8} {6,8}", "GAME", "REGION", "MIN", "AVG", "MAX", "JITTER", "LOSS");
            Console.WriteLine(new string('-', 85));

            foreach (var r in results)
            {
                Console.ForegroundColor = r.IsHealthy ? ConsoleColor.Green : ConsoleColor.Yellow;
                Console.WriteLine("{0,-20} {1,-22} {2,6:F0}ms {3,6:F0}ms {4,6:F0}ms ±{5,5:F1}ms {6,6:F0}%",
                    r.GameName, r.Region, r.MinLatencyMs, r.AvgLatencyMs, r.MaxLatencyMs, r.JitterMs, r.PacketLossPct);
            }
            Console.ResetColor();
            return 0;
        }

        if (command == "simulate")
        {
            string simModeStr = args.Length > 1 ? args[1] : "discord";
            var simMode = simModeStr.ToLowerInvariant() switch
            {
                "youtube" => SimulationMode.SimulateYouTubeThrottled,
                "outage" => SimulationMode.SimulateServerOutage,
                "dns" => SimulationMode.SimulateDnsPoisoning,
                _ => SimulationMode.SimulateDiscordBlocked
            };

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"[SIMULATION MODE ACTIVE: {simMode}]");
            Console.ResetColor();

            var simProber = new SimulatedSocketProber { Mode = simMode };
            engine.SocketProber = simProber;
            command = "check";
        }

        if (command == "check")
        {
            string? targetId = args.Length > 1 && !args[1].StartsWith("--") ? args[1] : null;

            PrintHeader();

            var profilesToTest = targetId != null
                ? registry.GetAllProfiles().Where(p => p.Id.Equals(targetId, StringComparison.OrdinalIgnoreCase)).ToList()
                : registry.GetAllProfiles().ToList();

            if (profilesToTest.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error: Service profile '{targetId}' not found in registry.");
                Console.ResetColor();
                return 1;
            }

            Console.WriteLine($"🔍 Probing {profilesToTest.Count} target services across OSI Layers 3–7...\n");

            var progress = new Progress<int>(p =>
            {
                // In CLI mode, keep output clean
            });

            var verdicts = await engine.DiagnoseAllAsync(profilesToTest, progress);

            if (jsonOutput)
            {
                Console.WriteLine(JsonSerializer.Serialize(verdicts, new JsonSerializerOptions { WriteIndented = true }));
                return 0;
            }

            foreach (var v in verdicts)
            {
                PrintVerdictCard(v);
            }

            return 0;
        }

        if (command == "export")
        {
            var profiles = registry.GetAllProfiles();
            var verdicts = await engine.DiagnoseAllAsync(profiles);
            var targets = registry.GetDefaultGameTargets();
            var gamePings = await gameRadar.PingAllTargetsAsync(targets);

            var report = new DiagnosticReport
            {
                AnonymizedHost = Environment.MachineName,
                Verdicts = verdicts,
                GamePings = gamePings
            };

            string md = redactor.ExportReportToMarkdown(report);
            string outFile = args.Length > 1 && !args[1].StartsWith("--") ? args[1] : "netward-report.md";
            await File.WriteAllTextAsync(outFile, md);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"✅ Redacted diagnostic report saved to: {Path.GetFullPath(outFile)}");
            Console.ResetColor();
            return 0;
        }

        PrintHelp();
        return 1;
    }

    private static void PrintHeader()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine(@"
 _   _      _   _    _              _ 
| \ | | ___| |_| |  | | __ _ _ __ _| |
|  \| |/ _ \ __| |/\| |/ _` | '__/ _` |
| |\  |  __/ |_ \  /\  / (_| | | | (_| |
|_| \_|\___|\__| \/  \/ \__,_|_|  \__,_|
NetWard v1.0.0 — Open-Source Internet Resilience & Diagnostic Companion
");
        Console.ResetColor();
    }

    private static void PrintVerdictCard(ServiceHealthVerdict v)
    {
        Console.ForegroundColor = v.Status switch
        {
            ServiceStatus.Healthy => ConsoleColor.Green,
            ServiceStatus.Degraded => ConsoleColor.Yellow,
            ServiceStatus.Blocked => ConsoleColor.Red,
            ServiceStatus.Throttled => ConsoleColor.Magenta,
            ServiceStatus.ServerOutage => ConsoleColor.White,
            _ => ConsoleColor.Gray
        };

        string statusLabel = v.Status switch
        {
            ServiceStatus.Healthy => "● HEALTHY",
            ServiceStatus.Degraded => "▲ DEGRADED",
            ServiceStatus.Blocked => "✖ BLOCKED",
            ServiceStatus.Throttled => "▼ THROTTLED",
            ServiceStatus.ServerOutage => "■ SERVER OUTAGE",
            _ => "? UNKNOWN"
        };

        Console.WriteLine($"┌─ {v.DisplayName} [{v.Category}] ───────────────────────────────────────");
        Console.WriteLine($"│ Status:     {statusLabel} ({v.OverallLatencyMs:F0} ms) [Confidence: {v.ConfidencePercent}%]");
        Console.WriteLine($"│ Cause:      {v.FailureCause}");
        Console.WriteLine($"│ RU:         {v.SummaryRu}");
        Console.WriteLine($"│ Tech:       {v.TechnicalDetailsRu}");
        Console.WriteLine($"│ Advice:     {v.RecommendationRu}");
        Console.WriteLine("└─────────────────────────────────────────────────────────────\n");
        Console.ResetColor();
    }

    private static void PrintHelp()
    {
        PrintHeader();
        Console.WriteLine("Usage: netward [command] [options]\n");
        Console.WriteLine("Commands:");
        Console.WriteLine("  check [service]     Run multi-layer diagnostics (default: all services)");
        Console.WriteLine("  game                Run tactical gaming latency, jitter, and packet loss radar");
        Console.WriteLine("  flush-dns           Flush local operating system DNS resolver cache");
        Console.WriteLine("  export [filename]   Generate anonymized Markdown telemetry report");
        Console.WriteLine("  simulate <mode>     Run offline simulation (discord|youtube|outage|dns)");
        Console.WriteLine("  help                Show this help message\n");
        Console.WriteLine("Options:");
        Console.WriteLine("  --json              Output raw machine-readable JSON");
    }
}
