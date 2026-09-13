using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using NetWard.Core.Diagnostics;
using NetWard.Core.LocalResilience;
using NetWard.Core.Models;
using NetWard.Core.Security;
using NetWard.Core.Services;
using NetWard.Core.Storage;

namespace NetWard.App.ViewModels;

public class ServiceCardViewModel : INotifyPropertyChanged
{
    private ServiceHealthVerdict? _verdict;
    private bool _isExpanded;

    public ServiceProfile Profile { get; }

    public ServiceHealthVerdict? Verdict
    {
        get => _verdict;
        set
        {
            _verdict = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(Status));
            OnPropertyChanged(nameof(StatusLabel));
            OnPropertyChanged(nameof(LatencyLabel));
            OnPropertyChanged(nameof(Summary));
            OnPropertyChanged(nameof(TechnicalDetails));
            OnPropertyChanged(nameof(Recommendation));
            OnPropertyChanged(nameof(ConfidenceLabel));
        }
    }

    public bool IsExpanded
    {
        get => _isExpanded;
        set { _isExpanded = value; OnPropertyChanged(); }
    }

    public ServiceStatus Status => Verdict?.Status ?? ServiceStatus.Unknown;
    public string StatusLabel => Verdict?.Status switch
    {
        ServiceStatus.Healthy => "РАБОТАЕТ",
        ServiceStatus.Degraded => "ЗАМЕДЛЕНО",
        ServiceStatus.Blocked => "БЛОКИРОВКА",
        ServiceStatus.Throttled => "ЗАМЕДЛЕНИЕ",
        ServiceStatus.ServerOutage => "СБОЙ СЕРВЕРА",
        ServiceStatus.LocalIssue => "СБОЙ СЕТИ",
        _ => "НЕ ПРОВЕРЕНО"
    };

    public string LatencyLabel => Verdict != null && Verdict.OverallLatencyMs > 0 ? $"{Verdict.OverallLatencyMs:F0} ms" : "-- ms";
    public string Summary => Verdict?.SummaryRu ?? "Нажмите 'Проверить всё' для запуска многоуровневой диагностики.";
    public string TechnicalDetails => Verdict?.TechnicalDetailsRu ?? "Данные пока отсутствуют.";
    public string Recommendation => Verdict?.RecommendationRu ?? "Рекомендации появятся после проверки.";
    public string ConfidenceLabel => Verdict != null ? $"{Verdict.ConfidencePercent}%" : "0%";

    public ServiceCardViewModel(ServiceProfile profile)
    {
        Profile = profile;
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}

public class MainViewModel : INotifyPropertyChanged
{
    private readonly IServiceRegistry _registry;
    private readonly IServiceDiagnosticsEngine _engine;
    private readonly IGameRadar _gameRadar;
    private readonly INetworkTuner _networkTuner;
    private readonly IDnsInspector _dnsInspector;
    private readonly IDiagnosticRedactor _redactor;
    private readonly IStorageManager _storage;

    private bool _isBusy;
    private int _progress;
    private string _statusText = "Готов к диагностике";
    private string _reportMarkdown = string.Empty;
    private LocalNetworkStatus? _localStatus;
    private DnsComparisonResult? _dnsResult;
    private SimulationMode _selectedSimulation = SimulationMode.RealNetwork;
    private string _currentLanguage = "ru";

    public string CurrentLanguage
    {
        get => _currentLanguage;
        set { _currentLanguage = value; OnPropertyChanged(); }
    }

    public ObservableCollection<ServiceCardViewModel> Services { get; } = new();
    public ObservableCollection<GamePingResult> GameResults { get; } = new();

    public bool IsBusy
    {
        get => _isBusy;
        set { _isBusy = value; OnPropertyChanged(); }
    }

    public int Progress
    {
        get => _progress;
        set { _progress = value; OnPropertyChanged(); }
    }

    public string StatusText
    {
        get => _statusText;
        set { _statusText = value; OnPropertyChanged(); }
    }

    public string ReportMarkdown
    {
        get => _reportMarkdown;
        set { _reportMarkdown = value; OnPropertyChanged(); }
    }

    public LocalNetworkStatus? LocalStatus
    {
        get => _localStatus;
        set { _localStatus = value; OnPropertyChanged(); }
    }

    public DnsComparisonResult? DnsResult
    {
        get => _dnsResult;
        set { _dnsResult = value; OnPropertyChanged(); }
    }

