using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Media;
using SanaSoft.Controls.Samples.Themes;

namespace SanaSoft.Controls.Samples.Controls;

/// <summary>
/// The SanaSoft logo: the mark followed by the "SanaSoft" wordmark.
/// </summary>
/// <remarks>
/// Set <see cref="FrameworkElement.Height"/> to size it; the mark and wordmark scale together.
/// The wordmark uses <see cref="Control.Foreground"/>, which follows <see cref="SanaSoftBrand.WordmarkBrushKey"/> by default.
/// </remarks>
public class SanaSoftLogo : Control
{
    public static readonly DependencyProperty ShowWordmarkProperty = DependencyProperty.Register(
        nameof(ShowWordmark), typeof(bool), typeof(SanaSoftLogo), new PropertyMetadata(true));

    public static readonly DependencyProperty MarkBrushProperty = DependencyProperty.Register(
        nameof(MarkBrush), typeof(Brush), typeof(SanaSoftLogo), new PropertyMetadata(SanaSoftBrand.BlueBrush));

    static SanaSoftLogo()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(SanaSoftLogo), new FrameworkPropertyMetadata(typeof(SanaSoftLogo)));
        FocusableProperty.OverrideMetadata(typeof(SanaSoftLogo), new FrameworkPropertyMetadata(false));
    }

    /// <summary>Whether "SanaSoft" is shown next to the mark. True by default.</summary>
    public bool ShowWordmark
    {
        get => (bool)GetValue(ShowWordmarkProperty);
        set => SetValue(ShowWordmarkProperty, value);
    }

    /// <summary>Fill of the mark. Brand blue by default.</summary>
    public Brush MarkBrush
    {
        get => (Brush)GetValue(MarkBrushProperty);
        set => SetValue(MarkBrushProperty, value);
    }

    // Without a peer, a custom control is invisible to UI Automation (screen readers and UI tests).
    protected override AutomationPeer OnCreateAutomationPeer() => new SanaSoftLogoAutomationPeer(this);

    private sealed class SanaSoftLogoAutomationPeer(SanaSoftLogo owner) : FrameworkElementAutomationPeer(owner)
    {
        protected override AutomationControlType GetAutomationControlTypeCore() => AutomationControlType.Image;

        protected override string GetClassNameCore() => nameof(SanaSoftLogo);

        protected override string GetNameCore() => base.GetNameCore() is { Length: > 0 } name ? name : "SanaSoft";

        // The template's Path and TextBlock are artwork, not separate elements.
        protected override List<AutomationPeer>? GetChildrenCore() => null;
    }
}
