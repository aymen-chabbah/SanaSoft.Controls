using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;

namespace SanaSoft.Controls.Samples.Controls;

/// <summary>
/// A card that shows a live example and the XAML that produces it.
/// </summary>
[TemplatePart(Name = CopyButtonPart, Type = typeof(ButtonBase))]
public class ExampleCard : ContentControl
{
    private const string CopyButtonPart = "PART_CopyButton";
    private const string CopyGlyph = "";
    private const string CopiedGlyph = "";

    public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(
        nameof(Header), typeof(string), typeof(ExampleCard), new PropertyMetadata(null));

    public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register(
        nameof(Description), typeof(string), typeof(ExampleCard), new PropertyMetadata(null));

    public static readonly DependencyProperty CodeProperty = DependencyProperty.Register(
        nameof(Code), typeof(string), typeof(ExampleCard), new PropertyMetadata(null));

    private ButtonBase? _copyButton;
    private DispatcherTimer? _resetTimer;

    public string? Header
    {
        get => (string?)GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    public string? Description
    {
        get => (string?)GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    public string? Code
    {
        get => (string?)GetValue(CodeProperty);
        set => SetValue(CodeProperty, value);
    }

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        if (_copyButton is not null)
            _copyButton.Click -= OnCopyClick;

        _copyButton = GetTemplateChild(CopyButtonPart) as ButtonBase;

        if (_copyButton is not null)
            _copyButton.Click += OnCopyClick;
    }

    private void OnCopyClick(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(Code) || _copyButton is null)
            return;

        Clipboard.SetText(Code);

        // Show a check mark briefly so the user knows the copy worked.
        _copyButton.Content = CopiedGlyph;
        _resetTimer ??= new DispatcherTimer(TimeSpan.FromSeconds(1.5), DispatcherPriority.Normal, OnResetTimerTick, Dispatcher);
        _resetTimer.Stop();
        _resetTimer.Start();
    }

    private void OnResetTimerTick(object? sender, EventArgs e)
    {
        _resetTimer?.Stop();

        if (_copyButton is not null)
            _copyButton.Content = CopyGlyph;
    }
}
