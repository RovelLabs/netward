namespace NetWard.Core.Localization;

public interface ILocalizationService
{
    string CurrentLanguage { get; set; }
    string Get(string key);
    event Action? LanguageChanged;
}

public class LocalizationService : ILocalizationService
{
    private string _currentLanguage = "ru";

    public string CurrentLanguage
    {
        get => _currentLanguage;
        set
        {
            if (_currentLanguage != value)
            {
                _currentLanguage = value;
                LanguageChanged?.Invoke();
            }
        }
    }

    public event Action? LanguageChanged;

    private static readonly Dictionary<string, string> RuStrings = new()
    {
        ["AppTitle"] = "NetWard — Монитор устойчивости сети",
        ["AppSubtitle"] = "Честная диагностика доступности сервисов и игровых серверов",
        ["TabServices"] = "Сервисы",
        ["TabGaming"] = "Игровой радар",
        ["TabLocalNetwork"] = "Сеть и DNS",
        ["TabExport"] = "Экспорт отчёта",
        ["BtnRunFullDiagnosis"] = "Проверить всё",
        ["BtnProbing"] = "Диагностика...",
        ["BtnFlushDns"] = "Очистить кэш DNS",
        ["BtnExportReport"] = "Скопировать отчёт",
        ["StatusHealthy"] = "Работает",
        ["StatusDegraded"] = "Замедлено",
        ["StatusBlocked"] = "Блокировка",
        ["StatusThrottled"] = "Замедление",
        ["StatusServerOutage"] = "Сбой сервера",
        ["StatusLocalIssue"] = "Сбой сети",
        ["StatusUnknown"] = "Не проверено",
        ["GamingPing"] = "Пинг",
        ["GamingJitter"] = "Джиттер",
        ["GamingLoss"] = "Потери",
        ["TechDetails"] = "Технические детали",
        ["Recommendations"] = "Рекомендации",
        ["Confidence"] = "Уверенность",
        ["Latency"] = "Задержка",
        ["SimulationMode"] = "Режим симуляции",
        ["SimulationReal"] = "Реальная сеть",
        ["SimulationDiscordBlocked"] = "Симуляция: Блокировка Discord (ТСПУ)",
        ["SimulationYouTubeThrottled"] = "Симуляция: Замедление YouTube (ТСПУ)",
        ["SimulationServerOutage"] = "Симуляция: Сбой сервера",
        ["SimulationDnsPoisoned"] = "Симуляция: DNS-подмена",
        ["GatewayHealth"] = "Шлюз роутера",
        ["LocalDns"] = "DNS провайдера",
        ["EncryptedDns"] = "Защищённый DoH DNS",
        ["CopySuccess"] = "Отчёт скопирован в буфер обмена!",
        ["FlushDnsSuccess"] = "Кэш DNS успешно очищен!"
    };

    private static readonly Dictionary<string, string> EnStrings = new()
    {
        ["AppTitle"] = "NetWard — Internet Resilience Companion",
        ["AppSubtitle"] = "Transparent reachability diagnostics for services and game servers",
        ["TabServices"] = "Services",
        ["TabGaming"] = "Gaming Radar",
        ["TabLocalNetwork"] = "Network & DNS",
        ["TabExport"] = "Export Report",
        ["BtnRunFullDiagnosis"] = "Run Full Check",
        ["BtnProbing"] = "Probing...",
        ["BtnFlushDns"] = "Flush DNS Cache",
        ["BtnExportReport"] = "Copy Report",
        ["StatusHealthy"] = "Healthy",
        ["StatusDegraded"] = "Degraded",
        ["StatusBlocked"] = "Blocked",
        ["StatusThrottled"] = "Throttled",
        ["StatusServerOutage"] = "Server Outage",
        ["StatusLocalIssue"] = "Local Issue",
        ["StatusUnknown"] = "Unknown",
        ["GamingPing"] = "Ping",
        ["GamingJitter"] = "Jitter",
        ["GamingLoss"] = "Loss",
        ["TechDetails"] = "Technical Details",
        ["Recommendations"] = "Recommendations",
        ["Confidence"] = "Confidence",
        ["Latency"] = "Latency",
        ["SimulationMode"] = "Simulation Mode",
        ["SimulationReal"] = "Live Network",
        ["SimulationDiscordBlocked"] = "Simulate: Discord Blocked (TSPU)",
        ["SimulationYouTubeThrottled"] = "Simulate: YouTube Throttled (TSPU)",
        ["SimulationServerOutage"] = "Simulate: Cloud Outage",
        ["SimulationDnsPoisoned"] = "Simulate: DNS Poisoned",
        ["GatewayHealth"] = "Router Gateway",
        ["LocalDns"] = "ISP DNS",
        ["EncryptedDns"] = "Encrypted DoH DNS",
        ["CopySuccess"] = "Report copied to clipboard!",
        ["FlushDnsSuccess"] = "DNS cache flushed successfully!"
    };

    public string Get(string key)
    {
        var dict = _currentLanguage.Equals("en", StringComparison.OrdinalIgnoreCase) ? EnStrings : RuStrings;
        if (dict.TryGetValue(key, out string? value))
        {
            return value;
        }
        return key;
    }
}
