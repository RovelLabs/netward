using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using NetWard.Core.Models;

namespace NetWard.App.Converters;

public class StatusToBrushConverter : IValueConverter
{
    private static readonly SolidColorBrush HealthyBrush = new(Color.FromRgb(0x00, 0xE5, 0x99));
    private static readonly SolidColorBrush DegradedBrush = new(Color.FromRgb(0xFF, 0xB3, 0x00));
    private static readonly SolidColorBrush BlockedBrush = new(Color.FromRgb(0xFF, 0x33, 0x4B));
    private static readonly SolidColorBrush ThrottledBrush = new(Color.FromRgb(0xC0, 0x54, 0xFF));
    private static readonly SolidColorBrush OutageBrush = new(Color.FromRgb(0xE5, 0xE7, 0xEB));
    private static readonly SolidColorBrush LocalIssueBrush = new(Color.FromRgb(0xFF, 0x7A, 0x00));
    private static readonly SolidColorBrush UnknownBrush = new(Color.FromRgb(0x6B, 0x72, 0x80));

    static StatusToBrushConverter()
    {
        HealthyBrush.Freeze();
        DegradedBrush.Freeze();
        BlockedBrush.Freeze();
        ThrottledBrush.Freeze();
        OutageBrush.Freeze();
        LocalIssueBrush.Freeze();
        UnknownBrush.Freeze();
    }

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is ServiceStatus status)
        {
            return status switch
            {
                ServiceStatus.Healthy => HealthyBrush,
                ServiceStatus.Degraded => DegradedBrush,
                ServiceStatus.Blocked => BlockedBrush,
                ServiceStatus.Throttled => ThrottledBrush,
                ServiceStatus.ServerOutage => OutageBrush,
                ServiceStatus.LocalIssue => LocalIssueBrush,
                _ => UnknownBrush
            };
        }
        return UnknownBrush;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotImplementedException();
}

public class StatusToLabelConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is ServiceStatus status)
        {
            return status switch
            {
                ServiceStatus.Healthy => "РАБОТАЕТ",
                ServiceStatus.Degraded => "ЗАМЕДЛЕНО",
                ServiceStatus.Blocked => "БЛОКИРОВКА",
                ServiceStatus.Throttled => "ЗАМЕДЛЕНИЕ",
                ServiceStatus.ServerOutage => "СБОЙ СЕРВЕРА",
                ServiceStatus.LocalIssue => "СБОЙ СЕТИ",
                _ => "НЕ ПРОВЕРЕНО"
            };
        }
        return "НЕ ПРОВЕРЕНО";
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotImplementedException();
}

public class BooleanToVisibilityConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        bool b = value is true;
        return b ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotImplementedException();
}
