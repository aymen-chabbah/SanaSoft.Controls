using System.Windows;
using System.Windows.Media;

namespace SanaSoft.Controls.Samples.Themes;

/// <summary>
/// SanaSoft brand colors and logo artwork.
/// </summary>
/// <remarks>
/// Use from XAML with <c>{x:Static themes:SanaSoftBrand.BlueBrush}</c>. Every brush, geometry and image is frozen,
/// so it can be shared freely across threads and windows.
/// </remarks>
public static class SanaSoftBrand
{
    public static readonly Color Blue = Color.FromRgb(0x14, 0x7B, 0xF8);

    public static readonly Color Slate = Color.FromRgb(0x2B, 0x30, 0x3A);

    public static readonly SolidColorBrush BlueBrush = Freeze(new SolidColorBrush(Blue));

    public static readonly SolidColorBrush SlateBrush = Freeze(new SolidColorBrush(Slate));

    /// <summary>The logo mark: a diagonal bar between two triangles, 40.5 x 34 units.</summary>
    public static readonly Geometry LogoMarkGeometry = Freeze(Geometry.Parse(
        "M33,0 L40.5,0 7.5,34 0,34 Z M7,0 L17.75,10.75 7,21.5 Z M33.5,12.5 L33.5,34 22.75,23.25 Z"));

    /// <summary>The logo mark in brand blue, e.g. for <see cref="Window.Icon"/>.</summary>
    public static readonly DrawingImage LogoMarkImage = Freeze(new DrawingImage(new GeometryDrawing(BlueBrush, null, LogoMarkGeometry)));

    /// <summary>
    /// Resource key of the brush used for the "SanaSoft" wordmark. Slate by default;
    /// <see cref="FluentBranding"/> switches it to white in the dark theme.
    /// </summary>
    public static readonly ComponentResourceKey WordmarkBrushKey = new(typeof(SanaSoftBrand), "WordmarkBrush");

    private static T Freeze<T>(T freezable) where T : Freezable
    {
        freezable.Freeze();
        return freezable;
    }
}
