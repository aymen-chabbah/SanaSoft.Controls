using System.Windows;

namespace SanaSoft.Controls.Themes;

/// <summary>
/// Resource dictionary that loads the SanaSoft design tokens for the chosen <see cref="Theme"/>.
/// </summary>
/// <example>
/// Merge it once in App.xaml:
/// <code><![CDATA[
/// <Application.Resources>
///   <ResourceDictionary>
///     <ResourceDictionary.MergedDictionaries>
///       <ss:SanaSoftTheme Theme="Light" />
///     </ResourceDictionary.MergedDictionaries>
///   </ResourceDictionary>
/// </Application.Resources>
/// ]]></code>
/// Changing <see cref="Theme"/> at runtime restyles every control that uses the tokens through DynamicResource.
/// </example>
public class SanaSoftTheme : ResourceDictionary
{
    private ThemeVariant _theme;

    /// <summary>Creates the dictionary with the <see cref="ThemeVariant.Light"/> palette.</summary>
    public SanaSoftTheme() => Load();

    /// <summary>The palette to load.</summary>
    public ThemeVariant Theme
    {
        get => _theme;
        set
        {
            if (_theme == value)
                return;

            _theme = value;
            Load();
        }
    }

    private void Load()
    {
        MergedDictionaries.Clear();
        MergedDictionaries.Add(Token("Typography"));
        MergedDictionaries.Add(Token("Spacing"));
        MergedDictionaries.Add(Token(_theme == ThemeVariant.Dark ? "Dark" : "Light"));
    }

    // Component-relative: WPF resolves it without needing a running Application (design time, tests, hosted WPF).
    private static ResourceDictionary Token(string name) =>
        new() { Source = new Uri($"/SanaSoft.Controls;component/Themes/Tokens/{name}.xaml", UriKind.Relative) };
}
