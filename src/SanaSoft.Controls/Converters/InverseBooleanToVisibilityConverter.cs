using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SanaSoft.Controls.Converters;

/// <summary>Converts <see langword="true"/> to <see cref="Visibility.Collapsed"/> and anything else to <see cref="Visibility.Visible"/>.</summary>
[ValueConversion(typeof(bool), typeof(Visibility))]
public sealed class InverseBooleanToVisibilityConverter : IValueConverter
{
    /// <inheritdoc />
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
        value is true ? Visibility.Collapsed : Visibility.Visible;

    /// <inheritdoc />
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        value is not Visibility.Visible;
}
