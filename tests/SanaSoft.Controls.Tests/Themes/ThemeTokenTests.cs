using System.Reflection;
using System.Windows;
using SanaSoft.Controls.Themes;

namespace SanaSoft.Controls.Tests.Themes;

public class ThemeTokenTests
{
    public static TheoryData<ThemeVariant> Variants => new() { ThemeVariant.Light, ThemeVariant.Dark };

    [StaTheory]
    [MemberData(nameof(Variants))]
    public void Every_resource_key_has_a_value(ThemeVariant variant)
    {
        var theme = new SanaSoftTheme { Theme = variant };

        var missing = AllKeys().Where(key => !theme.Contains(key)).Select(key => key.ResourceId).ToList();

        missing.Should().BeEmpty();
    }

    [StaFact]
    public void Switching_the_theme_replaces_the_palette()
    {
        var theme = new SanaSoftTheme();
        var light = theme[ResourceKeys.BackgroundBrush];

        theme.Theme = ThemeVariant.Dark;

        theme[ResourceKeys.BackgroundBrush].Should().NotBeSameAs(light);
    }

    [StaFact]
    public void Generic_dictionary_provides_light_tokens_as_fallback()
    {
        var control = new NumericBox();

        control.ApplyTemplate();

        control.TryFindResource(ResourceKeys.AccentBrush).Should().NotBeNull();
    }

    private static IEnumerable<ComponentResourceKey> AllKeys() =>
        typeof(ResourceKeys)
            .GetProperties(BindingFlags.Public | BindingFlags.Static)
            .Select(property => (ComponentResourceKey)property.GetValue(null)!);
}
