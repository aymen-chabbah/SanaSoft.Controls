using System.Collections;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using SanaSoft.Controls.Themes;

namespace SanaSoft.Controls.Samples.Themes;

/// <summary>
/// Brands the WPF Fluent theme (<c>Application.ThemeMode</c>) with the SanaSoft blue instead of the Windows accent color,
/// and keeps the SanaSoft.Controls tokens (<see cref="SanaSoftTheme"/>) on the same light/dark palette as Fluent.
/// </summary>
/// <remarks>
/// The Fluent theme bakes the Windows accent shades into many brushes (buttons, check boxes, selection, focus),
/// so every theme brush that uses a Windows accent shade is overridden with the matching brand shade.
/// The overrides are refreshed automatically whenever WPF reloads its theme (light/dark switch, ThemeMode change,
/// or the Windows setting changing while ThemeMode is System).
/// </remarks>
public static class FluentBranding
{
    // Windows accent shade -> brand shade. Fluent fills with Dark1 in light mode and Light2 in dark mode.
    private static readonly (Func<Color> System, Color Brand)[] Shades =
    [
        (() => SystemColors.AccentColorLight3, Color.FromRgb(0xA9, 0xCF, 0xFD)),
        (() => SystemColors.AccentColorLight2, Color.FromRgb(0x64, 0xA7, 0xFA)),
        (() => SystemColors.AccentColorLight1, Color.FromRgb(0x3D, 0x90, 0xF9)),
        (() => SystemColors.AccentColor, SanaSoftBrand.Blue),
        (() => SystemColors.AccentColorDark1, SanaSoftBrand.Blue),
        (() => SystemColors.AccentColorDark2, Color.FromRgb(0x0B, 0x5C, 0xC2)),
        (() => SystemColors.AccentColorDark3, Color.FromRgb(0x07, 0x3E, 0x85)),
    ];

    private static readonly SolidColorBrush WhiteBrush = Freeze(new SolidColorBrush(Colors.White));

    // Keys this class added to each resource dictionary, so they can be removed before re-applying.
    private static readonly ConditionalWeakTable<ResourceDictionary, List<object>> OverriddenKeys = new();

    // Resource dictionaries already re-applied on theme changes.
    private static readonly ConditionalWeakTable<ResourceDictionary, object> Watched = new();

    /// <summary>
    /// Applies the SanaSoft brand to the application's theme and keeps it applied when the theme changes.
    /// Call once at startup, e.g. in <c>OnStartup</c>.
    /// </summary>
    public static void Apply(Application application)
    {
        ArgumentNullException.ThrowIfNull(application);

        var resources = application.Resources;
        Apply(resources);

        // WPF swaps the Fluent dictionary in MergedDictionaries whenever the theme changes.
        if (resources.MergedDictionaries is INotifyCollectionChanged themes && Watched.TryAdd(resources, new object()))
            themes.CollectionChanged += (_, _) => Apply(resources);
    }

    private static void Apply(ResourceDictionary resources)
    {
        var keys = OverriddenKeys.GetOrCreateValue(resources);
        foreach (var key in keys)
            resources.Remove(key);
        keys.Clear();

        var shades = Shades
            .Select(shade => (System: Rgb(shade.System()), shade.Brand))
            .DistinctBy(shade => shade.System)
            .ToDictionary(shade => shade.System, shade => shade.Brand);

        var isDark = false;
        foreach (var dictionary in Flatten(resources.MergedDictionaries))
        {
            isDark |= dictionary.Source?.OriginalString.Contains("Fluent.Dark", StringComparison.OrdinalIgnoreCase) == true;

            foreach (DictionaryEntry entry in dictionary)
            {
                object? replacement = entry.Value switch
                {
                    SolidColorBrush brush when shades.TryGetValue(Rgb(brush.Color), out var brand) =>
                        Freeze(new SolidColorBrush(WithAlpha(brand, brush.Color.A)) { Opacity = brush.Opacity }),
                    Color color when shades.TryGetValue(Rgb(color), out var brand) => WithAlpha(brand, color.A),
                    _ => null,
                };

                if (replacement is null)
                    continue;

                resources[entry.Key] = replacement;
                keys.Add(entry.Key);
            }
        }

        resources[SanaSoftBrand.WordmarkBrushKey] = isDark ? WhiteBrush : SanaSoftBrand.SlateBrush;
        keys.Add(SanaSoftBrand.WordmarkBrushKey);

        // The library controls use their own tokens; keep them on the palette Fluent just loaded.
        foreach (var sanaSoftTheme in resources.MergedDictionaries.OfType<SanaSoftTheme>())
            sanaSoftTheme.Theme = isDark ? ThemeVariant.Dark : ThemeVariant.Light;
    }

    private static IEnumerable<ResourceDictionary> Flatten(IEnumerable<ResourceDictionary> dictionaries) =>
        dictionaries.SelectMany(dictionary => Flatten(dictionary.MergedDictionaries).Prepend(dictionary));

    private static uint Rgb(Color color) => (uint)(color.R << 16 | color.G << 8 | color.B);

    private static Color WithAlpha(Color color, byte alpha) => Color.FromArgb(alpha, color.R, color.G, color.B);

    private static SolidColorBrush Freeze(SolidColorBrush brush)
    {
        brush.Freeze();
        return brush;
    }
}
