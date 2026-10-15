using System.Windows;

namespace SanaSoft.Controls.Themes;

/// <summary>
/// Keys of the design tokens used by every SanaSoft control template.
/// </summary>
/// <remarks>
/// Reference a token with <c>{DynamicResource {x:Static ss:ResourceKeys.AccentBrush}}</c> so it follows theme switches.
/// The default (light) values are always available from the library's Generic.xaml;
/// <see cref="SanaSoftTheme"/> overrides them per application.
/// </remarks>
public static class ResourceKeys
{
    // Brushes (Themes/Tokens/Light.xaml and Dark.xaml)

    /// <summary>Window and page background.</summary>
    public static ComponentResourceKey BackgroundBrush { get; } = Create(nameof(BackgroundBrush));

    /// <summary>Background of input controls and cards.</summary>
    public static ComponentResourceKey SurfaceBrush { get; } = Create(nameof(SurfaceBrush));

    /// <summary>Background of a surface under the mouse.</summary>
    public static ComponentResourceKey SurfaceHoverBrush { get; } = Create(nameof(SurfaceHoverBrush));

    /// <summary>Background of a surface being pressed.</summary>
    public static ComponentResourceKey SurfacePressedBrush { get; } = Create(nameof(SurfacePressedBrush));

    /// <summary>Background of a disabled surface.</summary>
    public static ComponentResourceKey DisabledSurfaceBrush { get; } = Create(nameof(DisabledSurfaceBrush));

    /// <summary>Default border of input controls.</summary>
    public static ComponentResourceKey BorderBrush { get; } = Create(nameof(BorderBrush));

    /// <summary>Border of an input control under the mouse.</summary>
    public static ComponentResourceKey BorderHoverBrush { get; } = Create(nameof(BorderHoverBrush));

    /// <summary>Primary text color.</summary>
    public static ComponentResourceKey ForegroundBrush { get; } = Create(nameof(ForegroundBrush));

    /// <summary>Text color for labels, hints and glyphs.</summary>
    public static ComponentResourceKey SecondaryForegroundBrush { get; } = Create(nameof(SecondaryForegroundBrush));

    /// <summary>Text color of disabled controls.</summary>
    public static ComponentResourceKey DisabledForegroundBrush { get; } = Create(nameof(DisabledForegroundBrush));

    /// <summary>Brand color: focus rings, selection, primary actions.</summary>
    public static ComponentResourceKey AccentBrush { get; } = Create(nameof(AccentBrush));

    /// <summary>Accent color under the mouse.</summary>
    public static ComponentResourceKey AccentHoverBrush { get; } = Create(nameof(AccentHoverBrush));

    /// <summary>Accent color being pressed.</summary>
    public static ComponentResourceKey AccentPressedBrush { get; } = Create(nameof(AccentPressedBrush));

    /// <summary>Text drawn on top of <see cref="AccentBrush"/>.</summary>
    public static ComponentResourceKey AccentForegroundBrush { get; } = Create(nameof(AccentForegroundBrush));

    // Typography (Themes/Tokens/Typography.xaml)

    /// <summary>Font family of all controls (<see cref="System.Windows.Media.FontFamily"/>).</summary>
    public static ComponentResourceKey FontFamily { get; } = Create(nameof(FontFamily));

    /// <summary>Body text size (<see cref="double"/>).</summary>
    public static ComponentResourceKey FontSizeBody { get; } = Create(nameof(FontSizeBody));

    /// <summary>Small text size for captions and hints (<see cref="double"/>).</summary>
    public static ComponentResourceKey FontSizeCaption { get; } = Create(nameof(FontSizeCaption));

    /// <summary>Title text size (<see cref="double"/>).</summary>
    public static ComponentResourceKey FontSizeTitle { get; } = Create(nameof(FontSizeTitle));

    // Spacing and shape (Themes/Tokens/Spacing.xaml)

    /// <summary>Corner radius of input controls (<see cref="System.Windows.CornerRadius"/>).</summary>
    public static ComponentResourceKey ControlCornerRadius { get; } = Create(nameof(ControlCornerRadius));

    /// <summary>Border thickness of input controls (<see cref="Thickness"/>).</summary>
    public static ComponentResourceKey ControlBorderThickness { get; } = Create(nameof(ControlBorderThickness));

    /// <summary>Inner padding of input controls (<see cref="Thickness"/>).</summary>
    public static ComponentResourceKey ControlPadding { get; } = Create(nameof(ControlPadding));

    /// <summary>Minimum height of single-line input controls (<see cref="double"/>).</summary>
    public static ComponentResourceKey ControlMinHeight { get; } = Create(nameof(ControlMinHeight));

    private static ComponentResourceKey Create(string id) => new(typeof(ResourceKeys), id);
}
