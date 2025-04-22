using Microsoft.UI.Xaml.Data;
using System.Globalization;

namespace TransportManager.Converters;

/// <summary>
/// Converte valores booleanos para visibilidade de elementos
/// </summary>
public class BooleanToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        return (value is bool boolValue && boolValue)
            ? Visibility.Visible
            : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        return value is Visibility visibility && visibility == Visibility.Visible;
    }
}

/// <summary>
/// Converte status de veículo para cor
/// </summary>
public class StatusToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is VehicleStatus status)
        {
            return status switch
            {
                VehicleStatus.Available => new SolidColorBrush(Windows.UI.Color.FromArgb(255, 76, 175, 80)), // Verde
                VehicleStatus.InUse => new SolidColorBrush(Windows.UI.Color.FromArgb(255, 33, 150, 243)),    // Azul
                VehicleStatus.OutOfService => new SolidColorBrush(Windows.UI.Color.FromArgb(255, 244, 67, 54)), // Vermelho
                _ => new SolidColorBrush(Windows.UI.Color.FromArgb(255, 158, 158, 158)),  // Cinza
            };
        }

        return new SolidColorBrush(Windows.UI.Color.FromArgb(255, 158, 158, 158)); // Cinza como padrão
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Converte status de motorista para cor
/// </summary>
public class DriverStatusToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is DriverStatus status)
        {
            return status switch
            {
                DriverStatus.Available => new SolidColorBrush(Windows.UI.Color.FromArgb(255, 76, 175, 80)),   // Verde
                DriverStatus.OnDuty => new SolidColorBrush(Windows.UI.Color.FromArgb(255, 33, 150, 243)),     // Azul
                DriverStatus.OnLeave => new SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 193, 7)),     // Amarelo
                DriverStatus.Inactive => new SolidColorBrush(Windows.UI.Color.FromArgb(255, 158, 158, 158)),  // Cinza
                _ => new SolidColorBrush(Windows.UI.Color.FromArgb(255, 158, 158, 158)),  // Cinza como padrão
            };
        }

        return new SolidColorBrush(Windows.UI.Color.FromArgb(255, 158, 158, 158)); // Cinza como padrão
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Formata distância adicionando "km"
/// </summary>
public class DistanceFormatter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is double distance)
        {
            return $"{distance:N0} km";
        }

        return "0 km";
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Formata TimeSpan para exibição amigável
/// </summary>
public class TimeSpanFormatter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is TimeSpan timeSpan)
        {
            if (timeSpan.Days > 0)
            {
                return $"{timeSpan.Days} dia(s), {timeSpan.Hours} hora(s) e {timeSpan.Minutes} minuto(s)";
            }
            else if (timeSpan.Hours > 0)
            {
                return $"{timeSpan.Hours} hora(s) e {timeSpan.Minutes} minuto(s)";
            }
            else
            {
                return $"{timeSpan.Minutes} minuto(s)";
            }
        }

        return "Duração desconhecida";
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Converte booleano para cor (verde para verdadeiro, vermelho para falso)
/// </summary>
public class BoolToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is bool boolValue)
        {
            return boolValue
                ? new SolidColorBrush(Windows.UI.Color.FromArgb(255, 76, 175, 80))   // Verde
                : new SolidColorBrush(Windows.UI.Color.FromArgb(255, 244, 67, 54));  // Vermelho
        }

        return new SolidColorBrush(Windows.UI.Color.FromArgb(255, 158, 158, 158)); // Cinza como padrão
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Converte booleano para texto de validade
/// </summary>
public class BoolToValidityTextConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is bool boolValue)
        {
            return boolValue ? "Válida" : "Vencida";
        }

        return "Desconhecido";
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Converte DateTime para string formatada
/// </summary>
public class DateTimeToStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is DateTime dateTime)
        {
            return dateTime.ToString("dd/MM/yyyy");
        }

        return "Data desconhecida";
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        if (value is string dateString && DateTime.TryParseExact(
            dateString,
            "dd/MM/yyyy",
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out DateTime result))
        {
            return result;
        }

        return DateTime.Now;
    }
}