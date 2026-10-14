using System.Globalization;
using System.Windows;
using Sanasoft.Shared.Wpf.Converters;

namespace Sanasoft.Shared.Wpf.Tests.Converters;

public class InverseBooleanToVisibilityConverterTests
{
    [Fact]
    public void Collapses_when_the_value_is_true()
    {
        var sut = new InverseBooleanToVisibilityConverter();

        var actual = Convert(sut, true);

        actual.Should().Be(Visibility.Collapsed);
    }

    [Fact]
    public void Shows_when_the_value_is_false()
    {
        var sut = new InverseBooleanToVisibilityConverter();

        var actual = Convert(sut, false);

        actual.Should().Be(Visibility.Visible);
    }

    [Fact]
    public void Shows_when_the_value_is_null()
    {
        var sut = new InverseBooleanToVisibilityConverter();

        var actual = Convert(sut, null);

        actual.Should().Be(Visibility.Visible);
    }

    [Fact]
    public void Shows_when_the_value_is_not_a_boolean()
    {
        var sut = new InverseBooleanToVisibilityConverter();

        var actual = Convert(sut, "true");

        actual.Should().Be(Visibility.Visible);
    }

    [Fact]
    public void Converts_visible_back_to_false()
    {
        var sut = new InverseBooleanToVisibilityConverter();

        var actual = ConvertBack(sut, Visibility.Visible);

        actual.Should().Be(false);
    }

    [Fact]
    public void Converts_collapsed_back_to_true()
    {
        var sut = new InverseBooleanToVisibilityConverter();

        var actual = ConvertBack(sut, Visibility.Collapsed);

        actual.Should().Be(true);
    }

    [Fact]
    public void Converts_hidden_back_to_true()
    {
        var sut = new InverseBooleanToVisibilityConverter();

        var actual = ConvertBack(sut, Visibility.Hidden);

        actual.Should().Be(true);
    }

    private static object Convert(InverseBooleanToVisibilityConverter sut, object? value) =>
        sut.Convert(value!, typeof(Visibility), null!, CultureInfo.InvariantCulture);

    private static object ConvertBack(InverseBooleanToVisibilityConverter sut, Visibility value) =>
        sut.ConvertBack(value, typeof(bool), null!, CultureInfo.InvariantCulture);
}
