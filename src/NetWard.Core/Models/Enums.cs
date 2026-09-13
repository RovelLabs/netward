namespace NetWard.Core.Models;

public enum ServiceStatus
{
    Unknown,
    Healthy,
    Degraded,
    Blocked,
    Throttled,
    LocalIssue,
    ServerOutage
}

public enum FailureCause
{
    None,
    TspuSniBlock,
    DnsPoisoned,
    TcpUnreachable,
    ThrottledBandwidth,
    IspBlockPage,
    GlobalServerOutage,
    GeoRestricted,
    LocalGatewayFailure,
    TlsInterception,
    Unknown
}

public enum ServiceCategory
{
    Gaming,
    Voice,
    Video,
    Social,
    Development,
    ArtificialIntelligence,
    Messaging,
    Custom
}

public enum ProbeLayer
{
    Dns,
    TcpHandshake,
    TlsHandshake,
    HttpLayer,
    ContentStream
}

public enum SimulationMode
{
    RealNetwork,
    SimulateHealthy,
    SimulateDiscordBlocked,
    SimulateYouTubeThrottled,
    SimulateDnsPoisoning,
    SimulateServerOutage,
    SimulateOffline
}
