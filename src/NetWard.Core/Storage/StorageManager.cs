using System.Text.Json;
using NetWard.Core.Models;

namespace NetWard.Core.Storage;

public class AppSettings
{
    public string Language { get; set; } = "ru";
    public int AutoRefreshMinutes { get; set; } = 5;
    public SimulationMode SimulationMode { get; set; } = SimulationMode.RealNetwork;
    public bool AutoCheckOnStartup { get; set; } = true;
    public bool MinimizeToTray { get; set; } = true;
}

public interface IStorageManager
{
    AppSettings LoadSettings();
    void SaveSettings(AppSettings settings);
    List<ServiceProfile> LoadCustomServices();
    void SaveCustomServices(List<ServiceProfile> profiles);
    void SaveHistory(List<ServiceHealthVerdict> verdicts);
    List<ServiceHealthVerdict> LoadHistory();
}

public class StorageManager : IStorageManager
{
    private readonly string _dataDir;
    private readonly string _settingsPath;
    private readonly string _customServicesPath;
    private readonly string _historyPath;

    public StorageManager(string? baseDir = null)
    {
        _dataDir = baseDir ?? Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "NetWard");

        Directory.CreateDirectory(_dataDir);
        _settingsPath = Path.Combine(_dataDir, "settings.json");
        _customServicesPath = Path.Combine(_dataDir, "custom_services.json");
        _historyPath = Path.Combine(_dataDir, "history.json");
    }

    public AppSettings LoadSettings()
    {
        try
        {
            if (File.Exists(_settingsPath))
            {
                string json = File.ReadAllText(_settingsPath);
                return JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
            }
        }
        catch { }
        return new AppSettings();
    }

    public void SaveSettings(AppSettings settings)
    {
        AtomicWriteJson(_settingsPath, settings);
    }

    public List<ServiceProfile> LoadCustomServices()
    {
        try
        {
            if (File.Exists(_customServicesPath))
            {
                string json = File.ReadAllText(_customServicesPath);
                return JsonSerializer.Deserialize<List<ServiceProfile>>(json) ?? new List<ServiceProfile>();
            }
        }
        catch { }
        return new List<ServiceProfile>();
    }

    public void SaveCustomServices(List<ServiceProfile> profiles)
    {
        AtomicWriteJson(_customServicesPath, profiles);
    }

    public void SaveHistory(List<ServiceHealthVerdict> verdicts)
    {
        try
        {
            // Keep at most 50 recent verdict records
            var existing = LoadHistory();
            existing.InsertRange(0, verdicts);
            if (existing.Count > 50)
            {
                existing = existing.Take(50).ToList();
            }
            AtomicWriteJson(_historyPath, existing);
        }
        catch { }
    }

    public List<ServiceHealthVerdict> LoadHistory()
    {
        try
        {
            if (File.Exists(_historyPath))
            {
                string json = File.ReadAllText(_historyPath);
                return JsonSerializer.Deserialize<List<ServiceHealthVerdict>>(json) ?? new List<ServiceHealthVerdict>();
            }
        }
        catch { }
        return new List<ServiceHealthVerdict>();
    }

    private void AtomicWriteJson<T>(string filePath, T data)
    {
        string tmpPath = filePath + ".tmp";
        string json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(tmpPath, json);
        File.Move(tmpPath, filePath, overwrite: true);
    }
}