    public SimulationMode SelectedSimulation
    {
        get => _selectedSimulation;
        set
        {
            _selectedSimulation = value;
            OnPropertyChanged();
            ApplySimulationMode();
        }
    }

    public ICommand RunDiagnosisCommand { get; }
    public ICommand RunGameRadarCommand { get; }
    public ICommand FlushDnsCommand { get; }
    public ICommand CopyReportCommand { get; }

    public MainViewModel()
    {
        _registry = new DefaultServiceRegistry();
        _engine = new ServiceDiagnosticsEngine();
        _gameRadar = new GameRadar();
        _networkTuner = new NetworkTuner();
        _dnsInspector = new DnsInspector();
        _redactor = new DiagnosticRedactor();
        _storage = new StorageManager();

        foreach (var profile in _registry.GetAllProfiles())
        {
            Services.Add(new ServiceCardViewModel(profile));
        }

        RunDiagnosisCommand = new RelayCommand(async _ => await RunFullDiagnosisAsync());
        RunGameRadarCommand = new RelayCommand(async _ => await RunGameRadarAsync());
        FlushDnsCommand = new RelayCommand(async _ => await FlushDnsAsync());
        CopyReportCommand = new RelayCommand(_ => CopyReportToClipboard());

        _ = InitializeStartupAsync();
    }

    private async Task InitializeStartupAsync()
    {
        LocalStatus = await _networkTuner.GetLocalStatusAsync();
        await RunFullDiagnosisAsync();
        await RunGameRadarAsync();
    }

    public async Task RunFullDiagnosisAsync()
    {
        if (IsBusy) return;
        IsBusy = true;
        Progress = 0;
        StatusText = "Проверка доступности сервисов...";

        try
        {
            var profiles = _registry.GetAllProfiles();
            var progressReporter = new Progress<int>(p => Progress = p);

            var verdicts = await _engine.DiagnoseAllAsync(profiles, progressReporter);

            foreach (var verdict in verdicts)
            {
                var card = Services.FirstOrDefault(s => s.Profile.Id == verdict.ServiceId);
                if (card != null)
                {
                    card.Verdict = verdict;
                }
            }

            _storage.SaveHistory(verdicts);
            StatusText = $"Диагностика завершена: {verdicts.Count} сервисов проверено.";
            GenerateReport();
        }
        catch (Exception ex)
        {
            StatusText = $"Ошибка: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    public async Task RunGameRadarAsync()
    {
        StatusText = "Измерение задержки и потерь в играх...";
        try
        {
            var targets = _registry.GetDefaultGameTargets();
            var results = await _gameRadar.PingAllTargetsAsync(targets);

            GameResults.Clear();
            foreach (var r in results)
            {
                GameResults.Add(r);
            }
            StatusText = "Игровой радар обновлен.";
            GenerateReport();
        }
        catch (Exception ex)
        {
            StatusText = $"Ошибка радара: {ex.Message}";
        }
    }

    public async Task FlushDnsAsync()
    {
        StatusText = "Очистка локального кэша DNS...";
        bool success = await _networkTuner.FlushDnsCacheAsync();
        StatusText = success
            ? "✅ Кэш DNS успешно очищен!"
            : "⚠️ Не удалось очистить кэш DNS автоматически.";
    }

    private void GenerateReport()
    {
        var report = new DiagnosticReport
        {
            AnonymizedHost = Environment.MachineName,
            Verdicts = Services.Where(s => s.Verdict != null).Select(s => s.Verdict!).ToList(),
            GamePings = GameResults.ToList()
        };
        ReportMarkdown = _redactor.ExportReportToMarkdown(report);
    }

    private void CopyReportToClipboard()
    {
        if (!string.IsNullOrEmpty(ReportMarkdown))
        {
            Clipboard.SetText(ReportMarkdown);
            StatusText = "✅ Отчёт скопирован в буфер обмена!";
        }
    }

    private void ApplySimulationMode()
    {
        if (SelectedSimulation == SimulationMode.RealNetwork)
        {
            _engine.SocketProber = new SocketProber();
        }
        else
        {
            _engine.SocketProber = new SimulatedSocketProber { Mode = SelectedSimulation };
        }
        _ = RunFullDiagnosisAsync();
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}

public class RelayCommand : ICommand
{
    private readonly Action<object?> _execute;
    private readonly Predicate<object?>? _canExecute;

    public RelayCommand(Action<object?> execute, Predicate<object?>? canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    public bool CanExecute(object? parameter) => _canExecute?.Invoke(parameter) ?? true;
    public void Execute(object? parameter) => _execute(parameter);
    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
