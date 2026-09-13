using NetWard.Core.Diagnostics;
using NetWard.Core.Models;
using NetWard.Core.Services;
using Xunit;

namespace NetWard.Tests;

public class SimulationEngineTests
{
    private readonly DefaultServiceRegistry _registry = new();

    [Fact]
    public async Task DiagnoseService_WhenDiscordSimulatedBlocked_ReturnsBlockedVerdict()
    {
        var sim = new SimulatedSocketProber { Mode = SimulationMode.SimulateDiscordBlocked };
        var engine = new ServiceDiagnosticsEngine(sim);

        var discordProfile = _registry.GetProfileById("discord")!;
        var verdict = await engine.DiagnoseServiceAsync(discordProfile);

        Assert.Equal(ServiceStatus.Blocked, verdict.Status);
        Assert.Equal(FailureCause.TspuSniBlock, verdict.FailureCause);
        Assert.True(verdict.ConfidencePercent >= 90);
    }

    [Fact]
    public async Task DiagnoseService_WhenYouTubeSimulatedThrottled_ReturnsThrottledVerdict()
    {
        var sim = new SimulatedSocketProber { Mode = SimulationMode.SimulateYouTubeThrottled };
        var engine = new ServiceDiagnosticsEngine(sim);

        var ytProfile = _registry.GetProfileById("youtube")!;
        var verdict = await engine.DiagnoseServiceAsync(ytProfile);

        Assert.Equal(ServiceStatus.Throttled, verdict.Status);
        Assert.Equal(FailureCause.ThrottledBandwidth, verdict.FailureCause);
    }

    [Fact]
    public async Task DiagnoseService_WhenServerOutageSimulated_ReturnsOutageVerdict()
    {
        var sim = new SimulatedSocketProber { Mode = SimulationMode.SimulateServerOutage };
        var engine = new ServiceDiagnosticsEngine(sim);

        var steamProfile = _registry.GetProfileById("steam")!;
        var verdict = await engine.DiagnoseServiceAsync(steamProfile);

        Assert.Equal(ServiceStatus.ServerOutage, verdict.Status);
        Assert.Equal(FailureCause.GlobalServerOutage, verdict.FailureCause);
    }

    [Fact]
    public async Task DiagnoseService_WhenDnsPoisoningSimulated_ReturnsBlockedVerdict()
    {
        var sim = new SimulatedSocketProber { Mode = SimulationMode.SimulateDnsPoisoning };
        var engine = new ServiceDiagnosticsEngine(sim);

        var steamProfile = _registry.GetProfileById("steam")!;
        var verdict = await engine.DiagnoseServiceAsync(steamProfile);

        Assert.Equal(ServiceStatus.Blocked, verdict.Status);
        Assert.Equal(FailureCause.DnsPoisoned, verdict.FailureCause);
    }

    [Fact]
    public async Task DiagnoseService_WhenHealthySimulated_ReturnsHealthyVerdict()
    {
        var sim = new SimulatedSocketProber { Mode = SimulationMode.SimulateHealthy };
        var engine = new ServiceDiagnosticsEngine(sim);

        var githubProfile = _registry.GetProfileById("github")!;
        var verdict = await engine.DiagnoseServiceAsync(githubProfile);

        Assert.Equal(ServiceStatus.Healthy, verdict.Status);
        Assert.Equal(FailureCause.None, verdict.FailureCause);
    }

    [Fact]
    public async Task DiagnoseAll_ExecutesAllProfilesAndReportsProgress()
    {
        var sim = new SimulatedSocketProber { Mode = SimulationMode.SimulateHealthy };
        var engine = new ServiceDiagnosticsEngine(sim);

        int lastProgress = 0;
        var progress = new Progress<int>(p => lastProgress = p);

        var profiles = _registry.GetAllProfiles();
        var verdicts = await engine.DiagnoseAllAsync(profiles, progress);

        Assert.Equal(profiles.Count, verdicts.Count);
        Assert.Equal(100, lastProgress);
    }
}
