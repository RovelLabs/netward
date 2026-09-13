using NetWard.Core.Models;

namespace NetWard.Core.Services;

public interface IServiceRegistry
{
    IReadOnlyList<ServiceProfile> GetAllProfiles();
    ServiceProfile? GetProfileById(string id);
    void RegisterCustomProfile(ServiceProfile profile);
    bool RemoveCustomProfile(string id);
    IReadOnlyList<GameServerTarget> GetDefaultGameTargets();
}

public class DefaultServiceRegistry : IServiceRegistry
{
    private readonly List<ServiceProfile> _profiles = new();
    private readonly List<GameServerTarget> _gameTargets = new();

    public DefaultServiceRegistry()
    {
        InitializeDefaultProfiles();
        InitializeDefaultGameTargets();
    }

    public IReadOnlyList<ServiceProfile> GetAllProfiles() => _profiles.AsReadOnly();

    public ServiceProfile? GetProfileById(string id) =>
        _profiles.FirstOrDefault(p => string.Equals(p.Id, id, StringComparison.OrdinalIgnoreCase));

    public void RegisterCustomProfile(ServiceProfile profile)
    {
        profile.IsBuiltIn = false;
        var existing = GetProfileById(profile.Id);
        if (existing != null)
        {
            _profiles.Remove(existing);
        }
        _profiles.Add(profile);
    }

    public bool RemoveCustomProfile(string id)
    {
        var existing = GetProfileById(id);
        if (existing != null && !existing.IsBuiltIn)
        {
            return _profiles.Remove(existing);
        }
        return false;
    }

    public IReadOnlyList<GameServerTarget> GetDefaultGameTargets() => _gameTargets.AsReadOnly();

