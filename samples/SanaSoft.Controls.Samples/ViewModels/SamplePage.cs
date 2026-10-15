using System.Windows;

namespace SanaSoft.Controls.Samples.ViewModels;

/// <summary>
/// An entry in the navigation pane. The view is created the first time the page is shown.
/// </summary>
public sealed class SamplePage(string title, string description, string glyph, Func<FrameworkElement> createView, string? xamlTag = null)
{
    private FrameworkElement? _view;

    public string Title { get; } = title;

    public string Description { get; } = description;

    /// <summary>Segoe Fluent Icons glyph shown in the navigation pane.</summary>
    public string Glyph { get; } = glyph;

    /// <summary>How the control is written in XAML, e.g. <c>ss:WatermarkTextBox</c>. Null for pages that are not about one control.</summary>
    public string? XamlTag { get; } = xamlTag;

    public FrameworkElement View => _view ??= createView();
}
