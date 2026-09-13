using NetWard.Core.Models;
using NetWard.Core.Services;
using NetWard.Core.Storage;
using Xunit;

namespace NetWard.Tests;

public class StorageAndRegistryTests : IDisposable
{
    private readonly string _testDir;
    private readonly StorageManager _storage;
    private readonly DefaultServiceRegistry _registry = new();

    public StorageAndRegistryTests()
    {
        _testDir = Path.Combine(Path.GetTempPath(), "NetWard_Test_" + Guid.NewGuid().ToString("N"));
        _storage = new StorageManager(_testDir);
    }

    public void Dispose()
    {
        try
        {
            if (Directory.Exists(_testDir))
            {
                Directory.Delete(_testDir, true);
            }
        }
        catch { }
    }

    [Fact]
    public void Registry_ContainsEssentialTeenagerServices()
    {
        var profiles = _registry.GetAllProfiles();

        Assert.Contains(profiles, p => p.Id == "discord");
        Assert.Contains(profiles, p => p.Id == "youtube");
        Assert.Contains(profiles, p => p.Id == "steam");
        Assert.Contains(profiles, p => p.Id == "telegram");
        Assert.Contains(profiles, p => p.Id == "roblox");
        Assert.Contains(profiles, p => p.Id == "github");
        Assert.Contains(profiles, p => p.Id == "openai");
    }

    [Fact]
    public void Registry_AllowsAddingAndRemovingCustomProfile()
    {
        var custom = new ServiceProfile
        {
            Id = "mycustomserver",
            DisplayName = "My Minecraft Server",
            Category = ServiceCategory.Gaming,
            Endpoints = new() { new() { Host = "play.mycustom.ru", Port = 25565 } }
        };

        _registry.RegisterCustomProfile(custom);
        Assert.NotNull(_registry.GetProfileById("mycustomserver"));

        bool removed = _registry.RemoveCustomProfile("mycustomserver");
        Assert.True(removed);
        Assert.Null(_registry.GetProfileById("mycustomserver"));
    }

    [Fact]
    public void Storage_SavesAndLoadsSettingsSafely()
    {
        var settings = new AppSettings
        {
            Language = "en",
            AutoRefreshMinutes = 10,
            SimulationMode = SimulationMode.SimulateDiscordBlocked
        };

        _storage.SaveSettings(settings);
        var loaded = _storage.LoadSettings();

        Assert.Equal("en", loaded.Language);
        Assert.Equal(10, loaded.AutoRefreshMinutes);
        Assert.Equal(SimulationMode.SimulateDiscordBlocked, loaded.SimulationMode);
    }

    [Fact]
    public void Storage_RecoversGracefullyFromCorruptedFile()
    {
        string settingsFile = Path.Combine(_testDir, "settings.json");
        File.WriteAllText(settingsFile, "{ corrupted json syntax !!");

        var loaded = _storage.LoadSettings();
        Assert.NotNull(loaded);
        Assert.Equal("ru", loaded.Language); // default fallback
    }

    [Fact]
    public void Storage_HistoryRollingBufferMaintainsCap()
    {
        var list = new List<ServiceHealthVerdict>();
        for (int i = 0; i < 60; i++)
        {
            list.Add(new ServiceHealthVerdict { DisplayName = $"Test_{i}" });
        }

        _storage.SaveHistory(list);
        var loaded = _storage.LoadHistory();

        Assert.True(loaded.Count <= 50);
    }
}