    private void InitializeDefaultProfiles()
    {
        // 1. Discord
        _profiles.Add(new ServiceProfile
        {
            Id = "discord",
            DisplayName = "Discord",
            Category = ServiceCategory.Voice,
            IconGlyph = "🎧",
            Description = "Voice channels, text chat, and communities for gamers.",
            KnownRestrictions = "Officially blocked in Russia on Oct 8, 2024 via TSPU SNI filtering and voice RTC packet drops.",
            Endpoints = new List<ServiceEndpoint>
            {
                new() { Host = "discord.com", Port = 443, UseTls = true, HttpPath = "/", ExpectedHttpStatus = 200, Purpose = "Web Portal" },
                new() { Host = "gateway.discord.gg", Port = 443, UseTls = true, HttpPath = "/", ExpectedHttpStatus = 404, Purpose = "Realtime Gateway" }
            }
        });

        // 2. YouTube
        _profiles.Add(new ServiceProfile
        {
            Id = "youtube",
            DisplayName = "YouTube",
            Category = ServiceCategory.Video,
            IconGlyph = "▶️",
            Description = "Video streaming, tutorials, educational content, and gaming streams.",
            KnownRestrictions = "Subjected to artificial traffic policing and packet drops on GGC CDN nodes since August 2024.",
            Endpoints = new List<ServiceEndpoint>
            {
                new() { Host = "www.youtube.com", Port = 443, UseTls = true, HttpPath = "/", ExpectedHttpStatus = 200, Purpose = "Web Application" },
                new() { Host = "googlevideo.com", Port = 443, UseTls = true, HttpPath = "/", ExpectedHttpStatus = 404, CheckStreaming = true, Purpose = "Video Delivery CDN" }
            }
        });

        // 3. Steam
        _profiles.Add(new ServiceProfile
        {
            Id = "steam",
            DisplayName = "Steam",
            Category = ServiceCategory.Gaming,
            IconGlyph = "🎮",
            Description = "Valve game store, cloud saves, multiplayer coordination, and community hubs.",
            KnownRestrictions = "Store operational; community hub pages periodically face selective ISP blocking.",
            Endpoints = new List<ServiceEndpoint>
            {
                new() { Host = "store.steampowered.com", Port = 443, UseTls = true, HttpPath = "/", ExpectedHttpStatus = 200, Purpose = "Game Store & API" },
                new() { Host = "steamcommunity.com", Port = 443, UseTls = true, HttpPath = "/", ExpectedHttpStatus = 200, Purpose = "Community Hub & Guides" }
            }
        });

        // 4. Telegram
        _profiles.Add(new ServiceProfile
        {
            Id = "telegram",
            DisplayName = "Telegram",
            Category = ServiceCategory.Messaging,
            IconGlyph = "✈️",
            Description = "Fast cloud-based messaging, channels, student groups, and file sharing.",
            KnownRestrictions = "Operational nationwide; sporadic localized throttling tests in specific regions.",
            Endpoints = new List<ServiceEndpoint>
            {
                new() { Host = "web.telegram.org", Port = 443, UseTls = true, HttpPath = "/", ExpectedHttpStatus = 200, Purpose = "Web Client" },
                new() { Host = "api.telegram.org", Port = 443, UseTls = true, HttpPath = "/", ExpectedHttpStatus = 404, Purpose = "Bot & API Cluster" }
            }
        });

        // 5. Roblox
        _profiles.Add(new ServiceProfile
        {
            Id = "roblox",
            DisplayName = "Roblox",
            Category = ServiceCategory.Gaming,
            IconGlyph = "🧱",
            Description = "Multiplayer game creation platform widely played by teens.",
            KnownRestrictions = "Monitored under regulatory scrutiny; asset downloads can experience peering latency.",
            Endpoints = new List<ServiceEndpoint>
            {
                new() { Host = "www.roblox.com", Port = 443, UseTls = true, HttpPath = "/", ExpectedHttpStatus = 200, Purpose = "Platform Portal" },
                new() { Host = "setup.rbxcdn.com", Port = 443, UseTls = true, HttpPath = "/", ExpectedHttpStatus = 403, Purpose = "Asset CDN" }
            }
        });

        // 6. GitHub
        _profiles.Add(new ServiceProfile
        {
            Id = "github",
            DisplayName = "GitHub",
            Category = ServiceCategory.Development,
            IconGlyph = "🐙",
            Description = "Code hosting, developer collaboration, package releases, and open source.",
            KnownRestrictions = "Generally operational; occasional specific repository gists blocked.",
            Endpoints = new List<ServiceEndpoint>
            {
                new() { Host = "github.com", Port = 443, UseTls = true, HttpPath = "/", ExpectedHttpStatus = 200, Purpose = "Web Platform" },
                new() { Host = "api.github.com", Port = 443, UseTls = true, HttpPath = "/", ExpectedHttpStatus = 200, Purpose = "REST API" }
            }
        });

        // 7. OpenAI (ChatGPT)
        _profiles.Add(new ServiceProfile
        {
            Id = "openai",
            DisplayName = "OpenAI (ChatGPT)",
            Category = ServiceCategory.ArtificialIntelligence,
            IconGlyph = "🧠",
            Description = "AI assistant used by students for study, writing, and coding.",
            KnownRestrictions = "Provider-side geo-restriction (Cloudflare 403 / country check for Russian IP addresses).",
            Endpoints = new List<ServiceEndpoint>
            {
                new() { Host = "chatgpt.com", Port = 443, UseTls = true, HttpPath = "/", ExpectedHttpStatus = 200, Purpose = "Chat Interface" },
                new() { Host = "api.openai.com", Port = 443, UseTls = true, HttpPath = "/", ExpectedHttpStatus = 404, Purpose = "Inference API" }
            }
        });

        // 8. Twitch
        _profiles.Add(new ServiceProfile
        {
            Id = "twitch",
            DisplayName = "Twitch",
            Category = ServiceCategory.Video,
            IconGlyph = "🟣",
            Description = "Live gaming streams, esports broadcasts, and creator chats.",
            KnownRestrictions = "Operational, with intermittent peering bottlenecks causing stream buffering.",
            Endpoints = new List<ServiceEndpoint>
            {
                new() { Host = "www.twitch.tv", Port = 443, UseTls = true, HttpPath = "/", ExpectedHttpStatus = 200, Purpose = "Web Portal" }
            }
        });
    }

    private void InitializeDefaultGameTargets()
    {
        // Valve Counter-Strike 2 & Dota 2 Servers
        _gameTargets.Add(new GameServerTarget { GameName = "Counter-Strike 2", Region = "EU East (Warsaw)", Hostname = "155.133.230.1", Port = 27015 });
        _gameTargets.Add(new GameServerTarget { GameName = "Counter-Strike 2", Region = "EU North (Stockholm)", Hostname = "155.133.248.1", Port = 27015 });
        _gameTargets.Add(new GameServerTarget { GameName = "Counter-Strike 2", Region = "EU West (Frankfurt)", Hostname = "155.133.226.1", Port = 27015 });
        _gameTargets.Add(new GameServerTarget { GameName = "Dota 2", Region = "Stockholm Core", Hostname = "162.254.198.1", Port = 27015 });
        _gameTargets.Add(new GameServerTarget { GameName = "Dota 2", Region = "Vienna Core", Hostname = "185.25.182.1", Port = 27015 });

        // Roblox Primary Gateways
        _gameTargets.Add(new GameServerTarget { GameName = "Roblox", Region = "Europe Gateway", Hostname = "128.116.119.3", Port = 443 });

        // Minecraft Top Multiplayer Nodes
        _gameTargets.Add(new GameServerTarget { GameName = "Minecraft", Region = "Hypixel Network", Hostname = "mc.hypixel.net", Port = 25565 });
    }
}
